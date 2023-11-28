using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBase<T> where T : new()
{
    private static T instance;
    private static object lockObj = new();

    public static T Instance
    {
        get
        {
            lock (lockObj)
            {
                if (null == instance)
                    instance = new T();

                return instance;
            }
        }
    }
}
