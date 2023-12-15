using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundObj : MonoBehaviour
{
    public AudioSource audioSource;
    public Action stopCallBack;

    [SerializeField]
    private AudioClip clip;
    private float audioLength;
    private float freetime = 0.2f;
    private bool loop;

    public void Init(AudioClip clip, bool isLoop = false)
    {
        this.clip = clip;
        audioLength = Time.realtimeSinceStartup + clip.length;
        loop = isLoop;
        stopCallBack = Stop;
    }

    public void Play()
    {
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = SoundMgr.isMute == true ? 0.0f : 0.5f;

        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
        ObjectPoolMgr.Instance.ReleasePool(gameObject);
    }

    //TODO: 나중에 Update말고 최적화 가능한 방향으로 수정 필요
    private void Update()
    {
        if (loop == false && audioLength <= Time.realtimeSinceStartup)
            stopCallBack.Invoke();
    }
}
