using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallObj : WaveContent
{
    public void Initialize()
    {
        //TODO: Wall Sprite±³Ã¼
    }

    protected override void OnTouch()
    {
        Debug.Log("Wall");
        GameMgr.Instance.Player.movementPos = new Vector2(transform.position.x, -1f);
        //EditorApplication.isPaused = true;
    }

    protected override void OnDelete()
    {

    }
}
