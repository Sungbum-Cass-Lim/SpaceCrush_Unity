using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class WallObj : WaveContent
{
    public void Initialize()
    {
        //TODO: Wall Sprite교체
    }

    //TODO: Wall스크립트 작성 필요
    protected override void OnTouch()
    {
        GameMgr.Instance.Player.isWall = true;
        GameMgr.Instance.Player.wallPos = transform.position;
    }

    protected override void OnDelete()
    {

    }
}
