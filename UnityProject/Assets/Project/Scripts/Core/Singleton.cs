using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static object s_syncObject = new object();
    private static T s_instance = null;

    public static T Inst
    {
        get
        {
            if (s_instance == null)
            {
                lock (s_syncObject)
                {
                    s_instance = FindObjectOfType<T>();
                    if (s_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        s_instance = obj.AddComponent<T>();
                    }
                }
            }
            return s_instance;
        }
    }

    protected virtual void Awake()
    {
        if (null == s_instance)
        {
            s_instance = this as T;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        SetParent();

        DebugUtil.Log($"Singleton Awake ({this.name})");
    }

    void SetParent()
    {
        GameObject obj = GameObject.Find("ParentName");

        if (obj == null)
        {
            Transform parentTf = new GameObject("ParentName").transform;
            transform.SetParent(parentTf);
            DontDestroyOnLoad(parentTf);
        }
        else
        {
            transform.SetParent(obj.transform);
        }
    }
}
