using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public GameObject titleUi;
    public GameObject gameUi;
    public Text ScoreText;

    public bool isFeverTime = false;

    public PlayerController playerPrefab;

    public BackgroundController bgController;

    public WaveObj currentWave;
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

        if (currentWave != null)
        {
            currentWave.rigidbody2d.MovePosition((Vector2)currentWave.transform.position + (Vector2.down * currentDownSpeed * Time.fixedDeltaTime));

            if (currentWave.transform.position.y <= -10.0f)
            {
                WaveMgr.Instance.ResetWave(currentWave);

                currentWave = WaveMgr.Instance.GenerateWave();
            }
        }
    }

    public void GameStart()
    {
        titleUi.SetActive(false);
        gameUi.SetActive(true);

        currentWave = WaveMgr.Instance.GenerateWave();
        GameMgr.Instance.GameStart(Instantiate(playerPrefab));
    }

    public void WaveUp()
    {
        float upPosY = currentWave.transform.position.y + interverValue;

        currentWave.transform.position = new Vector2(0, upPosY);
    }

    public void FeverStart()
    {
        isFeverTime = true;

        bgController.BgFeverStart();
        currentDownSpeed = originDownSpeed + GameConfig.FEVER_UP;
        GameMgr.Instance.Player.feverTime = GameConfig.FEVER_TIME;
    }
}
