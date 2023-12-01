using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    #region WaveInfo
    public TextAsset waveJsonData;
    WaveInfo waveInfos;

    //�� �� �ް� ���� �� �� ����
    private BoxData blockData;
    public BoxData BlockData { get { return blockData; } }

    private ObstaclesData obstaclesData;
    public ObstaclesData ObstaclesData { get { return obstaclesData; } }
    #endregion

    #region Prefab
    public WaveObj wavePrefab;
    public BlockObj blockPrefab;
    public LifeObj lifePrefab;
    public WallObj wallPrefab;
    #endregion

    public int[,] field = new int[GameConfig.FILED_HEIGHT_SIZE, GameConfig.FILED_WIDHT_SIZE];

    public float oneblockSize;
    public Vector2 widhtBorder;
    public Vector2 heightBorder;

    //�� �κ� �������� �ٲٰų� �� ���� ��� ������ ����..
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

        //blockSize�� ���� ���ϴ� ����(ScreenWidth / blockImageWidth / fieldWidthCount + ����)
        oneblockSize = (Screen.width / 114f / GameConfig.FILED_WIDHT_SIZE) + 0.2f;

        //field Width, Height ������/�ִ밪 ���ϴ� ����(Ȧ���� ���������� ¦���� ���� ����)
        widhtBorder = new Vector2(GameConfig.FILED_WIDHT_SIZE / 2 * -oneblockSize, GameConfig.FILED_WIDHT_SIZE / 2 * oneblockSize);
        heightBorder = new Vector2(GameConfig.FILED_HEIGHT_SIZE / 2 * -oneblockSize, GameConfig.FILED_HEIGHT_SIZE / 2 * oneblockSize);
    }

    public WaveObj GenerateWave()
    {
        field = new int[GameConfig.FILED_HEIGHT_SIZE, GameConfig.FILED_WIDHT_SIZE];

        WaveObj wave = Instantiate(wavePrefab);
        wave.transform.SetParent(this.transform);

        //Block �� �� ����
        for (int i = 0; i < GameConfig.FILED_WIDHT_SIZE; i++)
        {
            field[0, i] = 1;

            Vector2 SpawnPos = new Vector2(widhtBorder.x + oneblockSize * i, heightBorder.y + Random.Range(-0.2f, 0.2f));
            int blockLife = BlockData.boxList[currentblockIdx][1];
            int blockHasFever = BlockData.boxList[currentblockIdx][3];

            GenerateBlock(SpawnPos, blockLife, blockHasFever).transform.SetParent(wave.transform);
            currentblockIdx++;
        }

        //��ֹ� ����(����, ��)
        if (obstaclesData.waveList[currentWaveIdx][0] > 0.3f)
        {
            //��ֹ� �� ����
            float ObstacleSpawnCount = (obstaclesData.waveList[currentWaveIdx][0] - 0.3f) / 0.1f;
            for (int i = 0; i < ObstacleSpawnCount; ++i)
            {
                Vector2 blankPos = GetBlankPos();
                field[(int)blankPos.y, (int)blankPos.x] = 1;

                Vector2 spawnPos = new Vector2(widhtBorder.x + blankPos.x * oneblockSize, heightBorder.y - blankPos.y * oneblockSize);
                int blockLife = ObstaclesData.obstaclesBoxList[currentObstacleIdx][1];

                GenerateBlock(spawnPos, blockLife).transform.SetParent(wave.transform);
                currentObstacleIdx++;
            }

            //TODO: ��(�ϵ��ڵ� ���� �ʿ�)
            if (obstaclesData.waveList[currentWaveIdx][0] > 0.4f)
            {
                foreach (var Wall in GenerateWall())
                    Wall.transform.SetParent(wave.transform);
            }
        }

        //Life����
        float lifeSpawnCount = Mathf.Floor((obstaclesData.waveList[currentWaveIdx][1] - 0.4f) / 0.1f);
        for (int i = 0; i < lifeSpawnCount; ++i)
        {
            Vector2 blankPos = GetBlankPos();
            field[(int)blankPos.y, (int)blankPos.x] = 1;

            Vector2 spawnPos = new Vector2(widhtBorder.x + blankPos.x * oneblockSize, heightBorder.y - blankPos.y * oneblockSize);

            GenerateLife(spawnPos, blockData.lifeList[currentLifeIdx][1]).transform.SetParent(wave.transform);
            currentLifeIdx++;
        }

        currentWaveIdx++;
        return wave;
    }

    private WaveContent GenerateBlock(Vector2 pos, int lifeValue, int hasFever = 0)
    {
        BlockObj block = Instantiate(blockPrefab);
        block.transform.position = pos;
        block.Initialize(lifeValue, hasFever);

        return block;
    }

    private WaveContent GenerateLife(Vector2 pos, int lifeValue)
    {
        LifeObj life = Instantiate(lifePrefab);
        life.transform.position = pos;
        life.Initialize(lifeValue);

        return life;
    }

    //TODO: ���� �ʿ�(�ϵ��ڵ�)
    private List<WaveContent> GenerateWall()
    {
        List<WaveContent> Walls = new List<WaveContent>();

        for (int iy = 0; iy < GameConfig.FILED_HEIGHT_SIZE; ++iy)
        {
            for (int ix = 0; ix < GameConfig.FILED_WIDHT_SIZE; ++ix)
            {
                if (field[iy, ix] == 1 && Random.Range(0f, 1f) <= 0.2f)
                {
                    WallObj wall = Instantiate(wallPrefab);
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
}