//using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class UserInfo
{
    public long uid;
    public string tid;
    public string gameId;
    public string token;
}

public class TargetGame
{
    public string entryCurrency;
    public int entryCharge;
}

public class UserManager : SingletonComponentBase<UserManager>
{
    //public Dictionary<long, TrainData> UserDataDic = new();
    public UserInfo UserInfo = new();
    //public TrainData MyUserData = new();
    public Dictionary<long, int> userDataCache = new();

    public bool isUserDataDequeue = true;
    float delay = 0.0f;
    public bool isPlay = false;

    protected override void InitializeSingleton()
    {

    }
    
    //public void UpdateS2C_UserData(in S2C_UserData data)
    //{
    //    Dictionary<long, TrainData> s2c_UserDataDic = new();
    //    foreach (var item in data.UserDataList)
    //    {
    //        s2c_UserDataDic.Add(item.Id, item);
    //    }

    //    List<long> sendList = new List<long>();

    //    // 새로운 유저 데이터와 기존의 유저 데이터를 비교함
    //    foreach (var user in data.UserDataList)
    //    {
    //        // 새로운 Uid가 있는지 검사
    //        if (!userDataCache.ContainsKey(user.Id))
    //        {
    //            if (UserInfo.uid != user.Id)
    //            {
    //                sendList.Add(user.Id);
    //                userDataCache.Add(user.Id, 0);
    //            }
    //        }
    //    }

    //    if (sendList.Count > 0)
    //        SocketClient.Instance.Send_TargetUser(sendList);

    //    // 기존의 데이터와 새로운 유저 데이터를 비교함
    //    foreach (var user in TrainMgr.Instance.TrainDictionary)
    //    {
    //        // 지워진 Uid가 있으면 지워줌
    //        if (!s2c_UserDataDic.ContainsKey(user.Key))
    //        {
    //            userDataCache.Remove(user.Key);
    //            if (TrainMgr.Instance.TrainDictionary.ContainsKey(user.Key))
    //            {
    //                TrainMgr.Instance.ReleaseActor(user.Key);
    //            }

    //            continue;
    //        }
    //        {
    //            TrainData trainData;
    //            if (s2c_UserDataDic.TryGetValue(user.Key, out trainData))
    //            {
    //                user.Value.UpdateServerTrainData(trainData);
    //            }
    //        }
    //        user.Value.UpdateTrainData(s2c_UserDataDic[user.Key]);
    //    }

    //    // 지울 데이터 담아둔 Queue 일괄 Release
    //    if (0 < TrainMgr.Instance.removeWaitHashSet.Count)
    //    {
    //        TrainMgr.Instance.WaitRelease();
    //    }

    //    //새로 들어온 유저랑 클라 저장 data 비교해서 클라엔 있는데 새로 온 data에 없으면 클라에서 제거
    //    List<long> removeIds = new List<long>();
    //    foreach (var user in UserDataDic)
    //    {
    //        if (!s2c_UserDataDic.ContainsKey(user.Key))
    //        {
    //            removeIds.Add(user.Key);
    //        }
    //    }

    //    if (removeIds.Count > 0)
    //    {
    //        for (int i = 0; i < removeIds.Count; ++i)
    //        {
    //            UserDataDic.Remove(removeIds[i]);
    //        }
    //    }
    //    /*
    //    foreach (var user in s2c_UserDataDic)
    //    {
    //        //UserDataDic[user.Key] = user.Value;

    //        //새로 들어온 data와 클라 저장 data에 같은 key가 있음 -> 아직 화면에 있는 상태
    //        if (UserDataDic.ContainsKey(user.Key))
    //        {
    //            UserDataDic[user.Key].ColorIndex = user.Value.ColorIndex;
    //            UserDataDic[user.Key].Id = user.Value.Id;
    //            UserDataDic[user.Key].CharacterId = user.Value.CharacterId;
    //            UserDataDic[user.Key].Direction = user.Value.Direction;
    //            UserDataDic[user.Key].Stamina = user.Value.Stamina;
    //            UserDataDic[user.Key].MagnetSize = user.Value.MagnetSize;
    //            UserDataDic[user.Key].MoveSpeed = user.Value.MoveSpeed;
    //            UserDataDic[user.Key].TotalExp = user.Value.TotalExp;
    //            UserDataDic[user.Key].Status = user.Value.Status;
    //            UserDataDic[user.Key].PositionList.Add(user.Value.PositionList);
    //        }
    //        else UserDataDic.Add(user.Key, user.Value);
    //    }
    //    */

    //}

    /*
    public void SyncUser(TargetTrainData data)
    {
        if (GameMgr.Instance.HeadDictionary.TryGetValue(data.Id, out var head))
        {
            head.SyncMovement(data);

            if (data.Status != BodyStatus.Invincible)
            {
                head.RefreshTainColor();
            }
            else if(data.Status == BodyStatus.Invincible)
            {
                head.ReStartInvincibilityEffect();
            }
        }
    }
    */
    public void AllRelease()
    {
        //UserDataDic.Clear();
        userDataCache.Clear();
        UserInfo = new();
        //MyUserData = new();
    }
}
