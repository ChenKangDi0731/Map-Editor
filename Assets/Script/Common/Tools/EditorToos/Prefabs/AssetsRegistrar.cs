using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アセット管理
/// </summary>
public class AssetsRegistrar : Singleton<AssetsRegistrar>
{

    Dictionary<int, AssetInfo> assetInfoDic;

    public void ReadAssetInfos()
    {
        if (assetInfoDic != null)
        {
            assetInfoDic.Clear();
            assetInfoDic = null;
        }

        XMLAssetsRegistrar.LoadPrefabsInfo(out assetInfoDic);

        if (assetInfoDic==null || assetInfoDic.Count==0)
        {
            return;
        }

        Debug.Log("[load AssetInfoTable] <color=#00ff00> asset count = " + assetInfoDic.Count+"</color>");

    }

    public AssetInfo GetAssetInfo(int assetID)
    {
        if (assetInfoDic == null) return null;

        AssetInfo info = null;

        if(assetInfoDic.TryGetValue(assetID,out info) == false)
        {
            return null;
        }

        return info;
    }


}

public class AssetInfo
{

    /// <summary>
    ///　プリハブID
    /// </summary>
    private int prefabId;
    public int ID { get { return prefabId; } }

    /// <summary>
    /// ID文字列
    /// </summary>
    public string IDStr { get { return ID.ToString(); } }

    /// <summary>
    /// 名前
    /// </summary>
    public string FullName { get { return NameWithoutExtension + Extension; } }

    /// <summary>
    /// 名前（拡張子なし）
    /// </summary>
    private string assetNameWithoutExtension;
    public string NameWithoutExtension { get { return assetNameWithoutExtension; } }

    /// <summary>
    /// アセット拡張子
    /// </summary>
    private string assetExtension;
    public string Extension { get { return assetExtension; } }

    /// <summary>
    /// パス（名前含む）
    /// </summary>
    private string assetPathWithoutName;
    public string PathWithoutName { get { return assetPathWithoutName; } }

    /// <summary>
    /// フール名前（パス
    /// </summary>
    public string FullPath { get { return PathWithoutName +"/"+ FullName; } }

    public string FullPathWithoutExtension { get { return PathWithoutName + "/" + NameWithoutExtension; } }

    public AssetInfo(int id)
    {
        prefabId = id;
    }

    #region データセッター

    public void SetAssetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogErrorFormat("<color=#00ff00> ---------- Asset name string is null ,id = {0} </color>", ID);
            return;
        }

        assetNameWithoutExtension = name;
    }

    public void SetAssetResourceExtension(string ext)
    {
        if (string.IsNullOrEmpty(ext))
        {
            Debug.LogErrorFormat("<color=#00ff00> ---------- Asset extension string is null ,id = {0} </color>", ID);
            return;
        }

        assetExtension = ext;
    }

    public void SetAssetPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogErrorFormat("<color=#00ff00> ---------- Asset path string is null ,id = {0} </color>", ID);
            return;
        }

        assetPathWithoutName = path;
    }


    #endregion データセッター


    //名前文字列のヌルチェック
    public bool CheckInfo()
    {
        if (string.IsNullOrEmpty(IDStr) || string.IsNullOrEmpty(assetNameWithoutExtension) || string.IsNullOrEmpty(assetExtension) || string.IsNullOrEmpty(assetPathWithoutName))
        {
            return false;
        }

        return true;
    }

}
