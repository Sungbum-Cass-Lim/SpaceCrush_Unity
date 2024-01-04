using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using BestHTTP.SocketIO;
using UnityEditor;
using UnityEngine;

public class WallObj : WaveContent
{
    public List<GameObject> wallType = new List<GameObject>();
    public BoxCollider2D boxCollider2D;

    private int currentType = 0;
    private float centerX;
    private float halfExtentsX;

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
        GameMgr.Instance.Player.CurrentWall = this;

        //오른쪽 벽
        if (GameMgr.Instance.Player.transform.position.x < transform.position.x)
        {
            GameMgr.Instance.Player.moveMaxX = boxCollider2D.bounds.min.x;
        }

        //왼쪽 벽
        else
        {
            GameMgr.Instance.Player.moveMinX = boxCollider2D.bounds.max.x;
        }
    }

    protected override void OnDelete()
    {
        
    }
}
