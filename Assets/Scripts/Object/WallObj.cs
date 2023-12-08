using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using BestHTTP.SocketIO;
using UnityEditor;
using UnityEngine;

public class WallObj : WaveContent
{
    public List<GameObject> wallType = new List<GameObject>();
    private int currentType = 0;

    public void Initialize(ContentType type)
    {
        contentType = type;

        wallType[currentType].SetActive(false);

        currentType = Random.Range(0, wallType.Count);
        wallType[currentType].SetActive(true);
    }

    //TODO: Wall스크립트 작성 필요
    protected override void OnTouch()
    {
        //오른쪽 벽
        if (GameMgr.Instance.Player.transform.position.x < transform.position.x)
        {
            GameMgr.Instance.Player.moveMaxX = transform.position.x - 0.3f;
        }

        //왼쪽 벽
        else
        {
            GameMgr.Instance.Player.moveMinX = transform.position.x + 0.3f;
        }
    }

    protected override void OnDelete()
    {
        
    }
}
