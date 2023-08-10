using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig : MonoBehaviour
{
    private static GameConfig _instance;
    public static GameConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject tempGO= GameObject.Find("GameConfig");
                if (tempGO == null)
                {
                    Debug.LogError("Get GameConfig failed");
                    return null;
                }

                _instance = tempGO.GetComponent<GameConfig>();
                if (_instance == null)
                {
                    Debug.LogError("Get GameConfig failed");
                    return null;
                }
            }
            return _instance;

        }
    }

    [Header("モード切替")]
    public bool isEditMode = true;

    #region param

    [Header("カメラ")]
    public Camera MainCamera;//メインカメラ
    public Transform defaultCameraPointT;
    public Transform mapCameraRootPoint;
    public Vector3 cameraDefaultEuler = new Vector3(60, 0, 0);
    #endregion param

    #region other_pram

    [Header("親オブジェクト")]
    public GameObject objPoolCellRoot;
    public GameObject managerRoot;

    [Header("インスタンス化去れたオブジェクトの親")]
    public GameObject sceneObjRoot;

    [Header("UI")]
    public MapEditUI mapEditUI;

    [Header("マップデータ")]
    public List<SceneMapData> sceneMapDataList = new List<SceneMapData>();

    [Header("マップエディターデータ")]
    public MapEditorOperateData mapEditorOperateData = new MapEditorOperateData(true);

    [Header("ゲームモードテスト用")]
    public DemoTest_1 testScript;



    #endregion other_param

    #region lifeCycle

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion lifeCycle

    public void DoInit()
    {
        if (objPoolCellRoot == null)
        {
            objPoolCellRoot = new GameObject("[ObjPoolCellRoot]");
        }
        DontDestroyOnLoad(objPoolCellRoot);
        if (managerRoot == null)
        {
            managerRoot = new GameObject("[ManagerRoot]");
        }
        DontDestroyOnLoad(managerRoot);

    }


    #region 外部方法

    #region Camera

    public void SetCamera2DefaultPoint(Camera c)
    {
        if (c == null)
            return;
        if (defaultCameraPointT == null)
        {
            Debug.LogError("カメラ初期化エラー");
            return;
        }

        c.transform.position = defaultCameraPointT.position;
        c.transform.rotation = defaultCameraPointT.rotation;
    }

    public void SetCameraFollowRootPoint(Camera c,out Transform returnRootPoint)
    {
        if (mapCameraRootPoint == null)
        {
            Debug.LogError("カメラ初期化エラー");
            if (c != null)
            {
                returnRootPoint = c.transform;
            }
            else
            {
                returnRootPoint = null;
            }
            return;
        }

        if (c != null)
        {
            c.transform.SetParent(mapCameraRootPoint);
        }

        float dist = (c.transform.position - mapCameraRootPoint.position).magnitude;
        Vector3 f = Quaternion.Euler(cameraDefaultEuler) * Vector3.forward;
        c.transform.position = mapCameraRootPoint.position + (-f) * dist;

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        if (go != null)
        {
            go.transform.SetParent(mapCameraRootPoint);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Renderer r = go.GetComponent<Renderer>();
            if (r != null)
            {
                r.receiveShadows = false;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        returnRootPoint = mapCameraRootPoint;
        
    }

    #endregion Camera


    public void SetOperateText(string str)
    {
        if (mapEditUI != null)
        {
            mapEditUI.SetOperateText(str);
        }
    }

    #endregion 外部方法
}
