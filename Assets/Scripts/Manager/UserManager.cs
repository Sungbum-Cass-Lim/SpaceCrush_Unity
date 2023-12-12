using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public UserInfo userInfo { get; private set; } = null;

    protected override void InitializeSingleton(){}

    public void OnUser(UserInfo user)
    {
        userInfo = user;   
    }
}
