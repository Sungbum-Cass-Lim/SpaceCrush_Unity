using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo
{
    public int uid = 0;
    public int tid = 0;
    public string gameId = "";
    public string token = "";
    public string pid = "";
}

public class UserManager : SingletonComponentBase<UserManager>
{
    public UserInfo userInfo;

    protected override void InitializeSingleton(){}

    public void OnUser(UserInfo user)
    {
        userInfo = user;   
    }
}
