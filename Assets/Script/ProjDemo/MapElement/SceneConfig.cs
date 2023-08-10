using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景配置脚本
/// </summary>
public class SceneConfig : MonoBehaviour
{

    [SerializeField] int sceneID;
    public int SceneID
    {
        get { return sceneID; }
    }

    [SerializeField] GameObject sceneRoot;
    public GameObject SceneRoot { get { return sceneRoot; } }
    [SerializeField] GameObject unitRoot;
    public GameObject UnitRoot { get { return unitRoot; } }

    public bool needSetSceneActive;
    public bool sceneActiveState;

    #region 地图参数

    [Header("SpawnPoint_Param")]
    [SerializeField]SpawnPointConfig _spawnConfig;//出怪点配置
    public SpawnPointConfig spawnConfig { get { return _spawnConfig; } }

    #region crystal_param

    [SerializeField] List<SceneCrystal> redCrystalList = new List<SceneCrystal>();
    [SerializeField] List<SceneCrystal> blueCrystalList = new List<SceneCrystal>();

    Dictionary<int, Dictionary<int,SceneCrystal>> crystalDic = new Dictionary<int, Dictionary<int, SceneCrystal>>();

    public List<SceneCrystal>.Enumerator redCrystalListEnumerator
    {
        get
        {
            if (redCrystalList == null) redCrystalList = new List<SceneCrystal>();
            return redCrystalList.GetEnumerator();
        }
    }

    public List<SceneCrystal>.Enumerator blueCrystalListEnumerator
    {
        get
        {
            if (blueCrystalList == null) blueCrystalList = new List<SceneCrystal>();
            return blueCrystalList.GetEnumerator();
        }
    }

    #endregion crystal_param

    //测试
    public List<MapCell> spawnUnitCellList = new List<MapCell>();

    #endregion 地图参数

    private void Awake()
    {
        GameSceneManager.Instance.RegisterScene(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region 外部方法

    public void DoInit()
    {

    }

    public void ActiveScene(bool active)
    {
        if (active == sceneActiveState) return;

        sceneActiveState = active;

        EditorMapControl.Instance.SetElementRoot(sceneRoot);

        //初始化出怪点信息
        if (spawnConfig != null)
        {
            spawnConfig.DoInit();
        }

        MapManager.Instance.DoInit();//地图管理类初始化

        Debug.LogError("ActiveScene - id = " + SceneID + " , state = " + active);
    }

    public MapCell GetRandomSpawnCell()
    {
        if (spawnUnitCellList == null)
        {
            Debug.LogError("[SceneConfig]Get random spawn cell failed");
            return null;
        }


        int cellIndex = -1;
        int searchTimes = 0;
        do
        {
            cellIndex = UnityEngine.Random.Range(-1, spawnUnitCellList.Count);
            searchTimes++;

            if ((cellIndex < spawnUnitCellList.Count && cellIndex > 0) && spawnUnitCellList[cellIndex] != null)
            {
                Debug.LogError("Spawn point random index = " + cellIndex);
                return spawnUnitCellList[cellIndex];
            }

        } while (searchTimes < spawnUnitCellList.Count * 2);

        Debug.LogError("[SceneConfig]Get random spawn cell failed,plz check is element null reference?");
        return null;
    }

    public MapCell GetSpawnCellWithID(int id)
    {
        if (spawnUnitCellList == null)
        {
            Debug.LogError("[SceneConfig]SpawnUnitCellList is null,get spawn cell failed");
            return null;
        }

        for(int index = 0; index < spawnUnitCellList.Count; index++)
        {
            if (spawnUnitCellList[index] == null) continue;
            if (spawnUnitCellList[index].cellID == id)
            {
                return spawnUnitCellList[index];
            }
        }

        Debug.LogError("[SceneConfig]Get spawn cell failed , cellId = " + id);
        return null;
    }

    public void RegisterCrystal(SceneCrystal crystal)
    {
        if (crystal == null) return;
        if (crystalDic == null) crystalDic = new Dictionary<int, Dictionary<int,SceneCrystal>>();

        int groupIndex = (int)crystal.Group;
        if (crystalDic.ContainsKey(groupIndex))
        {
            if (crystalDic[groupIndex] == null)
            {
                crystalDic[groupIndex] = new Dictionary<int, SceneCrystal>();
            }

            if (crystalDic[groupIndex].ContainsKey(crystal.CrystalID))
            {
                Debug.LogError("[RegisterCrystal]register crystal failed, already had same id ,id = " + crystal.CrystalID);
                return;
            }

            crystalDic[groupIndex].Add(crystal.CrystalID, crystal);
        }
    }

    public void UnregisterCrystal(SceneCrystal crystal)
    {
        if (crystal == null) return;
    }

    public void UnregisterCrystal(E_Group group, int id)
    {
        int groupIndex = (int)group;
        Dictionary<int, SceneCrystal> tempDic = null;
        if(crystalDic.TryGetValue(groupIndex,out tempDic) == false)
        {
            return;
        }

        if (tempDic.ContainsKey(id))
        {
            tempDic.Remove(id);
        }
    }

    /// <summary>
    /// 根据指定id获取
    /// </summary>
    /// <param name="group"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public SceneCrystal GetCrystal(E_Group group,int id)
    {
        int groupIndex = (int)group;
        Dictionary<int, SceneCrystal> tempDic = null;
        if (crystalDic.TryGetValue(groupIndex, out tempDic) == false)
        {
            return null;
        }

        if (tempDic.ContainsKey(id))
        {
            return tempDic[id];
        }

        return null;
    }

    /// <summary>
    /// 随机获取
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public SceneCrystal GetRandomCrystal(E_Group group)
    {
        int groupIndex = (int)group;
        Dictionary<int, SceneCrystal> tempDic = null;
        if(crystalDic.TryGetValue(groupIndex,out tempDic) == false)
        {
            return null;
        }

        if (tempDic.Count == 0) return null;
        int randomIndex = UnityEngine.Random.Range(0, tempDic.Count);

        Dictionary<int, SceneCrystal>.Enumerator enumerator = tempDic.GetEnumerator();

        int count = 0;
        SceneCrystal crystal = null;
        while (enumerator.MoveNext() && count <= randomIndex)
        {
            count++;
            crystal = enumerator.Current.Value;
        }

        return crystal;
    }

    public Dictionary<int,SceneCrystal>.Enumerator GetEnumeratorWithGroup(E_Group group)
    {
        if (crystalDic == null) return (new Dictionary<int, SceneCrystal>()).GetEnumerator();

        Dictionary<int, SceneCrystal> tempDic = null;
        if (crystalDic.TryGetValue((int)group, out tempDic) == false)
        {
            return (new Dictionary<int, SceneCrystal>()).GetEnumerator();
        }
        return tempDic.GetEnumerator();
    }

    #endregion 外部方法


    #region test_method

    [ContextMenu("sort map cell")]
    public void SortMapCellWithZoneID()
    {
        if (SceneRoot == null)
        {
            Debug.LogError("SceneRoot is null, sort map cell failed");
            return;
        }

        Dictionary<int, ZoneMapCellRoot> mapCellRootDic = new Dictionary<int, ZoneMapCellRoot>();
        ZoneMapCellRoot[] mapCellRoots = this.gameObject.GetComponentsInChildren<ZoneMapCellRoot>();
        for(int index = 0; index < mapCellRoots.Length; index++)
        {
            if (mapCellRootDic.ContainsKey(mapCellRoots[index].zoneID) == false)
            {
                mapCellRootDic.Add(mapCellRoots[index].zoneID, mapCellRoots[index]);
            }
            else
            {
                Debug.LogError("[SceneConfig]是否MapCell根节点设置了相同zoneID，请检查, zoneID = " + mapCellRootDic[index].zoneID);
            }
        }

        MapCell[] cells = this.gameObject.GetComponentsInChildren<MapCell>();
        for(int index = 0; index < cells.Length; index++)
        {
            MapCell cur = cells[index];
            if (cur == null) continue;

            if (mapCellRootDic.ContainsKey(cur.zoneID))
            {
                cur.transform.SetParent(mapCellRootDic[cur.zoneID].transform);
            }
            else
            {
                GameObject newRoot = new GameObject("MapCellRoot_Zone_" + cur.zoneID);
                newRoot.transform.SetParent(SceneRoot.transform);
                newRoot.transform.localPosition = Vector3.zero;
                newRoot.transform.localRotation = Quaternion.identity;
                newRoot.transform.localScale = Vector3.one;

                ZoneMapCellRoot cellRoot = newRoot.AddComponentOnce<ZoneMapCellRoot>();
                if (cellRoot == null)
                {
                    Debug.LogError("Add component ZoneMapCellRoot failed");
                    DestroyImmediate(newRoot);
                    continue;
                }

                cellRoot.zoneID = cur.zoneID;

                mapCellRootDic.Add(cellRoot.zoneID, cellRoot);

                cur.transform.SetParent(cellRoot.transform);
            }

        }
    }

    public LayerMask attackMask;
    public GameObject c;
    [ContextMenu("Layer test")]
    public void LayerTest()
    {
        int layerDetect = attackMask.value & (1 << c.layer);
        Debug.LogError("layerDetect = " + layerDetect + ", attackMask.value = " + attackMask.value + " , go layer = " + (1<<c.gameObject.layer));
    }

    #endregion test_method

}
