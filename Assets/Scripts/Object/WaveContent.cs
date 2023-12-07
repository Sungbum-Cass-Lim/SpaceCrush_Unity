using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContentType
{
    Block,
    Obstacle,
    Life,
    Wall,
}

public abstract class WaveContent : MonoBehaviour
{
    public WaveObj parentWave;

    public ContentType contentType;
    protected int index;

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
