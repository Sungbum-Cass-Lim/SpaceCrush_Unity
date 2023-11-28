using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObstaclesData
{
    public float[][] waveList;
    public int[][] obstaclesBoxList;
}
public struct BoxData
{
    public int playerLife;
    public int[][] lifeList;
    public int[][] boxList;
    public ObstaclesData obstaclesData;
}

[System.Serializable]
public struct WaveInfo
{
    public int bestScore;
    public string pid;
    public BoxData boxData;
    public bool result;
}

public class WaveMgr : SingletonComponentBase<WaveMgr>
{
    public const int widhtCount = 5;
    public const int heightCount = 7;

    #region WaveInfo
    public TextAsset waveJsonData;
    WaveInfo waveInfos;

    //한 번 받고 수정 할 일 없음
    private BoxData blockData;
    public BoxData BlockData { get { return blockData; } }

    private ObstaclesData obstaclesData;
    public ObstaclesData ObstaclesData { get { return obstaclesData; } }
    #endregion

    #region Prefab
    public BlockObj BlockPrefab;
    public LifeObj LifePrefab;
    public WallObj WallPrefab;
    #endregion

    public int[,] field = new int[heightCount, widhtCount]
    {
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0 },
    };

    public float oneblockSize;
    public Vector2 widhtBorder;
    public Vector2 heightBorder;

    //이 부분 스택으로 바꾸거나 더 좋은 방법 없을지 고민..
    private int currentWaveIdx = 0;
    private int currentblockIdx = 0;
    private int currentLifeIdx = 0;
    private int currentObstacleIdx = 0;

    protected override void InitializeSingleton()
    {
        waveInfos = JsonHelper.ReadJson<WaveInfo>(waveJsonData);
        Debug.Log("Load Wave Data");

        blockData = waveInfos.boxData;
        obstaclesData = blockData.obstaclesData;

        //blockSize와 여백 구하는 공식(ScreenWidth / blockImageWidth / fieldWidthCount + 여백)
        oneblockSize = (Screen.width / 114f / widhtCount) + 0.2f;

        //field Width, Height 최저값/최대값 구하는 공식
        widhtBorder = new Vector2(widhtCount / 2 * -oneblockSize, widhtCount / 2 * oneblockSize);
        heightBorder = new Vector2(heightCount / 2 * -oneblockSize, heightCount / 2 * oneblockSize);
    }

    public Transform GenerateWave()
    {
        Transform wave = new GameObject().transform;
        wave.name = "Wave";
        wave.SetParent(this.transform);

        //Block 한 줄 생성
        for (int i = 0; i < widhtCount; i++)
        {
            field[0, i] = 1;

            Vector2 SpawnPos = new Vector2(widhtBorder.x + oneblockSize * i, heightBorder.y + Random.Range(-0.2f, 0.2f));
            int blockLife = BlockData.boxList[currentblockIdx][1];
            int blockHasFever = BlockData.boxList[currentblockIdx][3];

            GenerateBlock(SpawnPos, blockLife, blockHasFever).transform.SetParent(wave);
            currentblockIdx++;
        }

        //장애물 생성(블럭, 벽)
        if (obstaclesData.waveList[currentWaveIdx][0] > 0.3f)
        {
            //장애물 한 블럭
            float ObstacleSpawnCount = (obstaclesData.waveList[currentWaveIdx][0] - 0.3f) / 0.1f;
            for (int i = 0; i < ObstacleSpawnCount; ++i)
            {
                Vector2 blankPos = GetBlankPos();
                field[(int)blankPos.y, (int)blankPos.x] = 1;

                Vector2 spawnPos = new Vector2(widhtBorder.x + blankPos.x * oneblockSize, heightBorder.y - blankPos.y * oneblockSize);
                int blockLife = ObstaclesData.obstaclesBoxList[currentObstacleIdx][1];

                GenerateBlock(spawnPos, blockLife).transform.SetParent(wave);
                currentObstacleIdx++;
            }

            //TODO: 벽(하드코딩 변경 필요)
            if (obstaclesData.waveList[currentWaveIdx][0] > 0.4f)
            {
                foreach (var Wall in GenerateWall())
                    Wall.transform.SetParent(wave);
            }
        }

        //Life생성
        float lifeSpawnCount = Mathf.Floor((obstaclesData.waveList[currentWaveIdx][1] - 0.4f) / 0.1f);
        for (int i = 0; i < lifeSpawnCount; ++i)
        {
            Vector2 blankPos = GetBlankPos();
            field[(int)blankPos.y, (int)blankPos.x] = 1;

            Vector2 spawnPos = new Vector2(widhtBorder.x + blankPos.x * oneblockSize, heightBorder.y - blankPos.y * oneblockSize);

            GenerateLife(spawnPos, blockData.lifeList[currentLifeIdx][1]).transform.SetParent(wave);
            currentLifeIdx++;
        }

        currentWaveIdx++;

        wave.position = Vector2.up * oneblockSize * heightCount;
        return wave;
    }

    private WaveContent GenerateBlock(Vector2 pos, int lifeValue, int hasFever = 0)
    {
        BlockObj block = Instantiate(BlockPrefab);
        block.transform.position = pos;
        block.Initialize(lifeValue, hasFever);

        return block;
    }

    private WaveContent GenerateLife(Vector2 pos, int lifeValue)
    {
        LifeObj life = Instantiate(LifePrefab);
        life.transform.position = pos;
        life.Initialize(lifeValue);

        return life;
    }

    //TODO: 개선 필요(하드코딩)
    private List<WaveContent> GenerateWall()
    {
        List<WaveContent> Walls = new List<WaveContent>();

        for (int iy = 0; iy < 7; ++iy)
        {
            for (int ix = 0; ix < 5; ++ix)
            {
                if (field[iy, ix] == 1 && Random.Range(0f, 1f) <= 0.2f)
                {
                    WallObj wall = Instantiate(WallPrefab);
                    Vector2 spawnPos = new Vector2((widhtBorder.x + ix * oneblockSize) + (oneblockSize / 2), (heightBorder.y - iy * oneblockSize) - oneblockSize - 0.3f);
                    wall.transform.position = spawnPos;

                    Walls.Add(wall);
                }
                else
                {
                    continue;
                }
            }
        }
        return Walls;
    }

    private Vector2 GetBlankPos()
    {
        while (true)
        {
            int randPosX = Random.Range(0, widhtCount);
            int randPosY = Random.Range(1, heightCount);

            if (field[randPosY, randPosX] == 0)
                return new Vector2(randPosX, randPosY);
        }
    }

    //TODO: ObjectPool 추가 후 정리 필요
    public void ResetWave(Transform CurrentWave)
    {
        Destroy(CurrentWave.gameObject);
        field = new int[,] 
        {
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0 },
        };
    }
}
