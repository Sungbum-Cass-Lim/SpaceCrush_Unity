using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContentType
{
    Block,
    Life,
    Wall,
}

public abstract class WaveContent : MonoBehaviour
{
    public ContentType contentType;
    public WaveObj parentWave;

    public Action TouchEvent;
    public Action DeleteEvent;

    protected void Start()
    {
        TouchEvent = OnTouch;
        DeleteEvent = OnDelete; 
    }

    protected abstract void OnTouch();
    protected abstract void OnDelete();
}
