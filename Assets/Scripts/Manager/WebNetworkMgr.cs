using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebNetworkMgr : SingletonComponentBase<WebNetworkMgr>
{
    protected override void InitializeSingleton() { }

    //TODO: 나중에 웹 연결 만들어야 함
    public void RequestToken()
    {
        UserInfo info = new();

        info.uid = 9;
        info.tid = 28728;
        info.gameId = "minnimoe";
        info.token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJtaW5uaW1vZSIsInRpZCI6Mjg3MjgsInVpZCI6OSwiaWF0IjoxNjY4NjY2MTcyLCJleHAiOjE2Njg3NTI1NzIsImF1ZCI6ImRldi50b3VybmFtZW50LnBsYXlkYXBwLmNvbSIsImlzcyI6InBsYXlkYXBwLmNvbSJ9.o9A98im67vkk0f-r0QcDOHSWT1CxNUhx9XQAZqUyYbs";

        UserManager.Instance.OnUser(info);
    }
}
