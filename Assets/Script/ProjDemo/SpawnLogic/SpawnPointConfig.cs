using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointConfig : MonoBehaviour
{

    public List<SpawnPointInfo> spawnPointInfoList = new List<SpawnPointInfo>();

    Dictionary<int, SpawnPointInfo> spawnPointDic;

    public void DoInit()
    {
        if (spawnPointInfoList != null)
        {
            if (spawnPointDic == null) spawnPointDic = new Dictionary<int, SpawnPointInfo>();
            spawnPointDic.Clear();

            for(int index = 0; index < spawnPointInfoList.Count; index++)
            {
                SpawnPointInfo curInfo = spawnPointInfoList[index];

                if (curInfo == null) continue;
                if (spawnPointDic.ContainsKey(curInfo.PointID) == false)
                {
                    spawnPointDic.Add(curInfo.PointID, curInfo);
                }
                else
                {
                    Debug.LogError("[SpawnPointConfig]spawnPointInfo has same id ,id = " + curInfo.PointID);
                }
            }
        }

        Debug.LogError("Init spawn config");
    }

    public SpawnPointInfo GetSpawnInfoWithID(int pointId)
    {
        if (spawnPointDic == null)
        {
            Debug.LogError("[SpawnPointConfig]spawnPointDic is null,Get spawnInfo failed");
            return null;
        }

        SpawnPointInfo returnValue = null;
        if(spawnPointDic.TryGetValue(pointId,out returnValue) == false)
        {
            Debug.LogError("[SpawnPointConfig]Get spawnPoint failed, pointId = " + pointId);
        }

        return returnValue;

    }


    public enum E_SpawnType
    {
        None=-1,
        Manual=0,
        SettingAutoOnce=1,
        SettingAutoLoop=2,
    }


    #region test_method

    [Header("测试方法")]
    public bool setID = false;
    public SpawnPointInfo t1;
    public MapCell t2;
    [ContextMenu("Copy Transform(t1->t2)")]
    public void CopyTransform()
    {
        if (t1 == null || t2 == null)
        {
            return;
        }

        t1.transform.position = t2.GetRoadPosition();
        if (setID)
        {
            t1.bindCellID = t2.cellID;
        }
    }

    [ContextMenu("Clear Reference")]
    public void ClearReference()
    {
        t1 = null;
        t2 = null;
    }

    [ContextMenu("ConfigMethod/Get All Spawn Point Info")]
    public void GetAllSpawnPointInfo()
    {
        if (spawnPointInfoList == null) spawnPointInfoList = new List<SpawnPointInfo>();
        spawnPointInfoList.Clear();

        SpawnPointInfo[] infoArray = this.GetComponentsInChildren<SpawnPointInfo>();
        spawnPointInfoList.AddRange(infoArray);
    }

    #endregion test_method

}
