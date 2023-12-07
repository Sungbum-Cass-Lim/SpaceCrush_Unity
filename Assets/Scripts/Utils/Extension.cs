using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension 
{
    #region JaggedArr_Extensions
    public static void AddJaggedArray<T>(this T[][] arr, int offset, T value1)
    {
        if(offset > arr.Length - 1)
        {
            Debug.LogError("OutOfRange");
            return;
        }

        arr[offset] = new T[1];

        arr[offset][0] = value1;
    }
    public static void AddJaggedArray<T>(this T[][] arr, int offset, T value1, T value2)
    {
        if (offset > arr.Length - 1)
        {
            Debug.LogError("OutOfRange");
            return;
        }

        arr[offset] = new T[2];

        arr[offset][0] = value1;
        arr[offset][1] = value2;
    }
    public static void AddJaggedArray<T>(this T[][] arr, int offset, T value1, T value2, T value3)
    {
        if (offset > arr.Length - 1)
        {
            Debug.LogError("OutOfRange");
            return;
        }

        arr[offset] = new T[3];

        arr[offset][0] = value1;
        arr[offset][1] = value2;
        arr[offset][2] = value3;
    }
    #endregion
}
