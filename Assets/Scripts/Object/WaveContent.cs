using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WaveContent : MonoBehaviour
{
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
