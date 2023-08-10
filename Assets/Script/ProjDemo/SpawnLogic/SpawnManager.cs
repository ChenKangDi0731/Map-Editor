using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

/// <summary>
/// キャラクター生成マネージャー（ゲームモード
///　＊ゲームモードはマップをテストするために実装したものです＊
/// </summary>
public class SpawnManager : Singleton<SpawnManager>
{
    SpawnPointConfig config;
    Dictionary<int, SpawnPointData> spawnPointDataDic;

    #region 外部方法
    public void DoInit()
    {

        if (GameSceneManager.Instance.curScene != null)
        {
            config = GameSceneManager.Instance.curScene.spawnConfig;
            if (config == null)
            {
                Debug.LogError("[SpawnManager]cur scene spawnConfig is null");
                return;
            }
        }

        spawnPointDataDic = new Dictionary<int, SpawnPointData>();

    }

    public Unit SpawnUnitManual(TestSpawnData data)
    {
        if (data == null || config == null)
        {
            Debug.LogError("[SpawnManager]spawn failed");
            return null;
        }
        SpawnPointData pointData = null;
        if (spawnPointDataDic.ContainsKey(data.spawnPointID))
        {
            pointData = spawnPointDataDic[data.spawnPointID];
        }
        else
        {

            SpawnPointInfo info = config.GetSpawnInfoWithID(data.spawnPointID);
            if (info != null)
            {
                pointData = info.GetTestSpawnPointData(data.spawnType, data.spawn_limit);

                if (spawnPointDataDic.ContainsKey(pointData.spawnPointID))
                {
                    Debug.LogError("[SpawnManager]pointDataID already exist,plz check , id = " + data.spawnPointID);
                    return null;
                }
                pointData.StartSpawn();
                spawnPointDataDic.Add(pointData.spawnPointID, pointData);
            }
        }

        if (pointData != null && pointData.CanSpawn())
        {
            SpawnPointInfo info = pointData.GetSpawnPoint();
            if (info == null)
            {
                Debug.LogError("[SpawnManager]Spawn failed, spawnPoint is null, pointDataId = " + pointData.spawnPointID);
                return null;
            }

            Unit u = UnitManager.Instance.CreateUnit(data.spawnUnitPrefabID,data.group);
            if (u == null)
            {
                Debug.LogError("[SpawnManager]Spawn unit failed");
                return null;
            }
            if (u.unitAI.BtOwner != null)
            {
                IBlackboard bb = u.unitAI.BtOwner.blackboard;
                if (bb != null)
                {
                    if (bb.IsContainsVariables(GameDefine.spawnPointStr))
                    {
                        bb.SetVariableValue(GameDefine.spawnPointStr, info);
                    }
                    else
                    {
                        bb.AddVariable<SpawnPointInfo>(GameDefine.spawnPointStr, info);
                    }
                }
            }

            u.unitTrans.position = info.pointTrans.position;
            u.unitTrans.forward = info.pointTrans.forward;

            u.unitAI.EnableAI(data.enableAIAtFirst);

            pointData.curSpawnCount++;

            return u;
        }

        return null;
    }

    public void OverrideSpawnPointData()
    {

    }

    public void SpawnUnitManual(List<TestSpawnData> dataList)
    {

    }

    #region data_operate

    public void UpdateSpwanData(TestSpawnData data)
    {

    }

    public void UpdateDataUnitCount(int pointId,bool isUnitInactive)
    {
        if (spawnPointDataDic == null) return;

        SpawnPointData data = null;
        if(spawnPointDataDic.TryGetValue(pointId,out data) == false)
        {
            Debug.LogError("[SpawnManager]Update spawn count failed, pointId = " + pointId);
            return;
        }

        if (data == null)
        {
            Debug.LogError("[SpawnManager]Update spawn count failed, pointId = " + pointId);
            return;
        }

        if (isUnitInactive)
        {
            data.curSpawnCount--;
        }
        else
        {

        }
    }

    public void UpdateDataUnitCount(SpawnPointInfo info,bool isUnitInactive)
    {
        if (spawnPointDataDic == null || info == null) return;

        SpawnPointData data = null;
        if (spawnPointDataDic.TryGetValue(info.PointID, out data) == false)
        {
            Debug.LogError("[SpawnManager]Update spawn count failed, pointId = " + info.PointID);
            return;
        }

        if (data == null)
        {
            Debug.LogError("[SpawnManager]Update spawn count failed, pointId = " + info.PointID);
            return;
        }

        if (isUnitInactive)
        {
            data.curSpawnCount--;
        }
        else
        {

        }
    }

    #endregion data_operate

    #endregion 外部方法
}
