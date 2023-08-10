using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メモリープール
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjPoolBase<T> where T:IBaseObjPoolCell,new()
{

    private static ObjPoolBase<T> poolInstance;
    public static ObjPoolBase<T> instance
    {
        get
        {
            if (poolInstance == null) poolInstance = new ObjPoolBase<T>(typeof(T).Name, 10, 3);
            return poolInstance;
        }
    }

    public Queue<IBaseObjPoolCell> cellQueue;

    private string objPoolName;
    public string name
    {
        get { return objPoolName; }
    }

    private string poolName;
    public string Name { get { return name + "Pool"; } }

    private int nameHashID;
    public int hashID
    {
        get { return nameHashID; }
    }

    private int maxPoolCount;

    public int PoolCount
    {
        get
        {
            if (cellQueue == null) return 0;
            return cellQueue.Count;
        }
    }

    public ObjPoolBase() { poolInstance = this; }

    public ObjPoolBase(string name, int maxCount = 10, int initSize = 0) : this()
    {
        cellQueue = new Queue<IBaseObjPoolCell>();
        InitPool(name, maxCount, initSize);
    }

    public void InitPool(string name, int maxCount = 10,int initSize = 0)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("メモリープール初期化エラー, " + typeof(T).Name);
            return;
        }

        objPoolName = name;

        maxPoolCount = maxCount;

        EnlargePoolSize(initSize);
    }

    public void EnlargePoolSize(int size)
    {
        if (maxPoolCount <= PoolCount)
        {
            return;
        }

        if (maxPoolCount < (PoolCount + size))
        {
            size = maxPoolCount - PoolCount;
        }

        for(int index = 0; index < size; index++)
        {
            IBaseObjPoolCell newObj = new T() as IBaseObjPoolCell;
            cellQueue.Enqueue(newObj);
        }

    }

    public T GetCell()
    {
        if (cellQueue == null)
        {
            cellQueue = new Queue<IBaseObjPoolCell>();
        }

        if (cellQueue.Count == 0)
        {
            EnlargePoolSize(1);
        }

        if (cellQueue.Count == 0)
        {
            Debug.LogError("メモリープール生成エラーが発生，objPool Name = " + Name);
            return default(T);
        }

        IBaseObjPoolCell cell = cellQueue.Dequeue();
        if (cell != null)
        {
            cell.Reset();
        }

        return (T)cell;
    }

    public void RecycleCell(T cell)
    {
        if (cell == null) return;

        if (maxPoolCount <= PoolCount)
        {
            return;
        }

        IBaseObjPoolCell c = cell as IBaseObjPoolCell;
        cell.Reset();
        cellQueue.Enqueue(c);


        Debug.LogError("pool count increase,count = " + PoolCount +" , "+typeof(T).Name);
    }

    public void RecycleCell(IBaseObjPoolCell cell)
    {
        if (cell == null) return;
        if (maxPoolCount <= PoolCount)
        {
            return;
        }

        cell.Reset();
        cellQueue.Enqueue(cell);
    }


    #region test_method

    public void CheckInstanceName()
    {
        if (poolInstance == null)
        {
            Debug.LogError("Instance is null , " + this.GetType().Name);
            return;
        }

        Debug.LogError("instance name = " + poolInstance.GetType().Name + " " + typeof(T).Name);
    }

    #endregion test_method
}

//test
public class BasePoolData<T>
{
    public BasePoolData() { }
}

public class TestBaseData1 : IBaseObjPoolCell
{

    public void Reset()
    {

    }

    public void Recycle()
    {
        ObjPoolBase<TestBaseData1>.instance.RecycleCell(this);
        Debug.LogError("recycle testBaseData1 obj");
    }

    ~TestBaseData1()
    {
        Recycle();
    }
}

public class TestBaseData2 : IBaseObjPoolCell
{
    public void Reset()
    {

    }

    public void Recycle()
    {
        ObjPoolBase<TestBaseData2>.instance.RecycleCell(this);
        Debug.LogError("recycle testBaseData2 obj");
    }

    ~TestBaseData2()
    {
        Recycle();
    }
}

