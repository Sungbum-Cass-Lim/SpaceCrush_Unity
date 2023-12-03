using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockObj : WaveContent
{
    public TextMeshPro lifeText;

    private int life;
    private bool isFeverBlock = false;

    public GameObject DefultBlock;
    public GameObject FeverBlcok;

    public GameObject blockBurstPrefab;

    public void Initialize(int lifeValue, int hasFever)
    {
        this.life = lifeValue;
        lifeText.text = this.life.ToString();

        if(hasFever > 0)
        {
            isFeverBlock = true;

            DefultBlock.SetActive(false);
            FeverBlcok.SetActive(true);
        }
    }

    protected override void OnTouch()
    {
        if(GameMgr.Instance.GameLogic.isFever)
        {
            GameMgr.Instance.GameScore += life;
            DeleteEvent.Invoke();
            return;
        }

        life--;

        GameMgr.Instance.GameScore++;
        GameMgr.Instance.GameLogic.waveUpCount++;
        GameMgr.Instance.Player.RemoveLife();

        lifeText.text = this.life.ToString();

        if (life > 0)
            SoundMgr.Instance.PlayFx("block_knock");
        else
            DeleteEvent.Invoke();

    }

    protected override void OnDelete()
    {
        SoundMgr.Instance.PlayFx("block_break");

        if (isFeverBlock)
            GameMgr.Instance.GameLogic.FeverStart();
        
        GameObject burstEffect = Instantiate(blockBurstPrefab);
        burstEffect.transform.position = transform.position;

        Destroy(burstEffect, 1.0f);
        Destroy(gameObject);
    }
}
