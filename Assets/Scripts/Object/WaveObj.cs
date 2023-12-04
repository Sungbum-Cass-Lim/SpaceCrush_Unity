using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObj : MonoBehaviour
{
    public Rigidbody2D rigidbody2d;
    private List<WaveContent> waveContentList = new List<WaveContent>();

    public void WaveContentAdd(WaveContent content)
    {
        content.parentWave = this;
        waveContentList.Add(content);
    }

    public void WaveRelease()
    {
        for(int i = waveContentList.Count - 1; i >= 0; i--)
        {
            waveContentList[i].parentWave = null;

            ObjectPoolMgr.Instance.ReleasePool(waveContentList[i].gameObject);
        }

        waveContentList.Clear();
        ObjectPoolMgr.Instance.ReleasePool(this.gameObject, false);
    }

    public void ContentRelease(WaveContent content)
    {
        if(waveContentList.Contains(content))
        {
            content.parentWave = null;

            ObjectPoolMgr.Instance.ReleasePool(content.gameObject);
            waveContentList.Remove(content);
        }
    }
}
