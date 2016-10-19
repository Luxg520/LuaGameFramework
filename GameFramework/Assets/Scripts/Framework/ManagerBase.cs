using UnityEngine;
using System.Collections;
/// <summary>
/// 管理器基类
/// </summary>
public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();
            }
            if (instance == null)
            {
                GameObject root = GameObject.Find("ManagerRoot");
                if (root == null)
                {
                    root = new GameObject("ManagerRoot");
                    DontDestroyOnLoad(root);
                }

                GameObject go = new GameObject("_" + typeof(T).FullName);
                instance = go.AddComponent<T>();
                instance.transform.SetParent(root.transform);
                instance.Init();
            }
            return instance;
        }
    }

    public virtual void Init()
    {

    }

    public virtual void Release()
    {

    }
}
