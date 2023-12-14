using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WebNetworkMgr : SingletonComponentBase<WebNetworkMgr>
{
    [DllImport("__Internal")]
    private static extern void initialize(string zone);

    [DllImport("__Internal")]
    private static extern void RequestToken();

    [DllImport("__Internal")]
    private static extern void RequestTarget();

    [DllImport("__Internal")]
    public static extern void Loading(bool show);

    [DllImport("__Internal")]
    public static extern void RequestMain();

    [DllImport("__Internal")]
    public static extern void SendFrontError(string data);
    [DllImport("__Internal")]
    public static extern void SendEndGame(int _score);


    public Action<bool> onRequestTokenCallBack = null; //프론트 통신용 콜백
    public Action onRequestTargetCallBack = null; //프론트 통신용 콜백

    protected override void InitializeSingleton() { }

    public void InitNetwork(Action<bool> RequestTokenCallBack)
    {
        this.onRequestTokenCallBack = RequestTokenCallBack;

#if !UNITY_EDITOR && UNITY_WEBGL
#if USE_WEBGL_STAGE || USE_WEBGL_PROD || USE_WEBGL_QA || USE_WEBGL_DEV
            RequestToken();
#elif USE_WEBGL_LOCAL_CASS
            string tempData = "{ \"uid\": 498687, \"tid\": 28728, \"gameId\": \"minnimoe\", \"token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtaW5uaW1vZSIsInRpZCI6Mjg3MjgsInVpZCI6NDk4Njg3LCJpYXQiOjE2Njg2NjYxNzIsImV4cCI6MTY2ODc1MjU3MiwiYXVkIjoiZGV2LnRvdXJuYW1lbnQucGxheWRhcHAuY29tIiwiaXNzIjoicGxheWRhcHAuY29tIn0.6l5sewAc06ctZnO8Qo7CoQwK_xdqT-3gc10epeTbmXo\"}";
            OnRequestToken(tempData);
#endif
#elif UNITY_EDITOR

        //테스트 맥북
        //string tempData = "{ \"uid\": 4294967280, \"tid\": 89765, \"gameId\": \"monstertrain\", \"token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtb25zdGVydHJhaW4iLCJ1aWQiOiI0Mjk0OTY3MjgwIiwidGlkIjoiODk3NjUiLCJqdGkiOiIzZDg3ZTg2Mi1jZmI5LTQ0MWMtOGJjZC0xMjU3YWU3NTI2YmYiLCJleHAiOjE3NDU0NjI1NDIsImlhdCI6MTY4MjMwNDE0MiwiaXNzIjoicGxheWRhcHAuY29tIiwiYXVkIjoiZGV2LnRvdXJuYW1lbnQucGxheWRhcHAuY29tIn0.2Bq1oP0H5izkviq88IEFwN1a5RtpEhwNAsy5kKq0pck\"}";
        //레인
        //string tempData = "{ \"uid\": 4294967040, \"tid\": 89765, \"gameId\": \"monstertrain\", \"token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtb25zdGVydHJhaW4iLCJ1aWQiOiI0Mjk0OTY3MDQwIiwidGlkIjoiODk3NjUiLCJqdGkiOiI2NjY2OTVhYS0wMjI2LTQzZTYtOWRmOC1iZmM4MzEwZDMzOGEiLCJleHAiOjE3NDU0NjI3MzQsImlhdCI6MTY4MjMwNDMzNCwiaXNzIjoicGxheWRhcHAuY29tIiwiYXVkIjoiZGV2LnRvdXJuYW1lbnQucGxheWRhcHAuY29tIn0.hCUtiXY7zEV1DiBZ45pZbRVUcEluGTL02yH0Cu0o4XI\"}";

        //남는거
        //string tempData = "{ \"uid\": 4294963200, \"tid\": 89765, \"gameId\": \"monstertrain\", \"token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtb25zdGVydHJhaW4iLCJ1aWQiOiI0Mjk0OTYzMjAwIiwidGlkIjoiODk3NjUiLCJqdGkiOiI5MzYzZjJlNS03ZjFmLTQzNWMtYmI4OC01YTQ0NmIwOTY2NWQiLCJleHAiOjE3NDU0NjI3NTksImlhdCI6MTY4MjMwNDM1OSwiaXNzIjoicGxheWRhcHAuY29tIiwiYXVkIjoiZGV2LnRvdXJuYW1lbnQucGxheWRhcHAuY29tIn0.XYsY5fL7_1JOp9PlfEBaEAtJBptv3UqrVJg4ldO80Ok\"}";

        //카스
        string tempData = "{ \"uid\": 9, \"tid\": 28728, \"gameId\": \"minnimoe\", \"token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtaW5uaW1vZSIsInRpZCI6Mjg3MjgsInVpZCI6OSwiaWF0IjoxNjY4NjY2MTcyLCJleHAiOjE2Njg3NTI1NzIsImF1ZCI6ImRldi50b3VybmFtZW50LnBsYXlkYXBwLmNvbSIsImlzcyI6InBsYXlkYXBwLmNvbSJ9.o9A98im67vkk0f-r0QcDOHSWT1CxNUhx9XQAZqUyYbs\"}";

        //ADS2
        //string tempData = "{ \"uid\": 4008636142, \"tid\": 89765, \"gameId\": \"monstertrain\", \"token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtb25zdGVydHJhaW4iLCJ1aWQiOiI0MDA4NjM2MTQyIiwidGlkIjoiNDk1IiwianRpIjoiMzY1NzZkMjMtNjFiZC00ZWFkLWFlNzAtY2E2YzlmY2VjZDY1IiwiZXhwIjoxODQ0NzU2NDUyLCJpYXQiOjE2ODY5MDM2NTIsImlzcyI6InBsYXlkYXBwLmNvbSIsImF1ZCI6ImRldi50b3VybmFtZW50LnBsYXlkYXBwLmNvbSJ9.7981EMwwvzZSCZf1gqFJjVGII3xIWofdrEoq5QYYDE8\"}";
        //싼
        //string tempData = "{ \"uid\": 4294901778, \"tid\": 1222, \"gameId\": \"monstertrain\", \"token\": \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtb25zdGVydHJhaW4iLCJ1aWQiOiI0Mjk0OTAxNzc4IiwidGlkIjoiMTIyMiIsImp0aSI6IjhhOThhZTc5LTNhYWUtNDk3NS05YzdmLTFhZmY3NWE5NTBhYSIsImV4cCI6MTg0MjA3MDYwNSwiaWF0IjoxNjg0MjE3ODA1LCJpc3MiOiJwbGF5ZGFwcC5jb20iLCJhdWQiOiJkZXYudG91cm5hbWVudC5wbGF5ZGFwcC5jb20ifQ.Hr-zSgi5NA1p2ORxEysw6YpLEV0cO9xkXCjs-wSPH4Y\"}";
        OnRequestToken(tempData);
#endif
    }

    public void InitTargetGame(Action RequestTargetCallBack)
    {
        this.onRequestTargetCallBack = RequestTargetCallBack;
#if !UNITY_EDITOR
#if USE_WEBGL_DEV
                         initialize("dev");
#elif USE_WEBGL_QA
                         initialize("qa");
#elif USE_WEBGL_STAGE || USE_WEBGL_PROD
                         initialize("prod");
#elif USE_WEBGL_LOCAL_CASS
                    onRequestTargetCallBack.Invoke();
#endif
        RequestTarget();
#else
        onRequestTargetCallBack.Invoke();
#endif

    }

    #region Web Interection
    public void SetLoading(bool show)
    {
#if !UNITY_EDITOR && UNITY_WEBGL && !USE_WEBGL_LOCAL_CASS
        Loading(show);
#endif
    }

    public void GoToMain()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        RequestMain();
#endif
    }

    public void SendError(string data)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Debug.Log("Enter SendFrontError :: " + data);
            SendFrontError(data);
#endif
    }
    #endregion

    public void OnRequestToken(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            GoToMain();
            return;
        }
        try
        {
            Debug.Log(data);
            Debug.Log(JsonUtility.FromJson<UserInfo>(data).uid);

            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(data);

            if (userInfo.token != null)
            {
                UserManager.Instance.OnUser(userInfo);
            }
            else
            {
                GoToMain();
            }

            if (UserManager.Instance.userInfo == null || string.IsNullOrEmpty(UserManager.Instance.userInfo.uid.ToString()))
            {
                onRequestTokenCallBack?.Invoke(false);
                return;
            }
            onRequestTokenCallBack?.Invoke(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"OnRequestToken error msg  {e}");
            onRequestTokenCallBack?.Invoke(false);
        }
    }

    public void OnRequestTarget(string data)
    {
        onRequestTargetCallBack.Invoke();
    }

    public void OnRequestRestart()
    {
        SceneManager.LoadScene(0);
    }

    //private void OnApplicationQuit()
    //{
    //    if (null != SocketClient.Instance)
    //        SocketClient.Instance.Dispose();
    //}
}
