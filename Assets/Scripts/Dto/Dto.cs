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

public class GameStartReqDto : BaseReqDto { }

#region GameStartResDto
public class ObstaclesData
{
    public float[][] waveList;
    public int[][] obstaclesBoxList;
}
public class BoxData
{
    public int playerLife;
    public int[][] lifeList;
    public int[][] boxList;
    public ObstaclesData obstaclesData;
}
public class GameStartResDto : BaseResDto
{
    public int bestScore;
    public string pid;
    public BoxData boxData;
}
#endregion

public class GameEndReqDto : BaseReqDto
{
    public int score;
    public int[][] totalCollisionData;
}
public class GameEndResDto : BaseResDto 
{
    public int score;
}

