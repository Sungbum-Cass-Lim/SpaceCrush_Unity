using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class WallObj : WaveContent
{
    public void Initialize()
    {
        //TODO: Wall Sprite��ü
    }

    //TODO: Wall��ũ��Ʈ �ۼ� �ʿ�
    protected override void OnTouch()
    {
        //������ ��
        if (GameMgr.Instance.Player.transform.position.x < transform.position.x)
        {
            GameMgr.Instance.Player.moveMaxX = transform.position.x - 0.3f;
        }

        //���� ��
        else
        {
            GameMgr.Instance.Player.moveMinX = transform.position.x + 0.3f;
        }
    }

    protected override void OnDelete()
    {

    }
}
