using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Transports;
using Newtonsoft.Json;

public class NetworkMgr : SingletonComponentBase<NetworkMgr>
{
    private string address = "";

    private SocketManager serverManager;
    private Socket serverSocket;

    private Action connetCallBack;

    protected override void InitializeSingleton() 
    {
#if !UNITY_EDITOR && UNITY_WEBGL
#if USE_WEBGL_LOCAL_CASS
        address = "ws://172.17.176.1:4001/spacecrush/socket.io"; //본점
#elif USE_WEBGL_DEV
        address = "https://dev-tournament.playdapp.com/spacecrush/socket.io";
#elif USE_WEBGL_QA
        address = "https://qa-tournament.playdapp.com//spacecrush/socket.io";
#elif USE_WEBGL_PROD
        address = "https://prod-tournament.playdapp.com//spacecrush/socket.io";
#endif
#elif UNITY_EDITOR
        address = "ws://127.0.0.1:4001/spacecrush/socket.io"; //본점
#endif
    }

    public void OnConnect(Action _connectCallBack)
    {
        Debug.Log("Connecting...");
        SocketOptions options = new SocketOptions();

        options.ConnectWith = TransportTypes.WebSocket;
        options.AutoConnect = false;

        options.Auth = (serverManager, serverSocket) => new { uid = UserManager.Instance.userInfo.uid, appVersion = Application.version, appName = Application.productName, token = UserManager.Instance.userInfo.token};

        serverManager = new SocketManager(new Uri(address), options);
        serverSocket = serverManager.GetSocket("/");

        InitCallBack();

        connetCallBack = _connectCallBack;

        serverManager.Open();
    }

    private void InitCallBack()
    {
        serverManager.Socket.On(SocketIOEventTypes.Connect, () =>
        {
            Debug.Log("Connected!");
            connetCallBack.Invoke();
        });

        serverManager.Socket.On(SocketIOEventTypes.Disconnect, () =>
        {
            Debug.Log("Disconnected!");
        });

        // The argument will be an Error object.
        serverManager.Socket.On<Error>(SocketIOEventTypes.Error, (resp) =>
        {
            Debug.Log($"Error msg: {resp.message}");
        });
    }

    #region Start Communication
    public void RequestStartGame(GameStartReqDto data)
    {
        Send<string>("StartGame", data, ResponseStartGame);
    }
    private void ResponseStartGame(string res)
    {
        Debug.Log($"[Recv : StartGame] => {res}");

        WaveMgr.Instance.Initilize(res);
        GameMgr.Instance.GameLogic.GameStart();
    }
    #endregion

    #region End Communication
    public void RequestEndGame(GameEndReqDto data)
    {
        Send<string>("EndGame", data, ResponseEndGame);
    }
    private void ResponseEndGame(string res)
    {
        Debug.Log($"[Recv : EndGame] => {res}");
        GameEndResDto endRes = JsonUtility.FromJson<GameEndResDto>(res);

#if !UNITY_EDITOR && UNITY_WEBGL
        WebNetworkMgr.SendEndGame(endRes.score);
#endif

        if (endRes.result)
        {
            GameMgr.Instance.GameOver();

            serverSocket.Disconnect();
            serverManager.Close();

            serverSocket = null;
            serverManager = null;
        }
    }
#endregion

    private void Send(string message, BaseReqDto data)
    {
        UserInfo user = UserManager.Instance.userInfo;

        data.uid = user.uid;
        data.tid = user.tid;
        data.gameId = user.gameId;
        data.pid = user.pid;
        data.token = user.token;

        var jsonData = JsonConvert.SerializeObject(data);

        Debug.Log($"[Send : {message}] => " + jsonData);
        serverSocket.Emit(message, jsonData);
    }
    private void Send<T>(string message, BaseReqDto data, Action<T> callBack)
    {
        UserInfo user = UserManager.Instance.userInfo;

        data.uid = user.uid;
        data.tid = user.tid;
        data.gameId = user.gameId;
        data.pid = user.pid;
        data.token = user.token;

        var jsonData = JsonConvert.SerializeObject(data);

        Debug.Log($"[Send : {message}] => " + jsonData);
        serverSocket.EmitCallBack(callBack, message, jsonData);
    }
}
