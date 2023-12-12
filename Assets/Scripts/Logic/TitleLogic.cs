using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleLogic : MonoBehaviour
{
    public Text versionText;
    public Text gameStartText;

    private Color startTextColor;

    private Coroutine startTextColorCorutine;

    private void Awake()
    {
        versionText.text = $"v.{Application.version}";

        startTextColor = gameStartText.color;
        startTextColorCorutine = StartCoroutine(StartTextColor());
    }

    IEnumerator StartTextColor()
    {
        yield return null;

        bool isAdd = false;
        Color copyColor = startTextColor;

        while (true)
        {
            if (isAdd == false)
            {
                copyColor.a -= Time.deltaTime;

                if (copyColor.a <= 0)
                    isAdd = true;
            }
            else if(isAdd == true)
            {
                copyColor.a += Time.deltaTime;

                if (copyColor.a >= 1)
                    isAdd = false;
            }

            gameStartText.color = copyColor;
            yield return null;
        }
    }

    public void OnStartButton()
    {
        StopCoroutine(startTextColorCorutine);
        gameStartText.color = startTextColor;

        WebNetworkMgr.Instance.InitNetwork(OnToken);
    }

    public void OnToken(bool result)
    {
        if (result)
            NetworkMgr.Instance.OnConnect(GetGameData);

        else
            Debug.LogError("Token Is Null");
    }

    private void GetGameData()
    {
        GameStartReqDto gameStartReqDto = new();
        NetworkMgr.Instance.RequestStartGame(gameStartReqDto);
    }
}
