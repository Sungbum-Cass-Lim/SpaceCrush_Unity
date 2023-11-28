using Unity.VisualScripting;
using UnityEngine;

public abstract class SingletonComponentBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    private static object lockObj = new();
    private static bool isQuitApplication = false;

    public static T Instance
    {
        get
        {
            if (isQuitApplication)
                return null;

            lock (lockObj)
            {
                if (null == instance)
                {
                    var componentName = typeof(T).ToString();

                    GameObject findGameObject = GameObject.Find(componentName);
                    if (null == findGameObject)
                    {
                        GameObject singleton = new()
                        {
                            name = "(SingletonComponent)" + componentName
                        };
                        instance = singleton.AddComponent<T>();
                    }
                    else
                        instance = findGameObject.GetComponent<T>();
                }

                Debug.Log(instance.name);
                DontDestroyOnLoad(instance);
                return instance;
            }
        }
    }

    private void Awake()
    {
        isQuitApplication = false;
        InitializeSingleton();
    }

    protected abstract void InitializeSingleton();

    private void OnApplicationQuit()
    {
        isQuitApplication = true;
    }
}
