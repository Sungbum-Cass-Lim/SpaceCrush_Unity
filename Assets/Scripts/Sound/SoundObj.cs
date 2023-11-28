using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
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

        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
        Destroy(this);
    }

    //TODO: ���߿� Update���� ����ȭ ������ �������� ���� �ʿ�
    private void Update()
    {
        if (loop == false && audioLength <= Time.realtimeSinceStartup)
            stopCallBack.Invoke();
    }
}
