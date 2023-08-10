using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorManager : MonoSingleton<MapEditorManager>
{

    bool operateState = false;//操作スイッチ

    E_EditMode editMode = E_EditMode.None;

    #region マップ操作データ
    public MapEditorOperateData operateData;

    #endregion マップ操作データ

    public MapEditorElementManager elementMgr;
    public MapEditorCameraManager cameraMgr;

    bool onPlane = false;

    #region curPlacementMode_param
    GameObject testObj;
    [SerializeField] EmptyMapCell curCell = null;

    #endregion curPlacementMode_param

    #region curOperationMode_param

    [SerializeField] MapElementCell curOperationCell = null;
    [SerializeField] MapElementCell curSelectOperationCell = null;
    #endregion curOperationMode_param

    Vector3 hitPoint = Vector3.zero;

    #region 其他参数
    int curOperatePrefabID = -1;
    MapElementCell curOperatePrefab;
    Vector3 previewPosOffset = new Vector3(0, 0.2f, 0);
    Quaternion previewRotation = new Quaternion();

    #endregion 其他参数

    public void SetObjActive(GameObject obj,bool active)
    {
        if (obj == null) return;
        if (obj.activeInHierarchy != active)
        {
            obj.SetActive(active);
        }
    }
    #region lifeCycle

    private void Update()
    {
        if (cameraMgr != null)
        {
            cameraMgr.DoUpdate(Time.deltaTime);

            EditorMapRaycast();
        }

        if (elementMgr != null)
        {
            elementMgr.DoUpdate(Time.deltaTime);
        }



    }

    private void FixedUpdate()
    {
        if (cameraMgr != null)
        {
            cameraMgr.DoFixedUpdate(Time.fixedDeltaTime);
        }


    }

    private void OnDestroy()
    {

        //unregister event
        //UnInitInputEvent();
    }

    #endregion lifeCycle


    public void DoInit(GameObject goRoot=null)
    {
        if (goRoot != null)
        {
            this.transform.SetParent(goRoot.transform);
        }

        operateData = GameConfig.Instance.mapEditorOperateData;

        //Init EditorElementMgr
        elementMgr = MapEditorElementManager.Instance;
        elementMgr.DoInit(goRoot);

        //Init EditorCameraMgr
        cameraMgr = MapEditorCameraManager.Instance;
        cameraMgr.DoInit();


        //Init EditorInputMode
        InitInputEvent();

        //testObj = AssetManager.Instance.Create(208);

        editMode = E_EditMode.Placement;

        operateState = true;

    }

    void InitInputEvent()
    {
        InputManager.Instance.AddInputMode(E_InputMode.Editor);//添加map editor input mode
        InputManager.Instance.SetInputModeKeyboardState(E_InputMode.Editor, true);//键盘输入开关
        InputManager.Instance.SetInputModeMouseState(E_InputMode.Editor, true);//鼠标输入开关
        UnBindInputEvent();//先解绑，避免重复绑定
        BindInputEvent();//绑定事件
    }

    void UnInitInputEvent()
    {
        UnBindInputEvent();
    }

    void BindInputEvent()
    {
        //bind event
        //----- mouse
        InputManager.Instance.event_mouseLeftButtonDown += MouseLeftButtonClickDown;
        InputManager.Instance.event_mouseLeftButtonUp += MouseLeftButtonClickUp;

        InputManager.Instance.event_mouseRightButtonDown += MouseRightButtonClickDown;
        InputManager.Instance.event_mouseRightButtonUp += MouseRightButtonClickUp;

        InputManager.Instance.event_mouseMidButtonDown += MouseMidButtonClickDown;
        InputManager.Instance.event_mouseMidButtonUp += MouseMidButtonClickUp;

        InputManager.Instance.event_mouseScrollWheelDown += MouseScrollWheelDown;
        InputManager.Instance.event_mouseScrollWheelUp += MouseScrollWheelUp;

        //----- keyboard
        InputManager.Instance.event_Button1 += KeyboardQButton;
        InputManager.Instance.event_Button2 += KeyboardEButton;

        InputManager.Instance.event_Button3 += KeyboardSpaceButton;
    }

    void UnBindInputEvent()
    {
        //----- mouse
        InputManager.Instance.event_mouseLeftButtonDown -= MouseLeftButtonClickDown;
        InputManager.Instance.event_mouseLeftButtonUp -= MouseLeftButtonClickUp;

        InputManager.Instance.event_mouseRightButtonDown -= MouseRightButtonClickDown;
        InputManager.Instance.event_mouseRightButtonUp -= MouseRightButtonClickUp;

        InputManager.Instance.event_mouseMidButtonDown -= MouseMidButtonClickDown;
        InputManager.Instance.event_mouseMidButtonUp -= MouseMidButtonClickUp;

        InputManager.Instance.event_mouseScrollWheelDown -= MouseScrollWheelDown;
        InputManager.Instance.event_mouseScrollWheelUp -= MouseScrollWheelUp;

        //----- keyboard
        InputManager.Instance.event_Button1 -= KeyboardQButton;
        InputManager.Instance.event_Button2 -= KeyboardEButton;

        InputManager.Instance.event_Button3 -= KeyboardSpaceButton;
    }

    //射线检测
    void EditorMapRaycast()
    {
        if (!operateState) return;

        int detectMask = -1;
        if (editMode == E_EditMode.Placement) detectMask = GameDefine.EditElement_Mask;
        else if (editMode == E_EditMode.Operation) detectMask = GameDefine.Terrain_Mask;

        if (cameraMgr.controlCamera != null)
        {
            Ray r = cameraMgr.controlCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(r, out hitInfo, 100, /*GameDefine.EditElement_Mask*/detectMask))
            {
                if (hitInfo.collider != null)
                {
                    if (editMode == E_EditMode.Placement)
                    {
                        EmptyMapCell cell = hitInfo.collider.gameObject.GetComponent<EmptyMapCell>();
                        SetCurEmptyCell(cell);
                        ShowCurOperatePrefabs(true);
                    }
                    else if (editMode == E_EditMode.Operation)
                    {
                        MapElementCell cell = hitInfo.collider.gameObject.GetComponent<MapElementCell>();
                        SetCurOperationCell(cell);
                    }
                }
            }
            else
            {
                if (editMode == E_EditMode.Placement)
                {
                    ReleaseCurEmptyCell();
                    ShowCurOperatePrefabs(false);
                }
                else if (editMode == E_EditMode.Operation)
                {
                    ReleaseCurOperationCell();
                }
            }
        }

    }

    void SetCurEmptyCell(EmptyMapCell cell)
    {
        if (cell == null) return;
        if (curCell == cell) return;

        ReleaseCurEmptyCell();

        onPlane = true;

        hitPoint = cell.GetElementPosition();

        curCell = cell;
        curCell.SetSelect(true);
    }

    void ReleaseCurEmptyCell()
    {
        if (curCell != null)
        {
            curCell.SetSelect(false);
            curCell = null;
        }

        onPlane = false;
    }

    void ShowCurOperatePrefabs(bool show)
    {
        if (curOperatePrefab == null)
        {
            if (show)
            {

                curOperatePrefab = CreateOperatePrefabs();

                if (curOperatePrefab == null) return;

            }
            else
            {
                return;
            }
        }
        else
        {
            if(curOperatePrefabID != MapEditorElementManager.Instance.GetCurOperateElementID())
            {
                ReleaseCurOperatePrefabs();

                curOperatePrefab = CreateOperatePrefabs();

                if (curOperatePrefab == null) return;

            }
        }

        if (show)
        {
            if (curCell == null)
            {
                Debug.LogError("[MapEditorManager]Cur emptyCell is null ,show prefabs failed");
                return;
            }

            curOperatePrefab.SetPosition(curCell.GetElementPosition() + previewPosOffset);

        }

        SetObjActive(curOperatePrefab.gameObject, show);

    }

    MapElementCell CreateOperatePrefabs()
    {
        GameObject tempGO = AssetManager.Instance.Create(MapEditorElementManager.Instance.GetCurOperateElementID());
        if (tempGO == null)
        {
            Debug.LogError("[MapEditorManager]Create preview prefab failed");
            return null;
        }

        MapElementCell cell = tempGO.AddComponentOnce<MapElementCell>();
        if (cell == null)
        {
            Debug.LogError("[MapEditorManager]Create preview prefab failed");
            return null;
        }

        curOperatePrefabID = MapEditorElementManager.Instance.GetCurOperateElementID();

        return cell;
    }

    void ReleaseCurOperatePrefabs()
    {
        if (curOperatePrefab == null) return;

        //Debug.Log("release");

        curOperatePrefabID = -1;

        previewRotation = Quaternion.identity;

        AssetManager.Instance.Destory(curOperatePrefab.gameObject);
        curOperatePrefab = null;
    }

    void SetCurOperationCell(MapElementCell cell)
    {
        if (cell == null) return;
        ReleaseCurOperationCell();

        curOperationCell = cell;
        //curOperationCell.SetSelect(true);
    }

    void ReleaseCurOperationCell()
    {
        if (curOperationCell != null)
        {
            //curOperationCell.SetSelect(false);
            curOperationCell = null;
        }
    }

    void SetCurSelectOperationCell(MapElementCell cell)
    {
        if (cell == null) return;
        if (curSelectOperationCell == cell) return;

        ReleaseCurSelectOperationCell();

        curSelectOperationCell = cell;
        curSelectOperationCell.SetSelect(true);
    }

    void ReleaseCurSelectOperationCell()
    {
        if (curSelectOperationCell != null)
        {
            curSelectOperationCell.SetSelect(false);
            curSelectOperationCell = null;
        }
    }

    #region input_event

    //--------- mouse

    /// <summary>
    /// 左クリック（押す
    /// </summary>
    public void MouseLeftButtonClickDown()
    {
        if (!operateState) return;

        //[生成モード]
        //create element

        //[操作モード（削除とか]
        //select element
        if (editMode == E_EditMode.Placement)
        {
            if (curCell != null)
            {

                if (elementMgr.CreateElement(curCell,previewRotation) != null)
                {
                    curCell.SetSelect(false);
                    curCell = null;

                    GameConfig.Instance.SetOperateText("オブジェクト生成");
                }
            }
        }
        else if (editMode == E_EditMode.Operation)
        {
            if (curOperationCell != null)
            {
                SetCurSelectOperationCell(curOperationCell);
            }

        }
    }

    /// <summary>
    /// 左クリック（放す
    /// </summary>
    public void MouseLeftButtonClickUp()
    {
        if (!operateState) return;
        //Debug.LogError(" Mouse button left click up");
    }

    /// <summary>
    /// ホイール（押す
    /// </summary>
    public void MouseMidButtonClickDown()
    {
        if (!operateState) return;

        //カメラ平行移動
        if (cameraMgr != null)
        {
            cameraMgr.SetHorizontalMoveCamera(true);
        }
        
    }

    /// <summary>
    /// ホイール（放す
    /// </summary>
    public void MouseMidButtonClickUp()
    {
        if (!operateState) return;

        if (cameraMgr != null)
        {
            cameraMgr.SetHorizontalMoveCamera(false);
        }
    }

    /// <summary>
    /// 右クリック（押す
    /// </summary>
    public void MouseRightButtonClickDown()
    {
        if (!operateState) return;

    }

    /// <summary>
    /// 右クリック（放す
    /// </summary>
    public void MouseRightButtonClickUp()
    {
        if (!operateState) return;

        //Debug.LogError("Mouse button right click up");
    }

    public void MouseScrollWheelUp()
    {
        if (cameraMgr != null)
        {
            cameraMgr.CameraZoom(true, 1);
        }

        //Debug.LogError("UP");
    }

    public void MouseScrollWheelDown()
    {
        if (cameraMgr != null)
        {
            cameraMgr.CameraZoom(false, 1);
        }

        //Debug.LogError("DOWN");
    }

    //----------- keyboard

    public void KeyboardQButton(bool pressDown)
    {
        if (cameraMgr != null)
        {
            cameraMgr.CameraRotate(true);
        }
    }

    public void KeyboardEButton(bool pressDown)
    {
        if (cameraMgr != null)
        {
            cameraMgr.CameraRotate(false);
        }
    }

    public void KeyboardSpaceButton(bool pressDown)
    {
        if (operateState == false) return;
        //Debug.Log("Space button press");
        if (curOperatePrefab != null)
        {
            //Debug.LogError("pre rot = " + previewRotation);
            previewRotation *= Quaternion.Euler(new Vector3(0, 90, 0));
            //Debug.LogError("after rot = " + previewRotation);

            curOperatePrefab.transform.localRotation = previewRotation;
        }

        GameConfig.Instance.SetOperateText("オブジェクト回転");
    }

    #endregion input_event

    #region 其他方法

    public void ChangeOperateState(bool state)
    {
        if (operateState != state)
        {
            operateState = state;
        }

        if (state == false)
        {
            ShowCurOperatePrefabs(false);
        }
    }

    public void ChangeEditMode(E_EditMode mode)
    {
        if (mode == editMode) return;

        editMode = mode;

        switch (mode)
        {
            case E_EditMode.Placement:

                ReleaseCurOperationCell();
                ReleaseCurSelectOperationCell();

                break;

            case E_EditMode.Operation:

                ReleaseCurEmptyCell();
                ReleaseCurOperatePrefabs();

                break;

            default:
                break;
        }

    }

    #endregion 其他方法


//test method

    public void DeleteTest()
    {
        if (curSelectOperationCell != null)
        {
            if (curSelectOperationCell.GetAboveCell() != null)
            {
                return;
            }

            MapElementCell tempCell = curSelectOperationCell;

            ReleaseCurOperationCell();
            ReleaseCurSelectOperationCell();

            EditorMapControl.Instance.DeleteElementN(tempCell.CellID);
        }
    }
}


[System.Serializable]
public class MapEditorOperateData
{
    [Header("カメラデータ")]
    public float cameraHorizontalMoveFactor = 0.1f;
    public float cameraMoveSpeed;
    public float cameraZoomSpeed;
    public int zoomValueInterval;
    public bool isInverseRange = false;
    [SerializeField] Vector2 cameraZoomRangeVector;//ズームイン範囲
    public Vector2 cameraZoomRange
    {
        get
        {
            return isInverseRange ? -cameraZoomRangeVector : cameraZoomRangeVector;
        }
    }

    [Header("マップデータ")]
    public int emptyElementID;
    [SerializeField]Vector2Int mapSize = Vector2Int.zero;//マップサイズ
    public Vector2Int MapSize
    {
        get
        {
            if (mapSize.x < 0 || mapSize.y < 0)
            {
                return new Vector2Int(10,10);
            }
            return mapSize;
        }
    }
    [SerializeField]Vector3 elementSize = Vector3.zero;//オブジェクトサイズ
    public Vector3 ElementSize
    {
        get
        {
            if(elementSize.x<=0|| elementSize.y < 0)
            {
                return Vector3.one;
            }
            return elementSize;
        }
    }
    [SerializeField]Transform centerPointT;//センター
    public Vector3 centerPoint//センター
    {
        get
        {
            if (centerPointT == null) return Vector3.zero;
            return centerPointT.position;
        }
    }
    public bool xOffsetSign;
    public bool yOffsetSign;
    public float elementBaseY;

    [Header("対象オブジェクトデータ")]
    List<int> editorElementIDList = new List<int>();//対象オブジェクトIDリスト
    public List<int> EditorElementIDList
    {
        get
        {
            if (editorElementIDList == null)
            {
                editorElementIDList = new List<int>();
            }
            return editorElementIDList;
        }
    }

    [Header("UIデータ")]
    [SerializeField] Transform previewPrefabCreatePoint;
    public Transform previewPrefabPoint
    {
        get { return previewPrefabCreatePoint; }
    }


    public MapEditorOperateData(bool constSet=false)
    {
        if (constSet)
        {
            if (editorElementIDList == null) editorElementIDList = new List<int>();

            editorElementIDList.Clear();

            Array enumValues = Enum.GetValues(typeof(E_ElementType));
            
            foreach(var temp in enumValues)
            {
                if ((E_ElementType)temp == E_ElementType.None)
                {
                    continue;
                }
                editorElementIDList.Add((int)temp);
            }
        }

    }
}

public enum E_EditMode
{
    None=-1,
    Placement=0,
    Operation=1,
}