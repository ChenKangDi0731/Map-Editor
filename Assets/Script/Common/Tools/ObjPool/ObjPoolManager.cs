using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メモリープール
/// </summary>
[ExecuteInEditMode]
public class ObjPoolManager : MonoBehaviour
{
    private static ObjPoolManager instance;
    public static ObjPoolManager Instance
    {
        get { return instance; }
    }

    #region gameObj_pool_param

    public GameObject poolCellRoot;

    public List<ObjPoolPreloadData> preloadDataList = new List<ObjPoolPreloadData>();

    public Dictionary<int, ObjPool> poolDic = new Dictionary<int, ObjPool>();

    public List<IObjPoolCell> createdPoolCellList = new List<IObjPoolCell>();

    #endregion gameObj_pool_param

    #region lifeCycle

    private void Awake()
    {
        instance = this;
    }

    #endregion lifeCycle

    #region 外部方法

    public void DoInit(GameObject goRoot=null)
    {
        poolCellRoot = this.gameObject;

        if (preloadDataList != null)
        {
            for(int index = 0; index < preloadDataList.Count; index++)
            {
                if (preloadDataList[index] == null) continue;

                RegisterObjPool(preloadDataList[index]);
            }

        }

        if (goRoot != null)
        {
            this.transform.SetParent(goRoot.transform);
        }

    }
    public void RegisterObjPool(GameObject go,int initSize = 0)
    {
        if (go == null)
        {
            return;
        }

        string poolName = go.name;
        int nameHashID = -1;
        if (poolName.Contains(".prefab"))
        {
            poolName = poolName.Remove(poolName.IndexOf(".prefab"));
        }

        nameHashID = poolName.GetHashCode();

        if (poolDic.ContainsKey(nameHashID))
        {
            return;
        }

        ObjPool pool = new ObjPool(go, poolName, nameHashID, poolCellRoot.transform, initSize);

        poolDic.Add(nameHashID, pool);

    }

    public void RegisterObjPool(ObjPoolPreloadData data)
    {
        if (data == null) return;
        RegisterObjPool(data.prefab, data.PreloadCount);
    }


    public ObjPoolCell GetCell(int nameHashCode)
    {
        ObjPool pool = null;
        if (poolDic.TryGetValue(nameHashCode, out pool) == false)
        {
            Debug.LogError("对象池不存在，获取对象池资源失败");
            return null;
        }

        return pool.GetCell();
    }


    public ObjPoolCell GetCell(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("名前存在しない");
            return null;
        }

        int nameHashCode = name.GetHashCode();

        ObjPool pool = null;
        if(poolDic.TryGetValue(nameHashCode,out pool) == false)
        {
            return null;
        }

        return pool.GetCell();

    }

    public ObjPoolCell GetCell(AssetInfo info)
    {
        if (info == null) return null;
        ObjPoolCell cell = GetCell(info.NameWithoutExtension);
        if (cell == null)
        {
            GameObject prefabGO = Resources.Load<GameObject>(info.FullPathWithoutExtension);
            if (prefabGO == null)
            {
                return null;
            }
            RegisterObjPool(prefabGO);
        }
        
        if (cell == null)
        {
            cell = GetCell(info.NameWithoutExtension);
        }

        return cell;
    }

    /// <summary>
    /// 回收资源
    /// </summary>
    /// <param name="cell"></param>
    public void Recycle(ObjPoolCell cell)
    {
        if (cell == null) return;
        ObjPool pool = null;
        if (poolDic.TryGetValue(cell.prefabsId,out pool) == false)
        {
            pool = new ObjPool(cell.gameObject, poolCellRoot.transform);
        }

        pool.RecycleCell(cell);

    }

    public void Recycle(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        ObjPoolCell cell = go.GetComponent<ObjPoolCell>();
        if (cell != null)
        {
            cell.Recycle();
            return;
        }

        go.transform.SetParent(null);

        GameObject.DestroyImmediate(go);
    }

    /// <summary>
    /// Idでオブジェクトをリサイクルする
    /// </summary>
    /// <param name="assetID"></param>
    public void RecycleCellByAssetID(int assetID)
    {
        if (createdPoolCellList == null || createdPoolCellList.Count == 0) return;
        List<IObjPoolCell> tempCellList = new List<IObjPoolCell>(createdPoolCellList.ToArray());
        for(int index = 0; index < tempCellList.Count; index++)
        {
            if (tempCellList[index] == null) continue;
            if (tempCellList[index].GetAssetID() == assetID)
            {
                tempCellList[index].Recycle();
            }
        }
    }

    /// <summary>
    /// リサイクル
    /// </summary>
    public void RecycleAll()
    {
        if (createdPoolCellList == null || createdPoolCellList.Count == 0) return;
        List<IObjPoolCell> tempCellList = new List<IObjPoolCell>(createdPoolCellList.ToArray());
        for(int index = 0; index < tempCellList.Count; index++)
        {
            if (tempCellList[index] == null) continue;
            tempCellList[index].Recycle();
        }

        createdPoolCellList.Clear();
    }

    /// <summary>
    /// プールに入れる
    /// </summary>
    /// <param name="cell"></param>
    public void RegisterCreatedCell(IObjPoolCell cell)
    {
        if (cell == null) return;
        createdPoolCellList.Add(cell);
    }

    /// <summary>
    /// プールに入れる
    /// </summary>
    /// <param name="cell"></param>
    public void RegisterCreatedCell(ObjPoolCell cell)
    {
        if (cell == null) return;
        createdPoolCellList.Add(cell);
    }

    /// <summary>
    /// プールから削除する
    /// </summary>
    /// <param name="cell"></param>
    public void UnRegisterCreatedCell(IObjPoolCell cell)
    {
        if (cell == null) return;
        if (createdPoolCellList.Contains(cell))
        {
            createdPoolCellList.Remove(cell);
        }
    }

    /// <summary>
    /// プールから削除する
    /// </summary>
    /// <param name="cell"></param>
    public void UnRegisterCreatedCell(ObjPoolCell cell)
    {
        if (cell == null) return;
        if (createdPoolCellList.Contains(cell))
        {
            createdPoolCellList.Remove(cell);
        }
    }


    public void ResetObjPool()
    {

    }

    #endregion 外部方法

}

[System.Serializable]
public class ObjPoolPreloadData
{
    public GameObject prefab;
    public int PreloadCount;
}
