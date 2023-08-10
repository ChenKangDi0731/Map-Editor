using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using E_SpawnType = SpawnPointConfig.E_SpawnType;


public class SpawnPointInfo : MonoBehaviour
{
    #region component

    private GameObject _gameObject;
    public GameObject pointGo
    {
        get
        {
            if (_gameObject == null) _gameObject = this.gameObject;
            return _gameObject;
        }
    }

    private Transform _transform;
    public Transform pointTrans
    {
        get
        {
            if (_transform == null) _transform = pointGo.transform;
            return _transform;
        }
    }

    #endregion component

    public E_Group group;
    public bool useCell = true;
    public int PointID;
    public int bindCellID;
    public SpawnPointData GetSpawnPointData(E_SpawnType spawnType)
    {
        return null;
    }

    #region 测试方法
    
    public SpawnPointData GetTestSpawnPointData(E_SpawnType spawnType,int spawnLimit)
    {
        SpawnPointData data = new SpawnPointData();
        data.FillTestParam(this, PointID, spawnType, group, spawnLimit);

        return data;
    }

    #endregion 测试方法
}

public class SpawnPointData
{
    bool spawnState;

    public E_SpawnType spawnType = E_SpawnType.None;

    public E_Group group;

    SpawnPointInfo parentSpawnPoint;

    public int spawnPointID
    {
        get;
        private set;
    }
    int spawn_limit;

    int _curSpawnCount;//当前点Spawn数量
    public int curSpawnCount
    {
        get { return _curSpawnCount; }
        set
        {
            if (value < 0)
                _curSpawnCount = 0;
            else
                _curSpawnCount = value;
        }
    }

    //auto
    //public Dictionary<float, int> spawnDataTimeDic = new Dictionary<float, int>();
    public List<SpawnData> spawnDataList = new List<SpawnData>();
    public int curSpawnIndex;
    public float curSpawnTime;

    public SpawnPointData() { }

    #region method
    public SpawnPointInfo GetSpawnPoint()
    {
        return parentSpawnPoint;
    }

    public bool CanSpawn()
    {
        return (spawnState && (curSpawnCount < spawn_limit || spawn_limit < 0));
    }

    void SetSpawnState(bool state)
    {
        spawnState = state;
    }

    public void StopSpawn()
    {
        SetSpawnState(false);
        ResetTimer();
        ResetSpawnData();
    }

    public void StartSpawn()
    {
        SetSpawnState(true);
        ResetTimer();
        ResetSpawnData();
    }

    public void PauseSpawn()
    {
        SetSpawnState(false);
    }

    public void ContinueSpawn()
    {
        SetSpawnState(true);
    }

    public void ResetTimer()
    {
        curSpawnTime = 0f;
    }

    public void ResetSpawnData()
    {
        curSpawnIndex = 0;
        if ((spawnType == E_SpawnType.SettingAutoOnce || spawnType == E_SpawnType.SettingAutoLoop) && spawnDataList != null)
        {
            for (int index = 0; index < spawnDataList.Count; index++)
            {
            }
        }

        curSpawnCount = 0;
    }

    #endregion method

    #region 测试方法

    public void FillTestParam(SpawnPointInfo info, int pointID,E_SpawnType type,E_Group g, int limit)
    {
        parentSpawnPoint = info;
        group = g;
        spawnType = type;
        spawnPointID = pointID;
        spawn_limit = limit;
        ResetSpawnData();
    }

    #endregion 测试方法
}

public class SpawnData {

}

//test data

public class TestSpawnData
{
    public int spawnPointID;
    public int spawnUnitPrefabID;
    public E_Group group;
    public E_SpawnType spawnType;
    public int spawn_limit;

    //auto spawn param
    public bool useSpawnInterval = false;
    public float spawnInterval = 0f;

    //other param
    public bool enableAIAtFirst;

}

