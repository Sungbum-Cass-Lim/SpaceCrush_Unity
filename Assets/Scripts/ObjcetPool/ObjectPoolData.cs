using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolData
{
    private readonly PoolObjectType poolType;
    public PoolObjectType PoolType { get { return poolType; } }

    private GameObject prefabOrg;
    private Transform cachePoolContainer;

    private readonly Stack<GameObject> instanceStack = new Stack<GameObject>();

    private readonly string fileName;

    private int usePoolOpjectCount = 0;

    public static Dictionary<GameObject, PoolObjUsingData> usingObjectDic = new Dictionary<GameObject, PoolObjUsingData>();

    public GameObject PrefabOrg
    {
        get { return prefabOrg; }
        set { prefabOrg = value; }
    }

    public ObjectPoolData(GameObject _prefab_org, PoolObjectType _poolType, string _fileName)
    {
        this.prefabOrg = _prefab_org;
        this.poolType = _poolType;
        this.fileName = _fileName;
    }

    public void ApplyPoolObjectCache(Transform transform, int _cacheCount)
    {
        GameObject _instance = new GameObject();
        _instance.transform.SetParent(transform);
        _instance.name = $"{fileName}(CachePoolContainer)";

        cachePoolContainer = _instance.transform;

        for (int i = 0; i < _cacheCount; ++i)
        {
            CreateObjectPool(cachePoolContainer);
        }
    }

    public void CreateObjectPool(Transform transform)
    {
        GameObject _instance = GameObject.Instantiate<GameObject>(prefabOrg);
        _instance.name = $"{fileName}(ObjectPool)";
        _instance.transform.SetParent(transform);
        _instance.SetActive(false);

        instanceStack.Push(_instance);
    }

    public GameObject Load()
    {
        GameObject _instance = null;

        if (null != prefabOrg)
        {
            if (instanceStack.Count > 0)
            {
                _instance = instanceStack.Pop();
                _instance.SetActive(true);
            }
            else
            {
                CreateObjectPool(cachePoolContainer);
                _instance = instanceStack.Pop();
                _instance.SetActive(true);
            }

            usingObjectDic.Add(_instance, new PoolObjUsingData(poolType, fileName));
            usePoolOpjectCount++;
        }
        else
        {
            Debug.LogError($"----prefab is null objectPoolType : {poolType}, fileName : {fileName}");
        }

        return _instance;
    }

    public T Load<T>() where T : Component
    {
        T result = null;

        GameObject _instanceObj = Load();
        if (null != _instanceObj)
        {
            result = _instanceObj.GetComponent<T>();
            if (null == result)
            {
                usingObjectDic.Remove(_instanceObj);
                Debug.LogError($"not found component : {poolType}, fileName : {fileName}");
            }
        }

        return result;
    }

    public void PushInstance(GameObject _instance, bool worldPositionStays)
    {
        if (null != _instance)
        {
            usePoolOpjectCount--;

            try
            {
                _instance.transform.SetParent(cachePoolContainer, worldPositionStays);
                _instance.transform.position = Vector2.zero;

                instanceStack.Push(_instance);
                usingObjectDic.Remove(_instance);
            }
            catch (Exception e)
            {
                Debug.LogError($"msg : {e}");
            }

            _instance.SetActive(false);
        }
    }
}
