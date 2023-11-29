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

    public WaveObj currentWave;

    public PlayerController playerPrefab;

    public float interverValue = 0.1f;
    public Action waveUpEvent;

    private void Awake()
    {
        waveUpEvent = WaveUp;
    }

    private void FixedUpdate()
    {
        if(currentWave != null)
        {
            currentWave.rigidbody2d.MovePosition((Vector2)currentWave.transform.position + (Vector2.down * 7.5f * Time.fixedDeltaTime));

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
}
