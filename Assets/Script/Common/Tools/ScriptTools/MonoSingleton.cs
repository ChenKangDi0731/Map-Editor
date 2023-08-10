using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> :MonoBehaviour where T:MonoBehaviour
{
    public static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject(typeof(T).Name);
                if (obj == null)
                {
                    Debug.LogError("<color=#ff0000>Create " + typeof(T).Name + " instance failed</color>");
                    return default(T);
                }

                GameObject.DontDestroyOnLoad(obj);
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;

                _instance = obj.AddComponentOnce<T>();
            }

            return _instance;
        }
    }
}
