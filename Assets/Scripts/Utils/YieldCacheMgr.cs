using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

internal static class YieldCacheMgr
{
    class FloatComparer : IEqualityComparer<float>
    {
        bool IEqualityComparer<float>.Equals(float x, float y) => x == y;
        int IEqualityComparer<float>.GetHashCode(float obj) => obj.GetHashCode();
    }

    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new();
    public static readonly WaitForFixedUpdate WaitForFixedUpdate = new();

    private static readonly Dictionary<float, WaitForSeconds> timeInterval = new(new FloatComparer());

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        if (!timeInterval.TryGetValue(seconds, out WaitForSeconds wfs))
            timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));

        return wfs;
    }
}
