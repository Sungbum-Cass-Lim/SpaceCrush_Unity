using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeObj : WaveContent
{
    public TextMeshPro lifeText;
    private int life;

    public void Initialize(int lifeValue)
    {
        this.life = lifeValue;
        lifeText.text = lifeValue.ToString();
    }

    protected override void OnTouch()
    {
        GameMgr.Instance.Player.AddLife(life);

        gameObject.SetActive(false);
    }

    protected override void OnDelete()
    {
        SoundMgr.Instance.PlayFx("item");
        parentWave.ContentRelease(this);
    }
}
