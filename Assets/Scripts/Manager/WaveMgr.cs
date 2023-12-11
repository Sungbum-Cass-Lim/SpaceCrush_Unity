using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Newtonsoft.Json;

public class WaveMgr : SingletonComponentBase<WaveMgr>
{
    public int ScrrenWidth;

    GameStartResDto waveInfos;

    //한 번 받고 수정 할 일 없음
    private BoxData blockData;
    public BoxData BlockData { get { return blockData; } }

    private ObstaclesData obstaclesData;
    public ObstaclesData ObstaclesData { get { return obstaclesData; } }

    public int[,] field = new int[GameConfig.FILED_HEIGHT_SIZE, GameConfig.FILED_WIDHT_SIZE];

    public float oneblockSize;
    public Vector2 widhtBorder;
    public Vector2 heightBorder;

    //이 부분 스택으로 바꾸거나 더 좋은 방법 없을지 고민..
    private int currentWaveIdx = 0;
    private int currentblockIdx = 0;
    private int playerLifeIdx = 0;
    private int currentObstacleIdx = 0;

    private int dataLogCount = 0;
    private int[][] dataLog = new int[10000][];

    protected override void InitializeSingleton(){}

    public void Initilize(string GameStartResDto)
    {
        TextAsset info = new TextAsset(GameStartResDto);
        waveInfos = JsonHelper.ReadJson<GameStartResDto>(info);
        Debug.Log("Load Wave Data");

        blockData = waveInfos.boxData;
        obstaclesData = blockData.obstaclesData;

        //blockSize와 여백 구하는 공식(ScreenWidth / blockImageWidth / fieldWidthCount + 여백 <= 오류 있음)
        oneblockSize = (ScrrenWidth / 114f / GameConfig.FILED_WIDHT_SIZE) + 0.2f;

        //field Width, Height 최저값/최대값 구하는 공식(오류 있음)
        widhtBorder = new Vector2(GameConfig.FILED_WIDHT_SIZE / 2 * -oneblockSize, GameConfig.FILED_WIDHT_SIZE / 2 * oneblockSize);
        heightBorder = new Vector2(GameConfig.FILED_HEIGHT_SIZE / 2 * -oneblockSize, GameConfig.FILED_HEIGHT_SIZE / 2 * oneblockSize);
    }

    public WaveObj GenerateWave()
    {
        field = new int[GameConfig.FILED_HEIGHT_SIZE, GameConfig.FILED_WIDHT_SIZE];

        WaveObj wave = ObjectPoolMgr.Instance.Load<WaveObj>(PoolObjectType.Wave, "Wave");
        wave.transform.SetParent(this.transform);

        //Block 한 줄 생성
        for (int i = 0; i < GameConfig.FILED_WIDHT_SIZE; i++)
        {
            field[0, i] = 1;

            Vector2 SpawnPos = new Vector2(widhtBorder.x + oneblockSize * i, heightBorder.y + Random.Range(-0.2f, 0.2f));

            int blockIndex = blockData.boxList[currentblockIdx][0];
            int blockLife = BlockData.boxList[currentblockIdx][1];
            int blockHasFever = BlockData.boxList[currentblockIdx][3];

            WaveContent block = GenerateBlock(SpawnPos, blockIndex, ContentType.Block, blockLife, blockHasFever);
            block.transform.SetParent(wave.transform);

            block.parentWave = wave;
            wave.WaveContentAdd(block);
                
            currentblockIdx++;
        }

        //장애물 생성(블럭, 벽)
        if (obstaclesData.waveList[currentWaveIdx][0] > 0.3f)
        {
            //장애물 한 블럭
            float obstacleSpawnCount = (obstaclesData.waveList[currentWaveIdx][0] - 0.3f) / 0.1f;
            for (int i = 0; i < obstacleSpawnCount; ++i)
            {
                Vector2 blankPos = GetBlankPos();
                field[(int)blankPos.y, (int)blankPos.x] = 1;

                Vector2 spawnPos = new Vector2(widhtBorder.x + blankPos.x * oneblockSize, heightBorder.y - blankPos.y * oneblockSize);

                int blockIndex = blockData.boxList[currentObstacleIdx][0];
                int blockLife = ObstaclesData.obstaclesBoxList[currentObstacleIdx][1];

                WaveContent blockObstacle = GenerateBlock(spawnPos, blockIndex, ContentType.Obstacle, blockLife);
                blockObstacle.transform.SetParent(wave.transform);

                wave.WaveContentAdd(blockObstacle);

                currentObstacleIdx++;
            }

            //TODO: 벽(하드코딩 변경 필요)
            if (obstaclesData.waveList[currentWaveIdx][0] > 0.4f)
            {
                foreach (var wall in GenerateWall())
                {
                    WaveContent wallObstacle = wall;
                    wall.transform.SetParent(wave.transform);

                    wave.WaveContentAdd(wall);
                }
            }
        }

        //Life생성
        float lifeSpawnCount = Mathf.Floor((obstaclesData.waveList[currentWaveIdx][1] - 0.4f) / 0.1f);
        for (int i = 0; i < lifeSpawnCount; ++i)
        {
            Vector2 blankPos = GetBlankPos();
            field[(int)blankPos.y, (int)blankPos.x] = 1;

            Vector2 spawnPos = new Vector2(widhtBorder.x + blankPos.x * oneblockSize, heightBorder.y - blankPos.y * oneblockSize);

            int lifeIndex = blockData.lifeList[playerLifeIdx][0];
            int lifeValue = blockData.lifeList[playerLifeIdx][1];

            WaveContent life = GenerateLife(spawnPos, lifeIndex, lifeValue);
            life.transform.SetParent(wave.transform);

            wave.WaveContentAdd(life);

            playerLifeIdx++;
        }

        currentWaveIdx++;
        return wave;
    }

    private WaveContent GenerateBlock(Vector2 pos, int index, ContentType type, int lifeValue, int hasFever = 0)
    {
        BlockObj block = ObjectPoolMgr.Instance.Load<BlockObj>(PoolObjectType.Wave, "Block");
        block.transform.position = pos;
        block.Initialize(index, type, lifeValue, hasFever);

        return block;
    }

    private WaveContent GenerateLife(Vector2 pos, int index, int lifeValue)
    {
        LifeObj life = ObjectPoolMgr.Instance.Load<LifeObj>(PoolObjectType.Wave, "Life");
        life.transform.position = pos;
        life.Initialize(index, ContentType.Life, lifeValue);

        return life;
    }

    //TODO: 개선 필요(하드코딩)
    private List<WaveContent> GenerateWall()
    {
        List<WaveContent> Walls = new List<WaveContent>();

        for (int iy = 0; iy < GameConfig.FILED_HEIGHT_SIZE; ++iy)
        {
            for (int ix = 0; ix < GameConfig.FILED_WIDHT_SIZE; ++ix)
            {
                if (field[iy, ix] == 1 && Random.Range(0f, 1f) <= 0.2f)
                {
                    WallObj wall = ObjectPoolMgr.Instance.Load<WallObj>(PoolObjectType.Wave, "Wall");
                    wall.Initialize(ContentType.Wall);

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
            int randPosX = Random.Range(0, GameConfig.FILED_WIDHT_SIZE);
            int randPosY = Random.Range(2, GameConfig.FILED_HEIGHT_SIZE - 1);

            if (field[randPosY, randPosX] == 0)
                return new Vector2(randPosX, randPosY);
        }
    }

    public void UploadLog(int index, int value, int type)
    {
        dataLog.Add(dataLogCount, index, value, type);
        dataLogCount++;
    }
    public void SendLog()
    {
        GameEndReqDto endReq = new();

        endReq.score = GameMgr.Instance.GameScore;
        endReq.totalCollisionData = new int[dataLogCount][];
        endReq.totalCollisionData = dataLog.Slice(dataLogCount);

        NetworkMgr.Instance.RequestEndGame(endReq);
    }
}
