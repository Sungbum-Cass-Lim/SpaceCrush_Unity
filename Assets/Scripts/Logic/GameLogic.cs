using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameLogic : MonoBehaviour
{
    [Header("UI")]
    public GameObject titleUi;
    public GameObject gameUi;
    public Text ScoreText;

    [Header("Fever Event")]
    public bool isFever = false;
    public float feverTime = 0.0f;
    public BackgroundController bgController;

    [Header("WaveSpawn")]
    public List<WaveObj> currentWaveList = new List<WaveObj>();
    public Stack<WaveObj> RemoveWaveStack = new Stack<WaveObj>();
    public int MaxSpawnWave;

    [Header("Wave Move")]
    public float originDownSpeed;
    public float currentDownSpeed;
    public float interverValue;
    public int waveUpCount;

    private void Awake()
    {
        currentDownSpeed = originDownSpeed;
    }

    private void FixedUpdate()
    {
        ScoreText.text = GameMgr.Instance.GameScore.ToString();

        //wave 积己
        if (currentWaveList.Count < MaxSpawnWave && GameMgr.Instance.gameState == GameState.Game)
        {
            Vector2 beforeWavePos = currentWaveList.Count > 0 ? currentWaveList[currentWaveList.Count - 1].transform.position : Vector2.up;

            currentWaveList.Add(WaveMgr.Instance.GenerateWave());
            currentWaveList[currentWaveList.Count-1].transform.position = beforeWavePos + (Vector2.up * WaveMgr.Instance.oneblockSize * GameConfig.FILED_HEIGHT_SIZE * 1.4f);
        }

        //wave 捞悼
        if (currentWaveList.Count > 0)
        {
            foreach (var wave in currentWaveList)
            {
                wave.rigidbody2d.MovePosition((Vector2)wave.transform.position + (Vector2.down * currentDownSpeed * Time.fixedDeltaTime));

                if (wave.transform.position.y < -10f)
                {
                    RemoveWaveStack.Push(wave);
                }
            }
        }

        //wave Up
        if (currentWaveList.Count > 0 && waveUpCount > 0)
        {
            WaveUp();
            waveUpCount = 0;
        }

        //wave 昏力
        for (int i = 0; i < RemoveWaveStack.Count; i++)
        {
            WaveObj RemoveWave = RemoveWaveStack.Pop();

            RemoveWave.WaveRelease();
            currentWaveList.Remove(RemoveWave);
        }

        //fever 包府
        if(isFever == true)
        {
            feverTime -= Time.fixedDeltaTime;

            if (feverTime < 0)
            {
                FeverEnd();
            }
        }
    }

    public void GameStart()
    {
        titleUi.SetActive(false);
        gameUi.SetActive(true);

        GameMgr.Instance.GameStart(ObjectPoolMgr.Instance.Load<PlayerController>(PoolObjectType.Player, "Player"));
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
        isFever = true;

        feverTime = GameConfig.FEVER_TIME;
        currentDownSpeed = originDownSpeed + GameConfig.FEVER_UP * 2;
        bgController.BgFeverStart();
    }

    public void FeverEnd()
    {
        isFever = false;

        feverTime = 0;
        currentDownSpeed = originDownSpeed;
        bgController.BgFeverEnd();
    }
}
