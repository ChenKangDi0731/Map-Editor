using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehavioursExtension
{

    public static T AddComponentOnce<T>(this MonoBehaviour mono)where T:MonoBehaviour
    {
        T returnComponent = mono.GetComponent<T>();

        if (returnComponent != null)
        {
            Component.DestroyImmediate(returnComponent);
            returnComponent = null;
        }

        returnComponent = mono.gameObject.AddComponent<T>();

        return returnComponent;

    }


    public static void MapAllComponent<T>(this MonoBehaviour mono,Action<T> action)
    {
        if (mono == null) return;
        if (action == null) return;

        T[] tempArray;
        tempArray = mono.GetComponentsInChildren<T>();
        if (tempArray == null || tempArray.Length == 0)
        {
            Debug.LogWarning("get children component failed , gameObject name = " + mono.gameObject.name);
            return;
        }

        for(int index = 0; index < tempArray.Length; index++)
        {
            if (tempArray[index] == null) continue;

            action?.Invoke(tempArray[index]);
        }

    }


    #region Coroutine

    public static Coroutine Delay(this MonoBehaviour mono, float time,Action action)
    {
        if (time < 0)
        {
            time = 0;
        }

        return mono.StartCoroutine(MonoCoroutineExtension.Instance.Delay(time, action));
    }

    #endregion Coroutine

}
