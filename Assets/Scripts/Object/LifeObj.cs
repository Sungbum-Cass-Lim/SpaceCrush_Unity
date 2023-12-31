using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeObj : WaveContent
{
    public TextMeshPro lifeText;
    private int life;

    public void Initialize(int index, ContentType type, int lifeValue)
    {
        this.index = index;
        contentType = type;
        life = lifeValue;

        lifeText.text = lifeValue.ToString();
    }

    protected override void OnTouch()
    {
        GameMgr.Instance.Player.AddLife(life);
        WaveMgr.Instance.UploadLog(index, GameMgr.Instance.Player.GetLife(), 9);

        DeleteEvent.Invoke();
    }

    protected override void OnDelete()
    {
        SoundMgr.Instance.PlayFx("item");
        parentWave.ContentRelease(this);
    }
}
