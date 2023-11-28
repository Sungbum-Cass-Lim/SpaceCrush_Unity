using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class JsonHelper
{
    public static T ReadJson<T>(TextAsset JsonFile)
    {
        //TextAsset textAsset = ResourceMgr.Instance.GetResource<TextAsset>(resourcePath);

        return JsonConvert.DeserializeObject<T>(JsonFile.text);
    }
}
