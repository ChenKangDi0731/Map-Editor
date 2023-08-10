using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : Singleton<MainController>
{

    GameObject testGO;

    #region lifeCycle

    public void DoInit()
    {
        GameConfig.Instance.DoInit();
        InputManager.Instance.DoInit(GameConfig.Instance.managerRoot);
        CameraManager.Instance.DoInit();

        ObjPoolManager.Instance.DoInit();//メモリープール初期化

        AssetsRegistrar.Instance.ReadAssetInfos();//アセット管理ツール初期化

        BattleManager.Instance.DoInit(); //バトルマネージャー
        CustomActionManager.Instance.DoInit();

        if (GameConfig.Instance.isEditMode)
        {
            MapEditorManager.Instance.DoInit(GameConfig.Instance.managerRoot);//マップエディターマネージャー初期化
            OperateDataManager.Instance.DoInit(30);//巻き戻し処理マネージャー初期化

            //ui
            if (GameConfig.Instance.mapEditUI != null)
            {
                GameConfig.Instance.mapEditUI.DoInit();
            }
        }
        else
        {
            //ui
            if (GameConfig.Instance.mapEditUI != null)
            {
                GameConfig.Instance.mapEditUI.ShowUI(false);
            }
        }

        GameSceneManager.Instance.DoInit();//シーン初期化

        SpawnManager.Instance.DoInit();//敵の生成を管理するマネージャー

        //测试
        if (GameConfig.Instance.testScript != null)
        {
            GameConfig.Instance.testScript.DoInit();
        }
    }

    public void DoUpdate(float deltaTime)
    {

        //巻き戻し B⁺Z
        //やり直し B+Y
        if (Input.GetKey(KeyCode.B))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                OperateDataManager.Instance.Undo();
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                OperateDataManager.Instance.Redo();
            }
        }

        //UI表示
        if (GameConfig.Instance.mapEditUI != null)
        {
            if (Input.GetKeyDown(KeyCode.J) && GameConfig.Instance.mapEditUI.IsUIShow() == false)
            {
                GameConfig.Instance.mapEditUI.ShowUI(true);
            }
            else if (Input.GetKeyDown(KeyCode.H) && GameConfig.Instance.mapEditUI.IsUIShow() == true)
            {
                GameConfig.Instance.mapEditUI.ShowUI(false);
            }

        }

        BattleManager.Instance.DoUpdate(Time.deltaTime);

        CustomActionManager.Instance.DoUpdate(Time.deltaTime);
    }

    public void DoFixedUpdate(float fixedUpdateTime)
    {
    }

    #endregion lifeCycle

}
