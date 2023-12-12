using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
//using UnityEngine.ResourceManagement.AsyncOperations;

public enum PoolObjectType
{ 
    None = -1,
    Wave = 0,
    Player,
    Effect,
    End
}

public class PoolObjInstanceData
{
    public Dictionary<string, ObjectPoolData> objDic = new Dictionary<string, ObjectPoolData>();
}

public struct PoolObjUsingData
{
    public PoolObjectType type;
    public string filename;

    public PoolObjUsingData(PoolObjectType _type, string _filename)
    {
        type = _type;
        filename = _filename;
    }
}

public class ObjectPoolMgr : SingletonComponentBase<ObjectPoolMgr>
{
    private readonly PoolObjInstanceData[] poolArray = new PoolObjInstanceData[(int)PoolObjectType.End];
    //public Dictionary<GameObject, PoolObjUsingData> usingObjectDic = new Dictionary<GameObject, PoolObjUsingData>();

    protected override void InitializeSingleton()
    {
        
    }

    private void Awake()
    {
        int instanceArrayCount = poolArray.Length;
        for (int n = 0; n < instanceArrayCount; ++n)
        {
            poolArray[n] = new PoolObjInstanceData();
        }

        Initialize();
    }

    public void Initialize()
    {
        IncreasePoolObjectCache(5, PoolObjectType.Wave, "Wave");
        IncreasePoolObjectCache(25, PoolObjectType.Wave, "Block");
        IncreasePoolObjectCache(15, PoolObjectType.Wave, "Wall");
        IncreasePoolObjectCache(15, PoolObjectType.Wave, "Life");
        IncreasePoolObjectCache(1, PoolObjectType.Player, "Player");
        IncreasePoolObjectCache(100, PoolObjectType.Player, "LifeTail");
        IncreasePoolObjectCache(50, PoolObjectType.Effect, "LifeTailRemove");
        IncreasePoolObjectCache(10, PoolObjectType.Effect, "SoundComponent");
        IncreasePoolObjectCache(10, PoolObjectType.Effect, "BlockBurst");
    }

    public void IncreasePoolObjectCache(int _cacheCount, PoolObjectType _poolType, string _fileName)
    {
        ObjectPoolData objectPoolData = GetPoolObjectData(_poolType, _fileName);

        objectPoolData.ApplyPoolObjectCache(this.transform, _cacheCount);
    }

    private ObjectPoolData GetPoolObjectData(PoolObjectType _pool_type, string _file_name)
    {
        ObjectPoolData _pool_data = GetLoadedPoolObjectData(_pool_type, _file_name);
        if (null == _pool_data)
        {
            GameObject _prefabOrg = Resources.Load<GameObject>(_file_name);
            if (null != _prefabOrg)
            {
                _pool_data = new ObjectPoolData( _prefabOrg, _pool_type, _file_name);

                Dictionary<string, ObjectPoolData> objDic = poolArray[(int)_pool_type].objDic;
                objDic.Add(_file_name, _pool_data);

                return _pool_data;
            }
        }
        else
        {
            if (_pool_data.PrefabOrg == null)
                Debug.LogError($"----prefab is null objectPoolType : {_pool_type}, fileName : {_file_name}");
        }

        return _pool_data;
    }

    
    private ObjectPoolData GetLoadedPoolObjectData(PoolObjectType _pool_type, string _file_name)
    {
        ObjectPoolData _result = null;

        if (PoolObjectType.None < _pool_type && _pool_type < PoolObjectType.End)
        {
            Dictionary<string, ObjectPoolData> _obj_dic = poolArray[(int)_pool_type].objDic;
            if (null != _obj_dic)
                _obj_dic.TryGetValue(_file_name, out _result);
        }

        return _result;
    }

    public T Load<T>(PoolObjectType _pool_type, string _file_name) where T : Component
    {
        ObjectPoolData poolData = GetPoolObjectData(_pool_type, _file_name);

        if (null != poolData)
        {
            return poolData.Load<T>();
        }

        return null;
    }

    public void ReleasePool(GameObject _instance, bool worldPositionStays = true)
    {
        if (null != _instance)
        {
            PoolObjUsingData data;
            if (ObjectPoolData.usingObjectDic.TryGetValue(_instance, out data))
            {
                Dictionary<string, ObjectPoolData> objDic = poolArray[(int)data.type].objDic;
                ObjectPoolData objData;
                if (objDic.TryGetValue(data.filename, out objData))
                {
                    objData.PushInstance(_instance, worldPositionStays);
                }
                else
                {
                    Debug.LogError($"lost pool object data instance name : {_instance.name}");
                }
            }
            else
            {
                Debug.LogError($"lost pool using object data instance name : {_instance.name}");

            }
        }
    }
}
