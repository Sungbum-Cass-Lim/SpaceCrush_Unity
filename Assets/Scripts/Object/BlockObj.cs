using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockObj : WaveContent
{
    public TextMeshPro lifeText;
    public int life;

    public GameObject DefultBlock;
    public GameObject FeverBlcok;

    public void Initialize(int lifeValue, int hasFever)
    {
        this.life = lifeValue;
        lifeText.text = this.life.ToString();

        if(hasFever > 0)
        {
            DefultBlock.SetActive(false);
            FeverBlcok.SetActive(true);
        }
    }

    protected override void OnTouch()
    {
        Debug.Log("Block Touch");
        transform.parent.position += Vector3.up * 0.65f;

        life--;
        lifeText.text = this.life.ToString();
        GameMgr.Instance.Player.RemoveLife();

        if (life <= 0)
            DeleteEvent.Invoke();
    }

    protected override void OnDelete()
    {
        Destroy(gameObject);
    }
}
