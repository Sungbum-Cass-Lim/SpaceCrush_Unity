using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLogic : MonoBehaviour
{
    [Header("Title Object")]
    public GameObject game;
    public GameObject gameUI;

    [Header("Title Sound Clip")]
    public AudioClip snd_Bgm;
    public AudioClip snd_bgm_Game_Over;

    [Header("Title State")]
    public bool isConnect = false;
    public bool isGameOver = false;
    public bool canGameStart = false;
    public bool isClickEnter = false;
    public bool isErrorDisconnet = false;
    public bool canClick = true;

    public static Action onConnectionUnSub = null;
    public static Action onDisConnectionUnSub = null;
    public static Action onErrorUnSub = null;

    private void Awake()
    {
        onConnectionUnSub = () =>
        {

        };
        onDisConnectionUnSub = () =>
        {

        };
        onErrorUnSub = () =>
        {

        };

        WebNetworkMgr.Instance.RequestToken();
    }

    public void GameStart()
    {
        NetworkMgr.Instance.OnConnect(GetGameData);
    }

    private void GetGameData()
    {
        GameStartReqDto gameStartReqDto = new();

        NetworkMgr.Instance.SendStartGame(gameStartReqDto);
    }
}
