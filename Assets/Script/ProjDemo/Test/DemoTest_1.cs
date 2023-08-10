using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;

public class DemoTest_1 : MonoBehaviour
{

    public int sceneSpawnPointIndex = -1;
    public MapCell curSpawnCell;

    public int testPrefabsId;



    public void DoInit()
    {
        if (GameSceneManager.Instance.curScene != null)
        {
            if (sceneSpawnPointIndex > 0)
            {
                curSpawnCell = GameSceneManager.Instance.curScene.GetSpawnCellWithID(sceneSpawnPointIndex);
            }
            else
            {
                curSpawnCell = GameSceneManager.Instance.curScene.GetRandomSpawnCell();
            }
            if (curSpawnCell == null)
            {
                Debug.LogError("[DemoTest_1]Get spawn cell failed ,plz check , spawnPointIndex = " + sceneSpawnPointIndex);
            }
        }
        else
        {
            Debug.LogError("[DemoTest_1]Init test script failed , sceneInstance is null");
        }
    }
    #region 生命周期

    private void Awake()
    {
        GameConfig.Instance.testScript = this;
    }

    private void Update()
    {
        
    }

    private void OnGUI()
    {
        if (GameConfig.Instance.isEditMode == false)
        {
            if (GUI.Button(new Rect(20, 60, 200, 30), "Create Dragon"))
            {
                //CreateTestMonster();
                CreateTestMonster2();
            }
            else if (GUI.Button(new Rect(20, 20, 200, 30), "Create Monster"))
            {
                CreateTestMonster();
            }
        }
    }

    #endregion 生命周期

    [Header("敵の生成に使用するデータ")]
    public int spawnPointID;
    public int spawnUnitPrefabsID;
    public int spawnLimit;
    public SpawnPointConfig.E_SpawnType spawnType;
    public bool enableAIAtFirst;

    [Header("start point")]
    public SpawnPointConfig.E_SpawnType spawnType_1;
    public int spawnUnitPrefabsID_1;
    public int spawnLimit_1;
    void CreateTestMonster()
    {
        TestSpawnData data = new TestSpawnData();
        int randomSpawnPoint = UnityEngine.Random.Range(1, 4);
        data.group = E_Group.Group_1;
        data.spawnType = spawnType_1;
        data.spawnPointID = randomSpawnPoint;
        data.spawnUnitPrefabID = spawnUnitPrefabsID_1;
        data.spawn_limit = spawnLimit_1;

        data.enableAIAtFirst = true;

        SpawnManager.Instance.SpawnUnitManual(data);
    }

    void CreateTestMonster2()
    {
        TestSpawnData data = new TestSpawnData();
        data.spawnPointID = spawnPointID;
        data.spawnUnitPrefabID = spawnUnitPrefabsID;
        data.spawn_limit = spawnLimit;
        data.group = E_Group.Group_2;
        data.spawnType = spawnType;

        data.enableAIAtFirst = enableAIAtFirst;

        SpawnManager.Instance.SpawnUnitManual(data);
    }
}
