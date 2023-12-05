/// <summary>
/// BestHttp의 WebSocket에 관한 정보는 https://benedicht.github.io/BestHTTP-Documentation/pages/best_http2/protocols/websocket/websocket.html 문서를 참고
/// </summary>

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using BestHTTP.WebSocket;
using BestHTTP.SocketIO3.Events;
using UnityEngine;
using System.Linq;
using System.Timers;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BestHTTP.JSON;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;

public enum SocketState
{
    READY,
    CONNECT,
    DISCONNECT,
    RECONNECTING,
    RECONNECTFAIL,
    ERROR
}

public enum JoinType
{
    JoinGame,
    ReJoinGame,
    PlayingRejonGame
}

public class SocketData
{
    public long tickCout;
    public Action eventAction;
    //public ProtoMsgType actionName;
    public long pingTime;
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

public static class ServerTick<T>
{
    public readonly static Func<T, long> _getter;

    static ServerTick()
    {
        _getter = PropertyHelper.GetGetter<T, long>("Time");
    }
    public static long Value(T data) => _getter(data);
}

public static class SeqNum<T>
{
    public readonly static Func<T, long> _getter;

    static SeqNum()
    {
        _getter = PropertyHelper.GetGetter<T, long>("Seq");
    }
    public static long Value(T data) => _getter(data);
}

public static class ServerTickHelper
{
    public static long GetServerTick<T>(this T data)
    {
        return ServerTick<T>.Value(data);
    }
}


public static class SeqNumHelper
{
    public static long GetSegNum<T>(this T data)
    {
        return SeqNum<T>.Value(data);
    }
}

public static class PropertyHelper
{
    public static Func<T, TRet> GetGetter<T, TRet>(string name)
    {
        var getter = typeof(T).GetProperty(name)?.GetMethod;
        if (null == getter)
            return null;

        //*
        return (Func<T, TRet>)Delegate.CreateDelegate(typeof(Func<T, TRet>), getter);
        /*/
        Func<MethodInfo, Func<T, TRet>> magicMethodHelper = _MagicMethod<T, TRet>;
        var magicMethod = magicMethodHelper.GetMethodInfo().GetGenericMethodDefinition()
            .MakeGenericMethod(typeof(T), typeof(TRet));
        
        return (Func<T, TRet>) magicMethod.Invoke(null, new object[] {getter})!;
        //*/
    }

    private static Func<T, TRet> _MagicMethod<T, TRet>(MethodInfo methodInfo)
    {
        var method = (Func<T, TRet>)Delegate.CreateDelegate(typeof(Func<T, TRet>), methodInfo);
        return (T target) => method(target);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public partial class SocketClient : SingletonComponentBase<SocketClient>, IDisposable
{
    //public DelegateEventObject<ConnectResponse> OnConnectEvent = new DelegateEventObject<ConnectResponse>();


    private string Address;
    public string targetGame = "coin";

    // @TODO Connect Backend for get jwt
    // @Author @Sunrabbit

    private WebSocket webSocket = null;
    public bool isDisconnected = false;

    #region 시간 동기화 관련 변수

    private Stopwatch time = Stopwatch.StartNew();
    public long preAskTime = 0;
    private long askTime = 0;
    private long timeGap = 0;



    #endregion


    //private const int BUFFER_SIZE = 16384; // 2^14

    //private IntPtr bufferPointer = Marshal.AllocHGlobal(BUFFER_SIZE);

    private byte[] reconnectData = null;
    private byte[] retryData = null;

    private Dictionary<string, object> deserializeParser = new Dictionary<string, object>();

    private Dictionary<int, Action<MemoryStream>> responseDictionary = new();

    public Action<bool> onSocketConnectCallBack = null; //io통신용 웹소켓 연결 콜백


    //서버에게 fin 보내서 finack 받았을 때 호출되는 콜백. 웹소켓 disconnect 때 아님!!
    private Action disconnectAction;

    private DateTime checkReconnectTime;

    public SocketState sockectState = SocketState.READY;
    public JoinType joinType = JoinType.JoinGame;
    public bool isRejoinEnd = false;

    private bool isOpenPopUp = false;
    public long save_userUidForFin = 0;

    public Queue<SocketData> socketQueue = new();
    public long pingCount = 0;

    public bool isSync = false;

    private int onErrorCount = 0;

    public bool isFocusOut = false;

    public int queueCount = 0;

    public void Dispose()
    {

        WebSocketDisconnect(() =>
        {
            Debug.Log("Disconnect dispose");
        });
    }


    protected override void InitializeSingleton()
    {

    }

    private int connectionTick = Environment.TickCount;

    public long GetClientTick()
    {
        return (long)time.Elapsed.TotalMilliseconds;
        //return Environment.TickCount - connectionTick;
    }

    public void Update()
    {
        return;
        if (sockectState != SocketState.CONNECT) return;

        //Debug.Log($"socketQueue.Count ::: {socketQueue.Count}");
        queueCount = socketQueue.Count;

        //while (socketQueue.Count > 0)
        //{
        //    var firstData = socketQueue.Peek();

        //    if (firstData.actionName == ProtoMsgType.S2CProgressTimeSync || firstData.actionName == ProtoMsgType.S2CJoinGame || firstData.actionName == ProtoMsgType.S2CRefreshViewData)
        //    {
        //        socketQueue.Dequeue().eventAction();
        //        break;
        //    }
        //    /*
        //    else if (150 - firstData.pingTime > 150)
        //    {
        //        socketQueue.Dequeue().eventAction();
        //        break;
        //    }
        //    */
        //    else if (Math.Abs(GetServerTime() - firstData.tickCout) > 50)
        //    {
        //        //Debug.Log("test :: " + ( GetServerTime() - firstData.tickCout ) + ", pingTime :: " + (100 - firstData.pingTime));
        //        try
        //        {
        //            socketQueue.Dequeue().eventAction();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.Log(e);
        //        }
        //    }
        //    else
        //        break;
        //}

        /*
        if (sockectState != SocketState.CONNECT) return;
        //Debug.Log($"socketQueue.Count ::: {socketQueue.Count}");

        while (socketQueue.Count > 0)
        {
            var firstData = socketQueue.Peek();

            if (firstData.actionName == ProtoMsgType.S2CProgressTimeSync)
            {
                socketQueue.Dequeue().eventAction();
                break;
            }

            else if ((GetServerTime() - firstData.tickCout) > 500)
            {
                //Debug.Log($"Dequeue data name : {firstData.actionName}, time : {firstData.tickCout}, GapTime : {(GetServerTime() - firstData.tickCout)}");
               
                
                if (firstData.actionName == ProtoMsgType.S2CRevengeTargetData
                    || firstData.actionName == ProtoMsgType.S2CLeaderBoard
                    || firstData.actionName == ProtoMsgType.S2CMinimapData
                    || firstData.actionName == ProtoMsgType.S2CEndGame)
                {
                    socketQueue.Dequeue().eventAction();
                }

                else if (GameMgr.Instance.Player != null && !GameMgr.Instance.Player.isMovementSync)
                {
                    if (firstData.actionName == ProtoMsgType.S2CRefreshViewData)
                    {
                        if (isSync == false && isFocusOut == false)
                        {
                            Send_RefreshViewData();
                            NetworkManager.Instance.SetLoading(true);
                        }
                        socketQueue.Dequeue().eventAction();
                    }
                    else
                    {
                        if (isSync == false && isFocusOut == false)
                        {
                            Send_RefreshViewData();
                            NetworkManager.Instance.SetLoading(true);
                        }
                        socketQueue.Dequeue();
                    }
                }
                else socketQueue.Dequeue().eventAction();
            }
            else
            {
                try
                {
                    if (GameMgr.Instance.Player != null && GameMgr.Instance.Player.isMovementSync)
                    {
                        if (firstData.actionName == ProtoMsgType.S2CUserData)
                        {
                            socketQueue.Dequeue();
                        }
                    }
                    else
                    {
                        socketQueue.Dequeue().eventAction();
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }*/
    }

    //public SocketClient()
    //{
    //    // Define으로 접속서버 주소 변경
    //    AddProtocol<S2C_UserData>(ProtoMsgType.S2CUserData, Recv_UserData);
    //    AddProtocol<S2C_CollisionData>(ProtoMsgType.S2CCollisionData, Recv_CollisionData);
    //    AddProtocol<S2C_ProgressTimeSync>(ProtoMsgType.S2CProgressTimeSync, Recv_ProgressTimeSync);
    //    AddProtocol<S2C_JoinGame>(ProtoMsgType.S2CJoinGame, Recv_JoinGame);
    //    AddProtocol<S2C_FoodData>(ProtoMsgType.S2CFoodData, Recv_FoodData);
    //    AddProtocol<S2C_EatFoodData>(ProtoMsgType.S2CEatFoodData, Recv_EatFoodData);
    //    AddProtocol<S2C_FoodRemoveData>(ProtoMsgType.S2CFoodRemoveData, Recv_FoodRemoveData);
    //    //AddProtocol<S2C_OnConnect>(ProtoMsgType.S2COnConnect, Recv_OnConnect);
    //    AddProtocol<S2C_MinimapData>(ProtoMsgType.S2CMinimapData, Recv_MinimapData);
    //    AddProtocol<S2C_BountyInform>(ProtoMsgType.S2CBountyInform, Recv_BountyInform);
    //    AddProtocol<S2C_BountyFinish>(ProtoMsgType.S2CBountyFinish, Recv_BountyFinish);
    //    AddProtocol<S2C_BountyGetPrize>(ProtoMsgType.S2CBountyGetPrize, Recv_BountyGetPrize);
    //    AddProtocol<S2C_LeaderBoard>(ProtoMsgType.S2CLeaderBoard, Recv_LeaderBoard);
    //    AddProtocol<S2C_EatSpecialItemData>(ProtoMsgType.S2CEatSpecialItemData, Recv_EatSpecialItem);
    //    AddProtocol<S2C_UseSkill>(ProtoMsgType.S2CUseSkill, Recv_UseSkill);
    //    AddProtocol<S2C_EndGame>(ProtoMsgType.S2CEndGame, Recv_EndGame);
    //    AddProtocol<S2C_RevengeTargetData>(ProtoMsgType.S2CRevengeTargetData, Recv_RevengeTargetData);
    //    AddProtocol<S2C_FinishRevenge>(ProtoMsgType.S2CFinishRevenge, Recv_FinishRevenge);
    //    AddProtocol<S2C_GetProfile>(ProtoMsgType.S2CGetProfile, Recv_GetProfile);
    //    AddProtocol<S2C_TargetsUserData>(ProtoMsgType.S2CTargetsUserData, Recv_TargetUser);
    //    AddProtocol<S2C_ReJoinGame>(ProtoMsgType.S2CReJoinGame, Recv_ReJoinGame);
    //    AddProtocol<S2C_RefreshViewData>(ProtoMsgType.S2CRefreshViewData, Recv_RefreshViewData);
    //    AddProtocol<S2C_BotData>(ProtoMsgType.S2CBotData, Recv_BotData);
    //    AddProtocol<S2C_TargetsBotData>(ProtoMsgType.S2CTargetsBotData, Recv_TargetBot);
    //    AddProtocol<S2C_BotKillNotify>(ProtoMsgType.S2CBotKillNotify, Recv_BotKillNotify);
    //    AddProtocol<S2C_Error>(ProtoMsgType.S2CError, Recv_GameError);
    //}

    //private void AddProtocol<T>(ProtoMsgType _type, Action<T> _dataevent) where T : IMessage<T>, new()
    //{
    //    deserializeParser.Add(typeof(T).FullName, new MessageParser<T>(() => new T()));

    //    responseDictionary.Add((int)_type, (stream) =>
    //    {

    //        try
    //        {
    //            var value = ProtobufDeserialize<T>(stream);

    //            //Debug.Log("Receive :: " + _type + ", " + value );

    //            var serverTick = value.GetServerTick<T>();

    //            var pingTime = GetServerTime() - serverTick;

    //            var clientElapsedTick = (long)time.Elapsed.TotalMilliseconds - askTime;
    //            var serverElapsedTick = serverTick - serverEpochTickCount;


    //            //Debug.Log($"elapse Time :: {clientElapsedTick - serverElapsedTick}, {clientElapsedTick}/{serverElapsedTick}, {askTime}/{serverEpochTickCount} == {time.Elapsed.TotalMilliseconds:f2}");
    //            //Debug.Log("elapse Time :: " + (clientElapsedTick - serverElapsedTick));
    //            var temp = new SocketData
    //            {
    //                tickCout = serverTick,
    //                eventAction = () => _dataevent(value),
    //                actionName = _type,
    //                pingTime = pingTime
    //            };

    //            pingCount = pingTime;

    //            if (temp.actionName == ProtoMsgType.S2CRefreshViewData)
    //            {
    //                socketQueue.Clear();
    //            }
    //            else if (temp.actionName == ProtoMsgType.S2CError)
    //            {
    //                Debug.Log("S2CERROR :: " + temp.actionName);
    //                temp.eventAction();
    //            }
    //            else if (temp.actionName == ProtoMsgType.S2CUserData)
    //            {
    //                var tsc = preAskTime;

    //                var tec = (long)GetClientTick();

    //                var rtt = tec - tsc;

    //                var tc = tsc + rtt / 2;

    //                timeGap = serverTick - tc;

    //                askTime = preAskTime;
    //                SocketClient.Instance.preAskTime = SocketClient.Instance.GetClientTick();
    //                serverEpochTickCount = serverTick;

    //                //Debug.Log($"[tsc :: {tsc}] || [tec :: {tec}] || [rtt :: {rtt}] || [tc :: {tc}] || [timeGap :: {serverTick} - {tc}] = {timeGap}]");

    //                //Debug.Log($"[tsc :: {tsc}] || [tec :: {tec}] || [rtt :: {rtt}] || [tc :: {tc}] || [timeGap :: {serverTick} - {tc}] = {timeGap}]");

    //            }
    //            //Debug.Log($"[packet :: {_type}] || [elapse Time :: {clientElapsedTick - serverElapsedTick}] || [elapse Tick :: {clientElapsedTick}/{serverElapsedTick}], || [seq :: {seq}] || [gap : {timeGap}] || [ClientTick :: {GetClientTick()}] || [ClientTime :: {GetServerTime()}] || [ServerTime :: {serverTick}] || [Ping :: {pingCount}] || [Now :: {DateTime.Now.ToString("hh:mm:ss.fff")}]");



    //            //socketQueue.Enqueue(temp);
    //            temp.eventAction.Invoke();

    //        }
    //        catch (Exception e)
    //        {
    //            Debug.LogError(e);
    //        }
    //    });
    //}

    public void Init()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
#if USE_WEBGL_LOCAL_RAIN || USE_WEBGL_LOCAL_ANDERSAN
        Address = "wss://172.16.35.16:3000/monstertrain"; //본점
#elif USE_WEBGL_DEV
        Address = "wss://dev-tournament.playdapp.com/monstertrain?entry_currency=" + targetGame;
        //Address = "wss://172.16.35.16:3000/monstertrain"; //본점
#elif USE_WEBGL_QA
        Address = "wss://qa-tournament.playdapp.com/monstertrain?entry_currency=" + targetGame;
#elif USE_WEBGL_PROD
        Address = "wss://prod-tournament.playdapp.com/monstertrain?entry_currency=" + targetGame;
#endif
#elif UNITY_EDITOR
        Address = "ws://127.0.0.1:4001/spacecrush"; //본점
        //Address = "ws://172.16.35.42:2222/monstertrain"; //2호점
#endif

        Debug.Log("Start Connection");
        WebSocketConnect();
    }

    public long GetServerTime()
    {
        return GetClientTick() + timeGap;
    }

    #region WebSocket Handlers

    private bool isTryDisconnect = false;
    private void WebSocketConnect()
    {
        sockectState = SocketState.READY;
        // WebGL에서는 확장 정보를 사용할 수 없음
        if (webSocket != null && webSocket.State == WebSocketStates.Open)
        {
            isTryDisconnect = true;
            webSocket.Close();

            return;
        }
        Debug.Log($"address : {Address}");
        this.webSocket = new WebSocket(new Uri(Address), "true", "websocket");

        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnBinary += OnBinaryMessageReceived;
        webSocket.OnClosed += OnWebSocketClosed;
        webSocket.OnError += OnError;
        webSocket.Open();

        checkReconnectTime = DateTime.UtcNow;

        //StartCoroutine("RecvTimeCheck");
    }

    private void OnWebSocketOpen(WebSocket webSocket)
    {
        Debug.Log("WebSocket is now Open!");
        sockectState = SocketState.CONNECT;
        SocketRequest(SocketClient.IOCommand.Syn);

#if !UNITY_EDITOR && UNITY_WEBGL

        NetworkManager.Instance.SetLoading(false);
#endif
    }

    private void OnMessageReceived(WebSocket webSocket, string message)
    {

        //Debug.Log("Text Message received from server: " + message);
    }

    private void OnBinaryMessageReceived(WebSocket webSocket, byte[] message)
    {
//        // TODO(Diego) : 이곳에서 받은 메세지 처리
//        //Debug.Log("Binary Message received from server. Length: " + message.Length);
//        if (sockectState == SocketState.ERROR || sockectState == SocketState.DISCONNECT) return;
//        UIMgr.Instance.UnloadAllMessageBox();

//        ReadOnlySpan<byte> buffer = new ReadOnlySpan<byte>(message, 1, message.Length - 1);

//        var header = new WebSocketHeader
//        {
//            headerVersion = message[0],
//            flag = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer.Slice(0, 2))),
//            dataLengh = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer.Slice(2, 4))),
//            sequenceNumber = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer.Slice(6, 4))),
//            uid = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(buffer.Slice(10, 8))),
//        };

//        if (header.dataLengh <= 0)
//        {
//            Debug.Log($"Disconnect with server normally.");
//        }


//        if (header.flag == (ushort)IOCommand.UniCast)
//        {
//            //header.messageId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(message[19..23]));
//            //header.payLoadLengh = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(message[23..27]));

//            header.messageId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer.Slice(18, 4)));
//            header.payLoadLengh = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer.Slice(22, 4)));

//            if (header.messageId <= -1 || header.payLoadLengh <= 0)
//            {
//                Debug.Log($"Binary messageId or payload Error :: flag :: {header.flag}, messageId :: {header.messageId}, payloadlength :: {header.payLoadLengh}");

//            }

//            if (responseDictionary.TryGetValue(header.messageId, out var messageFunction))
//            {
//                //var span = new Span<byte>(message, 27, message.Length - 27);

//                //messageFunction(message[27..]);
//                //messageFunction(buffer.Slice(26).ToArray());
//                messageFunction(new MemoryStream(message, 27, message.Length - 27));
//                checkReconnectTime = DateTime.UtcNow;
//            }
//        }
//        else
//        {
//            //서버 <-> 클라 시간 동기화 시작
//            if (header.flag == (ushort)IOCommand.SynAck)
//            {
//                preAskTime = askTime = GetClientTick();

//                C2S_StartTimeSync payload = new()
//                {
//                    Time = askTime
//                };



//                SocketRequest(ProtoMsgType.C2SStartTimeSync, payload.ToByteArray());
//                checkReconnectTime = DateTime.UtcNow;

//            }
//            else if (header.flag == (ushort)IOCommand.FinAck || header.flag == (ushort)IOCommand.Fin)
//            {
//                sockectState = SocketState.DISCONNECT;
//                webSocket.Close();
//                webSocket = null;

//                disconnectAction?.Invoke();
//                disconnectAction = null;
//                NetworkManager.Instance.SetLoading(false);
//                onSocketConnectCallBack?.Invoke(false);
//                onSocketConnectCallBack = null;
//            }

//            else if ((header.flag & (ushort)IOCommand.Err) == (ushort)IOCommand.Err)
//            {
//                header.messageId = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(message[19..23]));

//                Debug.Log($"Binary error messageId or payload Error :: flag :: {header.flag}, messageId :: {header.messageId}, payloadlength :: {header.payLoadLengh}");
//                if (header.messageId == (int)ErrorCode.AlreadyExistsSession)
//                {
//                    sockectState = SocketState.CONNECT;
//                    UIMgr.Instance.ShowMessageBox(MessageButtonType.OkCancel, (isOK) =>
//                    {
//                        if (isOK)
//                        {
//                            SocketRequest(SocketClient.IOCommand.Syn);
//#if !UNITY_EDITOR && UNITY_WEBGL
//                        NetworkManager.Instance.SetLoading(false);
//#endif
//                        }
//                        else
//                        {
//                            SocketClient.Instance.WebSocketDisconnect(() =>
//                            {
//                                onSocketConnectCallBack?.Invoke(false);
//#if UNITY_EDITOR
//                                GameMgr.Instance.GameReStart();
//#else
//                            NetworkManager.Instance.GoToMain();
//#endif
//                            });

//                        }
//                    }, GameConfig.Redundant_MESSAGE, GameConfig.Redundant_MESSAGE);
//                }
//                else if (header.messageId == (int)ErrorCode.DuplicateConnection)
//                {
//                    sockectState = SocketState.DISCONNECT;

//                    UIMgr.Instance.ShowMessageBox(MessageButtonType.Ok, (isOK) =>
//                    {
//                        StopAllCoroutines();
//                        GameMgr.Instance.GameReStart();
//                    }, GameConfig.Redundant_MESSAGE, GameConfig.Redundant_MESSAGE + "\nGo To Lobby.");
//                }
//            }
//            else
//            {
//                Debug.Log($"Binary else flag Error {header.flag}");
//            }
//        }
    }

    private void OnWebSocketClosed(WebSocket webSocket, UInt16 code, string message)
    {
        Debug.Log(string.Format("WebSocket is now Closed! (reason: {0})", message));
        sockectState = SocketState.DISCONNECT;
        webSocket.Close();
        webSocket = null;
        disconnectAction?.Invoke();
        disconnectAction = null;
        //NetworkManager.Instance.SetLoading(false);

        if (isTryDisconnect)
        {
            WebSocketConnect();
            isTryDisconnect = false;
        }
    }


    private void OnError(WebSocket ws, string error)
    {

        //Debug.Log($"onError occured:: {error}");
        //sockectState = SocketState.ERROR;
        //UIMgr.Instance.ShowMessageBox(MessageButtonType.Ok, (isOK) =>
        //{
        //    if (onErrorCount < 3)
        //    {
        //        if (this.webSocket != null && this.sockectState != SocketState.CONNECT)
        //            WebSocketConnect();
        //    }
        //    else
        //    {
        //        GameMgr.Instance.GameReStart();
        //        onErrorCount = 0;
        //    }
        //}, GameConfig.ERROR_TITLE, error);
    }
    #endregion

    /// <summary>
    /// IO Event만 발송할 경우
    /// </summary>
    public bool SocketRequest(IOCommand commandId)
    {
        if (commandId == IOCommand.Syn)
        {
            var jwtByte = Encoding.UTF8.GetBytes(UserManager.Instance.UserInfo.token);
            var jwtLength = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(jwtByte.Length));
            var header = new WebSocketHeader((short)commandId, UserManager.Instance.UserInfo.uid, -1,
                1 + jwtLength.Length + jwtByte.Length, HeaderSize.None);

            int byteSize = WebSocketHeader.Size + sizeof(byte) + sizeof(int) + jwtByte.Length;

            var buffer = new byte[byteSize];
            var stream = new MemoryStream(buffer);
            stream.SetLength(0);

            header.WriteTo(stream);
            stream.SetLength(WebSocketHeader.Size - (int)HeaderSize.GameHeader);

            stream.Write(jwtLength);
            stream.Write(jwtByte);

            return SocketRequest(buffer, 0, (int)stream.Length);

        }
        else
        {
            var header = new WebSocketHeader((short)commandId, save_userUidForFin, -1, 0,
                HeaderSize.None);

            int byteSize = WebSocketHeader.Size;

            var buffer = new byte[byteSize];
            var stream = new MemoryStream(buffer);
            header.WriteTo(stream);

            if (sockectState != SocketState.CONNECT && sockectState != SocketState.READY) return false;
            return SocketRequest(buffer, 0, byteSize - (int)HeaderSize.GameHeader);
        }
    }

    /// <summary>
    /// GameServer Data 발송할 경우
    /// </summary>
    //public bool SocketRequest(ProtoMsgType messageId, byte[] payload)
    //{
    //    var payloadLength = payload.Length;

    //    var headerSize = WebSocketHeader.Size;
    //    var buffer = new byte[headerSize + payloadLength];
    //    var stream = new MemoryStream(buffer);
    //    new WebSocketHeader((short)IOCommand.UniCast, UserManager.Instance.UserInfo.uid, (int)messageId, payloadLength)
    //        .WriteTo(stream);
    //    stream.Write(payload);

    //    return SocketRequest(buffer, 0, buffer.Length);
    //}

    //public bool SocketRequest<T>(T data, ProtoMsgType messageId) where T : IMessage<T>, new()
    //{
    //    if (isFocusOut && GameMgr.Instance.gameState == GameState.Play)
    //        return false;

    //    //Debug.Log($"Send done {data.Descriptor.Name} - {data.ToString()}");
    //    return SocketRequest(messageId, ProtobufSerialize<T>(data));
    //}

    //public bool FoeceSocketRequest<T>(T data, ProtoMsgType messageId) where T : IMessage<T>, new()
    //{
    //    //Debug.Log($"Send done {data.Descriptor.Name} - {data.ToString()}");
    //    return SocketRequest(messageId, ProtobufSerialize<T>(data));
    //}

    private bool SocketRequest(byte[] data)
    {
        return SocketRequest(data, 0, data.Length);
    }

    private bool SocketRequest(byte[] data, int offset, int count)
    {
        if (data.Length == 0)
        {
            return false;
        }

        if (this.sockectState == SocketState.CONNECT)
        {
            if (sockectState != SocketState.CONNECT && sockectState != SocketState.READY) return false;
            this.webSocket.Send(data, (ulong)offset, (ulong)count);

            //  Debug.Log($"Send Size {data.Length} -- Total : {totalBytpe}");
            retryData = data;
        }
        else
        {
            reconnectData = data;
        }

        return this.webSocket.IsOpen;
    }

    // TODO(Diego) : 상황에 맞게 Reconnect 할 수 있도록 구조 변경. 유저정보구조 확정시 작업진행할것.
    // private void Reconnect(accoutRes res)
    // {
    //     OnReconnectEvent.ExecuteCallback(res);
    //     isDisconnected = false;

    //     if (!reconnectData.IsEmpty())
    //     {
    //         SocketRequest(reconnectData.KEY, reconnectData.PARAM);
    //         reconnectData.Clear();
    //     }
    // }


    //public byte[] ProtobufSerialize<T>(T data) where T : IMessage<T>
    //{
    //    if (data == null)
    //    {
    //        return Array.Empty<byte>();
    //    }

    //    return MessageExtensions.ToByteArray(data);
    //}

    //public T ProtobufDeserialize<T>(MemoryStream stream) where T : IMessage<T>, new()
    //{
    //    if (stream == null)
    //    {
    //        return default(T);
    //    }

    //    MessageParser<T> parser = (MessageParser<T>)deserializeParser[typeof(T).FullName];
    //    return parser.ParseFrom(stream);
    //}

    // #region 안쓰지만 있는 Diego의 유산
    public void RequestRetry()
    {
        if (false == (retryData.Length != 0))
        {
            SocketRequest(retryData);
            retryData = null;
        }
    }


    public void WebSocketDisconnect(Action callBack)
    {
        if (socketQueue != null && socketQueue.Count > 0)
        {
            socketQueue.Clear();
        }

        if (webSocket != null)
        {
            StopCoroutine("RecvTimeCheck");
        }

        disconnectAction = callBack;

        if (sockectState == SocketState.CONNECT)
        {
            SocketRequest(IOCommand.Fin);
            //NetworkManager.Instance.SetLoading(true);
        }
        else
        {
            sockectState = SocketState.DISCONNECT;
            if (webSocket != null)
            {
                webSocket.Close();
                webSocket = null;
            }
            disconnectAction?.Invoke();
            disconnectAction = null;
            //NetworkManager.Instance.SetLoading(false);
            onSocketConnectCallBack?.Invoke(false);
            onSocketConnectCallBack = null;
        }
    }


    public void OnApplicationQuit()
    {
        WebSocketDisconnect(() =>
        {
            if (socketQueue != null && socketQueue.Count > 0) socketQueue.Clear();
            Debug.Log("OnApplicationQuit");
        });
    }

    IEnumerator RecvTimeCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            var durationMilSec = (DateTime.UtcNow - checkReconnectTime).Seconds;
            if (durationMilSec is > 3 and < 10)
            {
                sockectState = SocketState.RECONNECTING;
                Debug.Log("RECONNECTING");
            }

            else if (durationMilSec is > 10)
            {
                Debug.Log("CONNECT ERROR");
                yield break;
            }
        }
    }
}
