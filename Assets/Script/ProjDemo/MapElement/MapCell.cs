using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    [SerializeField] bool canChangeCellType = true;
    [SerializeField] E_CellType _cellType = E_CellType.NonInteractive;
    public E_CellType CellType
    {
        get { return _cellType; }
    }
    public int zoneID;//区域id

    public E_ElementType elementType;
    [SerializeField] protected Vector3Int gridPos;//网格位置
    public int cellID;

    //相邻节点列表
    protected Dictionary<int, MapCell> neighborDic = new Dictionary<int, MapCell>();

    [SerializeField]bool takeNextCellSwitch;//使用nextCell还是nextCellDir标识;（用于获取下一目标点true:nextCell,flase:nextCellDir
    [SerializeField] MapCell nextCell;//下一目标节点
    public bool isSetNextCell { get { return nextCell != null; } }

    [SerializeField] E_CellDir nextCellDir = E_CellDir.None;//下一目标节点方向
    [SerializeField] bool connectOperate = true;//用于区域间连接,是否需要作为寻路目标点（启用则自动连接）
    public bool needConnectOperate { get { return connectOperate; } }

    #region component_param

    private Transform cellTransform;
    public Transform cellTrans
    {
        get
        {
            if (cellTransform == null) cellTransform = cellGO.transform;
            return cellTransform;
        }
    }

    private GameObject cellGameObject;
    public GameObject cellGO
    {
        get
        {
            if (cellGameObject == null) cellGameObject = this.gameObject;
            return cellGameObject;
        }
    }

    #endregion component_param

    #region 其他参数

    [SerializeField]GameObject roadPoint;
    public bool HaveRoadPoint { get { return roadPoint != null; } }
    RoadSignArrow arrow;

    #endregion 其他参数

    #region 生命周期

    private void Awake()
    {
        MapManager.Instance.RegisterCell(this);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion 生命周期

    #region 外部方法

    /// <summary>
    /// 生成地图元素时调用的初始化方法
    /// </summary>
    /// <param name="id"></param>
    public virtual void CreatedInit(int id)
    {
        cellID = id;

        Transform tempT = transform.Find("/p_roadPoint");
        if (tempT != null)
        {
            roadPoint = tempT.gameObject;
        }
    }

    /// <summary>
    /// 运行时初始化
    /// </summary>
    public virtual void DoInit()
    {
        if (CellType == E_CellType.NormalCell || CellType == E_CellType.ZoneBorderStart || CellType == E_CellType.ZoneBorderEnd || CellType == E_CellType.ConstCell || CellType == E_CellType.ConstNormalCell)
        {
            if (arrow == null)
            {
                GameObject tempGO = AssetManager.Instance.Create(MapCellTool.roadSignArrowID);
                if (tempGO != null)
                {
                    arrow = tempGO.GetComponent<RoadSignArrow>();
                    if (arrow != null)
                    {
                        arrow.transform.SetParent(this.transform);
                        arrow.transform.localPosition = Vector3.zero;
                        arrow.transform.localRotation = Quaternion.identity;
                        arrow.transform.localScale = Vector3.one;

                    }
                    else
                    {
                        Debug.LogError("[MapCell]Get RoadSignArrow component failed, cellId = " + cellID);
                        DestroyImmediate(tempGO);
                    }
                }
            }

            if (arrow != null)
            {
                arrow.DoInit();
            }
        }
        //固定方向节点以及 边缘出发点/目标点可以手动设置行进方向
        if (CellType == E_CellType.ConstCell || CellType==E_CellType.ConstNormalCell || CellType==E_CellType.ZoneBorderStart)
        {
            if (arrow != null)
            {
                arrow.SetDir(nextCellDir);
                arrow.Show(true);
            }
        }
    }

    public virtual void SetGridPos(Vector3Int pos)
    {
        gridPos = pos;
    }

    public virtual void SetPosition(Vector3 pos, bool isLocal = false)
    {
        if (isLocal)
        {
            cellTrans.localPosition = pos;
        }
        else
        {
            cellTrans.position = pos;
        }
    }

    public virtual void SetRotation(Vector3 euler,bool isLocal = false)
    {
        if (isLocal)
        {
            cellTrans.localRotation = Quaternion.Euler(euler);
        }
        else
        {
            cellTrans.rotation = Quaternion.Euler(euler);
        }
    }

    public virtual Vector3Int GetGridPos()
    {
        return gridPos;
    }

    public virtual void RegisterNeighbor(E_CellDir dir, MapCell cell)
    {
        if (cell == null)
        {
            return;
        }

        if (neighborDic == null) neighborDic = new Dictionary<int, MapCell>();
        if (neighborDic.ContainsKey((int)dir))
        {
            MapCellTool.DisconnectCells(this, neighborDic[(int)dir], dir);
        }

        neighborDic.Add((int)dir, cell);

        //Debug.LogError("register neighbor ,cellid = " + cellID + ",dir = " + dir);
    }

    public virtual void UnregisterNeighbor(E_CellDir dir)
    {
        if (neighborDic == null) return;

        if (neighborDic.ContainsKey((int)dir))
        {
            neighborDic.Remove((int)dir);
        }
    }

    
    public void SetCellType(E_CellType type)
    {
        if (canChangeCellType == false) return;
        _cellType = type;
    }

    /// <summary>
    /// 设置路径的下一目标点（目前只有const cell 会使用（不需要设定方向
    /// </summary>
    /// <param name="cell"></param>
    public void SetNextCell(MapCell cell)
    {
        if (cell == null)
        {
            return;
        }
        nextCell = cell;

    }

    public MapCell GetNextCell()
    {
        if (takeNextCellSwitch)
        {
            return nextCell;
        }
        else
        {
            if (neighborDic.ContainsKey((int)nextCellDir))
            {
                return neighborDic[(int)nextCellDir];
            }

            return null;
        }
    }

    public Vector3 GetNextCellPosition()
    {
        return Vector3.zero;
    }

    /// <summary>
    /// 获取当前路点位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRoadPosition()
    {
        if (HaveRoadPoint)
        {
            return roadPoint.transform.position;
        }
        else
        {
            return this.cellTrans.position;
        }
    }

    /// <summary>
    /// 设置路径的下一目标点（方向
    /// </summary>
    /// <param name="dir"></param>
    public void SetNextDir(E_CellDir dir)
    {
        nextCellDir = dir;

        if (neighborDic.ContainsKey((int)dir) == false)
        {
            Debug.LogError("[MapCell]neighbor cell does not contain dir [" + dir + "] cell,plz checked,cellid = " + cellID);
        }

        Debug.Log("SetNextDir : " + dir + ", cellId = " + cellID);

        if (arrow != null)
        {
            arrow.SetDir(dir);
            if (nextCellDir == E_CellDir.None)
            {
                arrow.Show(false);
            }
            else
            {
                arrow.Show(true);
            }
        }
        else
        {
            Debug.LogError("arrow is null, cellID = " + cellID);
        }
    }

    /// <summary>
    /// 根据方向获取邻接节点
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public MapCell GetNeighborCell(E_CellDir dir)
    {
        if (neighborDic == null) return null;
        MapCell returnValue = null;
        neighborDic.TryGetValue((int)dir, out returnValue);

        return returnValue;
    }

    /// <summary>
    /// 根据类型获得邻接节点
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public List<MapCell> GetNeighborCellWithType(E_CellType type)
    {
        List<MapCell> returnValue = new List<MapCell>();
        if (neighborDic != null)
        {
            foreach(var temp in neighborDic)
            {
                if (temp.Value == null) continue;

                if (temp.Value.CellType == type)
                {
                    returnValue.Add(temp.Value);
                }
            }
        }

        return returnValue;
    }

    /// <summary>
    /// 获取所有邻接节点
    /// </summary>
    /// <returns></returns>
    public List<MapCell> GetAllNeighborCell()
    {
        List<MapCell> returnValue;

        if (neighborDic != null)
        {
            returnValue = new List<MapCell>(neighborDic.Values);
        }
        else
        {
            returnValue = new List<MapCell>();
        }

        return returnValue;
    }

    /// <summary>
    /// 设定下一目标点获取模式
    /// </summary>
    /// <param name="state"></param>
    public void SetTakeNextCellSwitch(bool state)
    {
        takeNextCellSwitch = state;
    }

    /// <summary>
    /// 获取下一目标点类型
    /// </summary>
    /// <returns></returns>
    public E_CellType GetNextCellType()
    {
        if (takeNextCellSwitch)
        {
            if (nextCell == null) return E_CellType.NonInteractive;
            return nextCell.CellType;
        }
        else
        {
            if (neighborDic.ContainsKey((int)nextCellDir) == false || neighborDic[(int)nextCellDir]==null)
            {
                return E_CellType.NonInteractive;
            }

            return neighborDic[(int)nextCellDir].CellType;
        }
    }

    public bool GetNextCellDir(out Vector3 dir)
    {
        dir = Vector3.forward;
        if (takeNextCellSwitch)
        {
            if (nextCell == null) return false;

            dir = (nextCell.GetRoadPosition() - this.GetRoadPosition()).normalized;
            return true;
        }
        else
        {
            switch (nextCellDir)
            {
                case E_CellDir.Forward:
                    dir = Vector3.forward;
                    break;
                case E_CellDir.Backward:
                    dir = -Vector3.forward;
                    break;
                case E_CellDir.Right:
                    dir = Vector3.right;
                    break;
                case E_CellDir.Left:
                    dir = -Vector3.right;
                    break;
                case E_CellDir.Up:
                    dir = Vector3.up;
                    break;
                case E_CellDir.Down:
                    dir = -Vector3.down;
                    break;
                default:
                    return false;
            }
            return true;

        }

    }

    #endregion 外部方法

    #region test_method
    

    Vector3 gizmosOffset = new Vector3(0, 1, 0);
    [Header("テストデータ")]
    public float gizmosRadiu = 0.2f;
    Color constNormalColor = new Color(0.5f, 0, 0.5f, 1);
    public void OnDrawGizmos()
    {

        switch (_cellType) {
            case E_CellType.StartCell:
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(this.transform.position + gizmosOffset, 1f);
                break;

            case E_CellType.TargetCell:
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(this.transform.position + gizmosOffset, 1f);
                break;
            case E_CellType.ZoneBorderStart:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(this.transform.position + gizmosOffset, gizmosRadiu);
                break;

            case E_CellType.ZoneBorderEnd:
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(this.transform.position + gizmosOffset, gizmosRadiu);
                break;
            case E_CellType.NormalCell:
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(this.transform.position + gizmosOffset, gizmosRadiu);
                break;

            case E_CellType.ConstNormalCell:
                Gizmos.color = constNormalColor;
                Gizmos.DrawWireSphere(this.transform.position + gizmosOffset, gizmosRadiu);
                break;
            case E_CellType.ConstCell:
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(this.transform.position + gizmosOffset, gizmosRadiu);
                break;
            default:
                return;
        }


    }

    private void OnDrawGizmosSelected()
    {
        if (neighborDic == null) return;

        foreach(var temp in neighborDic)
        {
            if (temp.Value == null)
            {
                continue;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(temp.Value.transform.position + gizmosOffset, gizmosRadiu + 0.1f);
        }
    }

    #endregion test_method

    [ContextMenu("test find road point")]
    public void TestFindRoadPoint()
    {
        //roadPoint = GameObject.Find("/"+this.gameObject.name+"/p_roadPoint");
        if (transform.childCount == 0)
        {
            Debug.LogError("find road point failed");
            return;
        }
        /*
        Transform[] childTransArray = transform.GetComponentsInChildren<Transform>();
        for(int index = 0; index < childTransArray.Length; index++)
        {
            if (childTransArray[index].gameObject.name == "p_roadPoint")
            {
                roadPoint = childTransArray[index].gameObject;
                break;
            }
        }
        */
        Transform t = transform.Find("p_roadPoint");
        if (t == null)
        {
            Debug.LogError("find road point failed");
            return;
        }
        roadPoint = t.gameObject;

        if (roadPoint == null)
        {
            Debug.LogError("find road point failed");
        }
    }
}

public class MapCellTool
{

    static public int roadSignArrowID = 910001;

    private MapCellTool()
    {

    }

    public static void ConnectCells(MapCell cellOne,MapCell cellTwo,E_CellDir dirRelateToCellOne)
    {
        if(cellOne == null || cellTwo == null)
        {
            Debug.LogError("[MapCellTool] Connect cell failed, cell reference is null");
            return;
        }

        cellOne.RegisterNeighbor(dirRelateToCellOne, cellTwo);
        cellTwo.RegisterNeighbor(GetInverseDir(dirRelateToCellOne), cellOne);

    }

    public static void DisconnectCells(MapCell cellOne,MapCell cellTwo,E_CellDir dirRelateToCellOne)
    {
        if (cellOne!=null)
        {
            cellOne.UnregisterNeighbor(dirRelateToCellOne);
        }

        if (cellTwo != null)
        {
            cellTwo.UnregisterNeighbor(GetInverseDir(dirRelateToCellOne));
        }
    }

    public static E_CellDir GetInverseDir(E_CellDir dir)
    {
        switch (dir)
        {
            case E_CellDir.Up:
                return E_CellDir.Down;

            case E_CellDir.Down:
                return E_CellDir.Up;

            case E_CellDir.Forward:
                return E_CellDir.Backward;

            case E_CellDir.Backward:
                return E_CellDir.Forward;

            case E_CellDir.Left:
                return E_CellDir.Right;

            case E_CellDir.Right:
                return E_CellDir.Left;
            default:
                Debug.LogError("[MapCellTool]E_CellDir value error, value = " + dir);
                return E_CellDir.None;
        }

    }

    public static E_CellDir CalcDir(MapCell cellOne,MapCell cellTwo)
    {
        if(cellOne==null || cellTwo == null)
        {
            Debug.LogError("Get cell dir failed");
            return E_CellDir.None;
        }

        Vector3Int gridPosOffset = cellTwo.GetGridPos() - cellOne.GetGridPos();
        if (gridPosOffset.magnitude != 1)
        {
            Debug.LogErrorFormat("cellID[{0}] cellID[{1}]不是相邻节点");
            return E_CellDir.None;
        }

        if (gridPosOffset.x == 1)
        {
            return E_CellDir.Right;
        }else if (gridPosOffset.x == -1)
        {
            return E_CellDir.Left;
        }else if (gridPosOffset.y == 1)
        {
            return E_CellDir.Up;
        }else if (gridPosOffset.y == -1)
        {
            return E_CellDir.Down;
        }else if (gridPosOffset.z == 1)
        {
            return E_CellDir.Forward;
        }else if (gridPosOffset.z == -1)
        {
            return E_CellDir.Backward;
        }

        Debug.LogErrorFormat("Calc dir failed , cellID[{0}],cellID[{1}],offset={2}", cellOne.GetGridPos(), cellTwo.GetGridPos(), gridPosOffset);
        return E_CellDir.None;
    }
}

public enum E_CellDir
{
    None=-1,
    Up = 0,
    Down = 1,
    Forward = 2,
    Left = 3,
    Backward = 4,
    Right = 5,
}

/// <summary>
/// 地图元素类型
/// </summary>
public enum E_CellType
{
    Obstacle = -2,
    NonInteractive = -1,
    NormalCell = 0,
    ZoneBorderStart = 1,
    ZoneBorderEnd = 2,

    TargetCell = 3,
    StartCell = 4,

    ConstCell = 5,
    ConstNormalCell = 6,

    StartCrystal=7,
    TargetCrystal=8,

}