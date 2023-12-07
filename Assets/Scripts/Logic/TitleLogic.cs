using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogic : MonoBehaviour
{
    private void Awake()
    {
        WebNetworkMgr.Instance.RequestToken();
    }

    public void GameStart()
    {
        NetworkMgr.Instance.OnConnect(GetGameData);
    }

    private void GetGameData()
    {
        GameStartReqDto gameStartReqDto = new();

        NetworkMgr.Instance.RequestStartGame(gameStartReqDto);
    }
}
