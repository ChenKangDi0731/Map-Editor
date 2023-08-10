using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObjPoolManager:Singleton<BaseObjPoolManager>
{
    public T GetObj<T>() where T : IBaseObjPoolCell, new()
    {
        return ObjPoolBase<T>.instance.GetCell();
    }

    /// <summary>
    /// メモリープール初期化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="maxSize"></param>
    /// <param name="initSize"></param>
    public void InitBaseObjPool<T>(string name,int maxSize=10,int initSize = 0) where T : IBaseObjPoolCell, new()
    {

        ObjPoolBase<T>.instance.InitPool(name, maxSize, initSize);

    }

    public void InitBaseObjPool<T>(int maxSize=10,int initSize=0)where T : IBaseObjPoolCell, new()
    {
        ObjPoolBase<T>.instance.InitPool(typeof(T).Name, maxSize, initSize);
    }
}
