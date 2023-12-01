using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    [Header("UI")]
    public GameObject titleUi;
    public GameObject gameUi;
    public Text ScoreText;

    [Header("Fever Event")]
    public bool isFeverTime = false;
    public PlayerController playerPrefab;
    public BackgroundController bgController;

    [Header("WaveSpawn")]
    public List<WaveObj> currentWaveList = new List<WaveObj>();
    public Stack<WaveObj> RemoveWaveStack = new Stack<WaveObj>();
    public int MaxSpawnWave;

    [Header("Wave Move")]
    public float originDownSpeed;
    public float currentDownSpeed;
    public float interverValue;
    public Action waveUpEvent;

    private void Awake()
    {
        currentDownSpeed = originDownSpeed;

        waveUpEvent = WaveUp;
    }

    private void FixedUpdate()
    {
        ScoreText.text = GameMgr.Instance.GameScore.ToString();

        //wave 생성
        if (currentWaveList.Count < MaxSpawnWave && GameMgr.Instance.gameState == GameState.Game)
        {
            currentWaveList.Add(WaveMgr.Instance.GenerateWave());
            currentWaveList[currentWaveList.Count-1].transform.position = (Vector2.up * WaveMgr.Instance.oneblockSize * GameConfig.FILED_HEIGHT_SIZE) * currentWaveList.Count;
        }

        //wave 이동
        if (currentWaveList.Count > 0)
        {
            foreach (var wave in currentWaveList)
            {
                wave.rigidbody2d.MovePosition((Vector2)wave.transform.position + (Vector2.down * currentDownSpeed * Time.fixedDeltaTime));

                if(wave.transform.position.y < -10f)
                {
                    RemoveWaveStack.Push(wave);
                }
            }
        }

        //wave 삭제
        for (int i = 0; i < RemoveWaveStack.Count; i++)
        {
            WaveObj RemoveWave = RemoveWaveStack.Pop();

            currentWaveList.Remove(RemoveWave);
            Destroy(RemoveWave.gameObject);
        }
    }

    public void GameStart()
    {
        titleUi.SetActive(false);
        gameUi.SetActive(true);

        GameMgr.Instance.GameStart(Instantiate(playerPrefab));
    }

    public void WaveUp()
    {
        foreach (var wave in currentWaveList)
        {
            float upPosY = wave.transform.position.y + interverValue;
            wave.transform.position = new Vector2(0, upPosY);
        }
    }

    public void FeverStart()
    {
        isFeverTime = true;

        bgController.BgFeverStart();
        currentDownSpeed = originDownSpeed + GameConfig.FEVER_UP;
        GameMgr.Instance.Player.feverTime = GameConfig.FEVER_TIME;
    }
}
