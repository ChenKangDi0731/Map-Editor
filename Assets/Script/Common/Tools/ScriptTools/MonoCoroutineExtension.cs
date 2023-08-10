using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoCoroutineExtension : MonoSingleton<MonoCoroutineExtension>
{
    /*
    public static MonoCoroutineExtension _instance;
    public static MonoCoroutineExtension instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("MonoCoroutineControl");
                if (obj == null)
                {
                    Debug.LogError("<color=#ff0000>Create MonoCoroutineInit instance failed</color>");
                    return null;
                }

                DontDestroyOnLoad(obj);
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;

                _instance = obj.AddComponentOnce<MonoCoroutineExtension>();
            }

            return _instance;
        }
    }

    */

    private void Awake()
    {
        //if (instance == null)
        //{
        //    _instance = this;
        //}
        //else
        //{
        //    debug.logerror("<color=#ff0000>是否存在多个monocoroutineinit实例，请检查</color>");
        //}
    }


    #region Coroutine

    public IEnumerator Delay(float delayTime,Action action)
    {
        yield return new WaitForSeconds(delayTime);

        action?.Invoke();
    }

    #endregion Coroutine

}
