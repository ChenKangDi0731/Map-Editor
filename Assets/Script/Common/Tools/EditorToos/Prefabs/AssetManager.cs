using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{
    #region AssetInfoTableでアセットを獲得し、メモリープールに保存する

    public GameObject Create(int assetID,Transform parent = null)
    {
        //find asset in objPool
        AssetInfo info = AssetsRegistrar.Instance.GetAssetInfo(assetID);
        if (info == null)
        {
            Debug.LogError("[AssetManager]生成エラーが発生，id = " + assetID);
            return null;
        }

        //find assetInfo in AssetsRegistrar
        ObjPoolCell cell = ObjPoolManager.Instance.GetCell(info);
        if (cell == null)
        {
            Debug.LogError("[AssetManager]メモリープール存在しない、生成エラーが発生 ， id = " + assetID);
            return null;
        }

        if (parent != null)
        {
            cell.gameObject.transform.SetParent(parent);
        }

        return cell.gameObject;
    }

    public GameObject Create(int assetID, Action<GameObject> callback)
    {
        //find asset in objPool
        AssetInfo info = AssetsRegistrar.Instance.GetAssetInfo(assetID);
        if (info == null)
        {
            Debug.LogError("[AssetManager]生成エラーが発生，id = " + assetID);
            return null;
        }

        //find assetInfo in AssetsRegistrar
        ObjPoolCell cell = ObjPoolManager.Instance.GetCell(info);
        if (cell == null)
        {
            Debug.LogError("[AssetManager]メモリープール存在しない、生成エラーが発生 ， id = " + assetID);
            return null;
        }

        callback?.Invoke(cell.gameObject);

        return cell.gameObject;
    }

    public GameObject Create(int assetID, Transform parent, Action<GameObject> callback)
    {
        //find asset in objPool
        AssetInfo info = AssetsRegistrar.Instance.GetAssetInfo(assetID);
        if (info == null)
        {
            Debug.LogError("[AssetManager]生成エラーが発生，id = " + assetID);
            return null;
        }

        //find assetInfo in AssetsRegistrar
        ObjPoolCell cell = ObjPoolManager.Instance.GetCell(info);
        if (cell == null)
        {
            Debug.LogError("[AssetManager]メモリープール存在しない、生成エラーが発生 ， id = " + assetID);
            return null;
        }

        if (parent != null)
        {
            cell.gameObject.transform.SetParent(parent);
        }
        cell.gameObject.transform.localPosition = Vector3.zero;
        cell.gameObject.transform.localRotation = Quaternion.identity;

        callback?.Invoke(cell.gameObject);

        return cell.gameObject;
    }
    #endregion AssetInfoTableでアセットを獲得し、メモリープールに保存する

    #region オブジェクトを削除

    public void Destory(GameObject go)
    {
        ObjPoolManager.Instance.Recycle(go);
    }

    public void Destory(GameObject go, Action<GameObject> callback)
    {

    }

    public void DestoryObjByID(int assetID)
    {
        ObjPoolManager.Instance.RecycleCellByAssetID(assetID);
    }

    #endregion オブジェクトを削除

}
