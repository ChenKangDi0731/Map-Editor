using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjExtension
{
    public static T AddComponentOnce<T>(this GameObject go)where T:MonoBehaviour
    {
        T returnComponent = go.GetComponent<T>();
        if (returnComponent != null)
        {
            MonoBehaviour.DestroyImmediate(returnComponent);
            returnComponent = null;
        }

        returnComponent = go.AddComponent<T>();
        return returnComponent;
    }

    public static void MapAllComponent<T>(this GameObject go, Action<T> action, bool includeRoot = false)
    {
        if (go == null || action == null) return;

        T[] tempArray;

        tempArray = go.GetComponentsInChildren<T>();
        if(tempArray==null || tempArray.Length == 0)
        {
            return;
        }

        T rootComponent = go.GetComponent<T>();

        for(int index = 0; index < tempArray.Length; index++)
        {
            if (tempArray[index] == null) continue;
            //if (includeRoot == false && rootComponent.ToString() == tempArray[index].ToString()) Debug.LogError("1. = "+rootComponent.ToString()+", 2. = "+tempArray[index].ToString());

            action?.Invoke(tempArray[index]);
        }
    }

    public static void SetGOActive(this GameObject go,bool active)
    {
        if (go == null) return;

        if (go.activeSelf != active)
        {
            go.SetActive(active);
        }
    }
}
