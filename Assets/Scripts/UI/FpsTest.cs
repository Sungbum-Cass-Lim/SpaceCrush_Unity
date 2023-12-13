using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsTest : MonoBehaviour
{
    public Text fpsTxt;
    float deltaTime = 0.0f;

    private int lowFps1;
    private int lowFps2;
    private int lowFps3;

    // Start is called before the first frame update
    void Start()
    {

#if USE_WEBGL_STAGE || USE_WEBGL_PROD
        fpsTxt.gameObject.SetActive(false);
#else
        ShowField();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        ShowFps();
    }

    private void ShowField()
    {
        lowFps1 = 0;
        lowFps2 = 0;
        lowFps3 = 0;
    }

    //테스트용
    private void ShowFps()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float dt1 = deltaTime;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        float dt2 = dt1 - Time.unscaledDeltaTime;
        //string text = $"{msec:F1}ms || {fps:F1} fps || ping :: {SocketClient.Instance.pingCount}";

        if ((int)fps < 59 && (int)fps > 55)
        {
            lowFps1++;
        }

        if ((int)fps <= 55 && (int)fps > 50)
        {
            lowFps2++;
        }

        if ((int)fps <= 50)
        {
            lowFps3++;
        }

        string text = $"{fps:F1} fps\n59 > count : {lowFps1}\n55>= count : {lowFps2}\n50 >= count : {lowFps3}";
        fpsTxt.text = text;

    }
}
