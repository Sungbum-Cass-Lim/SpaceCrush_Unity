using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseReqDto
{
    public int uid;
    public int tid;
    public string gameId;
    public string pid;
    public string token;
}

public class BaseResDto
{
    public bool result = true;
}

public class GameCollisionReqDto : BaseReqDto
{
    private int[] scoreData;
    public int score;
}

public class GameEndReqDto : BaseReqDto
{
    public int score;
    public int[] totalCollisionData;
}

public class GameStartReqDto : BaseReqDto { }
