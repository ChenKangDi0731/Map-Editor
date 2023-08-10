using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メモリープール
/// </summary>
public class ObjPool
{
    private GameObject poolGO;
    private Transform poolGoRootT;

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

    private Queue<ObjPoolCell> cellQueue;

    public int PoolCount {
        get
        {
            if (cellQueue == null) return 0;
            return cellQueue.Count;
        }
    }

    public ObjPool(GameObject go, string n, int hashID, Transform root, int initSize = 0)
    {
        if (go == null)
        {
            Debug.LogError("メモリープール作成エラー");
            return;
        }

        if (string.IsNullOrEmpty(n))
        {
            Debug.LogError("メモリープール作成エラー");

            return;
        }

        poolGO = go;
        poolGoRootT = root;
        objPoolName = n;
        nameHashID = hashID;

        cellQueue = new Queue<ObjPoolCell>();

        EnlargePoolSize(initSize);

        //Debug.LogErrorFormat("[ObjPool] 创建对象池， name = {0},count = {1}", Name, PoolCount);
    }

    public ObjPool(GameObject go,Transform root, int initSize = 1)
    {
        if (go == null)
        {
            Debug.LogError("メモリープール作成エラー");

            return;
        }

        poolGO = go;
        poolGoRootT = root;
        string n = go.name;
        if (n.Contains(".prefab"))
        {
            n = n.Remove(name.IndexOf(".prefab"));
        }
        objPoolName = n;
        nameHashID = name.GetHashCode();

        cellQueue = new Queue<ObjPoolCell>();

        EnlargePoolSize(initSize);

        Debug.LogErrorFormat("[ObjPool] メモリープール作成， name = {0},count = {1}", Name, PoolCount);
    }

    void EnlargePoolSize(int size)
    {
        if (poolGO == null) return;

        for(int index = 0; index < size; index++)
        {
            GameObject newGO = GameObject.Instantiate(poolGO, poolGoRootT);
            ObjPoolCell cell= newGO.AddComponentOnce<ObjPoolCell>();
            if (cell != null)
            {
                cell.prefabsId = hashID;
                cell.gameObject.transform.SetParent(poolGoRootT);
                cell.gameObject.SetActive(false);
            }
            cellQueue.Enqueue(cell);
        }
    }

    public ObjPoolCell GetCell()
    {
        if (cellQueue.Count == 0)
        {
            //create cell
            EnlargePoolSize(1);
        }

        if (cellQueue.Count == 0)
        {
            Debug.LogError("メモリープール獲得エラー，objPool Name = " + Name);
            return null;
        }

        ObjPoolCell cell = cellQueue.Dequeue();
        if (cell != null)
        {
            cell.gameObject.SetActive(true);
            cell.gameObject.transform.SetParent(poolGoRootT);
        }

        ObjPoolManager.Instance.RegisterCreatedCell(cell);

        return cell;
    }

    public void RecycleCell(ObjPoolCell cell)
    {
        if (cell == null) return;

        cell.gameObject.transform.SetParent(poolGoRootT);
        cell.gameObject.SetActive(false);

        ObjPoolManager.Instance.UnRegisterCreatedCell(cell);

        cell.transform.localPosition = Vector3.zero;
        cell.transform.rotation = Quaternion.identity;
        cell.transform.localScale = Vector3.one;

        cellQueue.Enqueue(cell);
    }

}
