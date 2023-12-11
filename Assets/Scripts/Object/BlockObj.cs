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

    public void Initialize(int index, ContentType type, int lifeValue, int hasFever)
    {
        this.index = index;
        contentType = type;
        isFeverBlock = false;

        life = lifeValue;
        lifeText.text = this.life.ToString();

        DefultBlock.SetActive(true);
        FeverBlcok.SetActive(false);

        if (hasFever > 0)
        {
            isFeverBlock = true;

            DefultBlock.SetActive(false);
            FeverBlcok.SetActive(true);
        }
    }

    protected override void OnTouch()
    {
        UploadCrushData();

        if (GameMgr.Instance.GameLogic.isFever)
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
        
        GameObject burstEffect = ObjectPoolMgr.Instance.Load<Transform>(PoolObjectType.Effect, "BlockBurst").gameObject;
        burstEffect.transform.position = transform.position;

        Destroy(burstEffect, 1.0f);
        parentWave.ContentRelease(this);
    }

    private void UploadCrushData()
    {
        if(GameMgr.Instance.GameLogic.isFever == true)
        {
            if (contentType == ContentType.Block)
            {
                if (isFeverBlock)
                {
                    WaveMgr.Instance.UploadLog(index, life, 7);
                }
                else
                {
                    WaveMgr.Instance.UploadLog(index, life, 6);
                }
            }
            else if (contentType == ContentType.Obstacle)
            {
                WaveMgr.Instance.UploadLog(index, life, 8);
            }
        }

        else if (life > 1)
        {
            if (contentType == ContentType.Block)
            {
                if (isFeverBlock)
                {
                    WaveMgr.Instance.UploadLog(index, life, 1);
                }
                else
                {
                    WaveMgr.Instance.UploadLog(index, life, 0);
                }
            }
            else if (contentType == ContentType.Obstacle)
            {
                WaveMgr.Instance.UploadLog(index, life, 4);
            }
        }

        else
        {
            if (contentType == ContentType.Block)
            {
                if (this.isFeverBlock)
                {
                    WaveMgr.Instance.UploadLog(index, life, 3);
                }
                else
                {
                    WaveMgr.Instance.UploadLog(index, life, 2);
                }
            }
            else if (contentType == ContentType.Obstacle)
            {
                WaveMgr.Instance.UploadLog(index, life, 5);
            }
        }
    }
}
