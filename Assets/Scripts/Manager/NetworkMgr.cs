using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Transports;
using BestHTTP.Authentication;

public class NetworkMgr : SingletonComponentBase<NetworkMgr>
{
    private const string address = "ws://127.0.0.1:4001/spacecrush/socket.io";

    private SocketManager serverManager;
    private Socket serverSocket;

    private Action connetCallBack;
    protected override void InitializeSingleton(){}

    public void OnConnect(Action _connectCallBack)
    {
        Debug.Log("Connecting...");
        SocketOptions options = new SocketOptions();

        options.ConnectWith = TransportTypes.WebSocket;
        options.AutoConnect = false;

        options.Auth = (serverManager, serverSocket) => new { uid = 9, appVersion = "1.0.7", appName = "SpaceCrush", token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtaW5uaW1vZSIsInRpZCI6Mjg3MjgsInVpZCI6OSwiaWF0IjoxNjY4NjY2MTcyLCJleHAiOjE2Njg3NTI1NzIsImF1ZCI6ImRldi50b3VybmFtZW50LnBsYXlkYXBwLmNvbSIsImlzcyI6InBsYXlkYXBwLmNvbSJ9.o9A98im67vkk0f-r0QcDOHSWT1CxNUhx9XQAZqUyYbs" };

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

    public void RequestStartGame(GameStartReqDto data)
    {
        Send<string>("StartGame", data, ResponseStartGame);
    }
    public void ResponseStartGame(string res)
    {
        WaveMgr.Instance.Initilize(res);
        GameMgr.Instance.GameLogic.GameStart();
    }

    public void RequestEndGame(GameEndReqDto data)
    {
        //Send("EndGame", data);
    }
    public void ResponseEndGame(string res)
    {
        GameEndResDto endRes = JsonUtility.FromJson<GameEndResDto>(res);
    }

    private void Send<T>(string message, BaseReqDto data, Action<T>? callBack)
    {
        UserInfo user = UserManager.Instance.userInfo;

        data.uid = user.uid;
        data.tid = user.tid;
        data.gameId = user.gameId;
        data.pid = user.pid;
        data.token = user.token;

        var jsonData = JsonUtility.ToJson(data);

        Debug.Log($"[Send : {message}] => " + jsonData);

        if (callBack != null)
            serverSocket.EmitCallBack(callBack, message, jsonData);
        else
            serverSocket.Emit(message, jsonData);
    }
}
