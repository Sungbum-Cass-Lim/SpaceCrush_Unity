using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public GameObject titleUi;
    public GameObject gameUi;

    public Transform CurrentWave;

    public PlayerController playerPrefab;

    private void Update()
    {
        if(CurrentWave != null)
        {
            //TODO: 속도 조절 값 필요
            CurrentWave.position += Vector3.down * Time.deltaTime * 4;

            if(CurrentWave.position.y <= -10.0f)
            {
                WaveMgr.Instance.ResetWave(CurrentWave);

                CurrentWave = WaveMgr.Instance.GenerateWave();
            }
        }
    }

    public void GameStart()
    {
        titleUi.SetActive(false);
        gameUi.SetActive(true);

        CurrentWave = WaveMgr.Instance.GenerateWave();
        GameMgr.Instance.GameStart(Instantiate(playerPrefab));
    }
}
