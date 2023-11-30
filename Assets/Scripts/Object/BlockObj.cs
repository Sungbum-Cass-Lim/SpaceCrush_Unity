using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockObj : WaveContent
{
    public TextMeshPro lifeText;
    private int life;
    private bool isFever = false;

    public GameObject DefultBlock;
    public GameObject FeverBlcok;
    

    public void Initialize(int lifeValue, int hasFever)
    {
        this.life = lifeValue + 500;
        lifeText.text = this.life.ToString();

        if(hasFever > 0)
        {
            isFever = true;

            DefultBlock.SetActive(false);
            FeverBlcok.SetActive(true);
        }
    }

    protected override void OnTouch()
    {
        if(GameMgr.Instance.Player.feverTime > 0)
        {
            DeleteEvent.Invoke();
            return;
        }

        life--;
        GameMgr.Instance.GameLogic.waveUpEvent.Invoke();

        lifeText.text = this.life.ToString();
        GameMgr.Instance.Player.RemoveLife();

        if (life <= 0)
            DeleteEvent.Invoke();
    }

    protected override void OnDelete()
    {
        if(isFever)
            GameMgr.Instance.Player.feverTime = GameConfig.FEVER_TIME;
        
        Destroy(gameObject);
    }
}
