using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMapControl : Singleton<EditorMapControl>
{
    int emptyElementId;
    public int EmptyElementID { get { return emptyElementId; } }
    #region マップデータ

    Transform elementsRootT;

    Vector2Int mapSize = Vector2Int.zero;
    Vector3 elementSize = Vector3.zero;
    Vector3 centerPoint;
    bool xOffsetSign;
    bool yOffsetSign;
    float elementBaseY;

    #endregion マップデータ

    Vector3 startCellCenter = Vector3.zero;
    Vector3 startPoint = Vector3.zero;

    Dictionary<int, EmptyMapCell> cellDic = new Dictionary<int, EmptyMapCell>();
    Dictionary<int, EmptyMapCell> runtimeCellDic = new Dictionary<int, EmptyMapCell>();

    Dictionary<int, MapElementCell> mapCellDic = new Dictionary<int, MapElementCell>();

    List<int> mapCellAssetIDList = new List<int>();

    #region test_param

    MapElementCell lastedCreatedElement = null;

    #endregion test_param

    #region lifeCycle


    #endregion lifeCycle

    public void DoInit()
    {
        if (MapEditorManager.Instance.operateData != null)
        {
            MapEditorOperateData data = MapEditorManager.Instance.operateData;
            emptyElementId = data.emptyElementID;
            mapSize = data.MapSize;
            elementSize = data.ElementSize;
            centerPoint = data.centerPoint;
            xOffsetSign = data.xOffsetSign;
            yOffsetSign = data.yOffsetSign;

            elementBaseY = data.elementBaseY;

            InitMap();
        }
        else
        {
            Debug.LogError("GameConfigにマップデータがない、初期化エラー");
        }
    }

    void InitMap()
    {
        if (cellDic == null) cellDic = new Dictionary<int, EmptyMapCell>();
        if (mapCellDic == null) mapCellDic = new Dictionary<int, MapElementCell>();
        if (mapCellDic == null) mapCellDic = new Dictionary<int, MapElementCell>();
        if (mapCellAssetIDList == null) mapCellAssetIDList = new List<int>();

        ResetAllMapElement();
        //cellDic.Clear();

        //calc startPoint:スタートポジションを求める
        int xSign = (xOffsetSign) ? 1 : -1;
        int ySign = (yOffsetSign) ? 1 : -1;

        int midElementX = Mathf.FloorToInt(mapSize.x / 2);
        int midElementY = Mathf.FloorToInt(mapSize.y / 2);

        float halfElementX = elementSize.x *0.5f;
        float halfElementY = elementSize.y * 0.5f;

        float xOffset1 = (midElementX * elementSize.x) * xSign;
        float yOffset1 = (midElementY * elementSize.y) * ySign;

        float xOffset2 = (midElementX * elementSize.x + halfElementX) * xSign;
        float yOffset2 = (midElementY * elementSize.y + halfElementY) * ySign;

        startCellCenter.x = centerPoint.x + xOffset1;
        startCellCenter.y = elementBaseY;
        startCellCenter.z = centerPoint.z + yOffset1;

        startPoint.x = centerPoint.x + xOffset2;
        startPoint.y = elementBaseY;
        startPoint.z = centerPoint.z + yOffset2;

        GameObject cellGO = null;
        EmptyMapCell cell = null;
        Vector3 newCellPos = Vector3.zero;

        int cellID = 0;
        //create empty map cell
        for(int yIndex = 0; yIndex < mapSize.y; yIndex++)
        {
            for(int xIndex = 0; xIndex < mapSize.x; xIndex++)
            {
                cellGO = AssetManager.Instance.Create(emptyElementId,elementsRootT);
                if (cellGO == null)
                {
                    Debug.LogError("オブジェクト生成エラーが発生， emptyPrefabID = " + emptyElementId);
                    return;
                }

                cell = cellGO.GetComponent<EmptyMapCell>();
                if (cell == null) { return; }

                cellID = MapEditorElementManager.Instance.nextElementID;//id獲得

                //ポジション設定
                newCellPos.x = startCellCenter.x + (xIndex * elementSize.x) * (-xSign);
                newCellPos.y = elementBaseY;
                newCellPos.z = startCellCenter.z + (yIndex * elementSize.y) * (-ySign);
                cell.SetPosition(newCellPos);
                cell.SetGridPos(xIndex, yIndex, 1);

                cell.DoInit(cellID);

                cell.SetBaseCellState(true);

                cellDic.Add(cellID, cell);

                

            }
        }

        Debug.Log("<color=#ff0000>init editor map done</color>");

        if (cellDic.Count != 0)
        {
            EmptyMapCell firstCell = null;
            if(cellDic.TryGetValue(0,out firstCell)==false)
            {
                return;
            }

            firstCell.gameObject.AddComponentOnce<DirGizmos>();

        }
    }

    void ResetAllMapElement()
    {
        AssetManager.Instance.DestoryObjByID(emptyElementId);

        //マップ内すべてのオブジェクトを削除
        for(int index = 0; index < mapCellAssetIDList.Count; index++)
        {
            AssetManager.Instance.DestoryObjByID(mapCellAssetIDList[index]);
        }

        cellDic.Clear();
        runtimeCellDic.Clear();

        mapCellAssetIDList.Clear();
        mapCellDic.Clear();

        //InitMap();
    }

    #region 外部方法

    public void SetElementRoot(GameObject root)
    {
        if (root == null) return;
        elementsRootT = root.transform;
    }

    public void SetElementRoot(Transform root)
    {
        if (root == null) return;
        elementsRootT = root;
    }

    public MapElementCell GetMapElementByID(int id)
    {
        if (mapCellDic == null) return null;

        MapElementCell cell = null;

        if(mapCellDic.TryGetValue(id,out cell) == false)
        {
            Debug.LogError("MapElementCell取得エラー，id = " + id);
        }
        return cell;
    }

    public EmptyMapCell GetBaseCellByID(int id)
    {
        if (runtimeCellDic != null)
        {
            if (runtimeCellDic.ContainsKey(id))
            {
                return runtimeCellDic[id];
            }
        }

        if (cellDic != null)
        {
            if (cellDic.ContainsKey(id))
            {
                return cellDic[id];
            }
        }

        Debug.LogError("EmptyCell取得エラー，id = " + id);
        return null;
    }

    public void ShowEmptyCellByID(int id,bool show)
    {
        if (cellDic == null) return;

        EmptyMapCell cell = GetBaseCellByID(id);
        if (cell == null)
        {
            Debug.LogError("[" + this.GetType().Name + "]ShowEmptyCell failed ,id = " + id + " , show = " + show);
            return;
        }

        cell.UnregisterElementCorrelate(true);
        cell.ShowCell(show);
    }

    public void RegisterEmptyCell(EmptyMapCell cell)
    {
        if (cell == null) return;

        if (runtimeCellDic == null) runtimeCellDic = new Dictionary<int, EmptyMapCell>();

        Debug.Log("register empty cell , id = " + cell.CellID);

        if (runtimeCellDic.ContainsKey(cell.CellID) == false)
        {
            runtimeCellDic.Add(cell.CellID, cell);
        }
        else
        {
            Debug.LogError("[EditorMapControl]マップの中にもう存在してる，id = " + cell.CellID);
        }
    }

    public void UnRegisterEmptyCell(int id)
    {
        if (runtimeCellDic == null) return;

        if (runtimeCellDic.ContainsKey(id))
        {
            runtimeCellDic.Remove(id);
        }
        else
        {
            Debug.LogErrorFormat("[{0}]削除エラー ，id = {1}", this.GetType().Name, id);
        }
    }

    public void UnRegisterEmptyCell(EmptyMapCell cell)
    {
        if (cell == null) return;
        UnRegisterEmptyCell(cell.CellID);
    }

    /// <summary>
    /// オブジェクト生成
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="createElementPrefabID"></param>
    /// <returns></returns>
    public GameObject CreateElement(EmptyMapCell cell, int createElementPrefabID,params object[] args)
    {
        if (cell == null) return null;

        Quaternion cellRot = Quaternion.identity;
        try
        {
            cellRot = (Quaternion)args[0];
        }
        catch
        {
            cellRot = Quaternion.identity;
        }
        GameObject newGO = AssetManager.Instance.Create(createElementPrefabID,elementsRootT);
        if (newGO != null)
        {
           
            MapElementCell mapCell = newGO.AddComponentOnce<MapElementCell>();
            if (mapCell != null)
            {
                mapCell.SetPosition(cell);
                mapCell.SetRotation(cellRot);
                mapCell.SetGridPos(cell.GetGridPos());
                mapCell.DoInit(MapEditorElementManager.Instance.nextElementID);

                mapCell.RegisterElementCorrelate(cell.GetBelowCell(), false);
                mapCell.SetBaseEmptyCellID(cell.CellID);

                if (mapCellDic.ContainsKey(mapCell.CellID) == false)
                {
                    mapCellDic.Add(mapCell.CellID, mapCell);
                    if (mapCellAssetIDList.Contains(mapCell.CellID) == false)
                    {
                        mapCellAssetIDList.Add(mapCell.CellID);
                    }
                }
                else
                {
                    Debug.LogError("[EditorMapControl]MapCellDic has same id element,id = " + mapCell.CellID);
                }

                //プレイヤーの操作を保存する（巻き戻し用
                OperateDataManager.Instance.OperationRecord(false,E_OperateType.CreateElement,//タイプ
                    mapCell.CellID,//生成されたオブジェクトのID
                    mapCell.GetBaseEmptyCellID(),//マップマスのid
                    mapCell.Trans.rotation,//回転データ
                    MapEditorElementManager.Instance.GetElementPrefabID(mapCell.elementType),//プリハブid
                    mapCell.GetAboveEmptyCellID());


                if (lastedCreatedElement == null)
                {
                    lastedCreatedElement = mapCell;
                }
            }
        }

        return newGO;
    }

    /// <summary>
    /// 巻き戻し（オブジェク生成
    /// </summary>
    /// <param name="elementPrefabId"></param>
    /// <param name="emptyCellId"></param>
    /// <param name="cellId"></param>
    /// <param name="cellRot"></param>
    /// <param name="aboveEmptyCellId">上方辅助元素id</param>
    /// <returns></returns>
    public void CreateElement(int elementPrefabId, int emptyCellId, int cellId, Quaternion cellRot, int aboveEmptyCellId = -1, bool recordOperate = false)
    {
        GameObject newGO = AssetManager.Instance.Create(elementPrefabId,elementsRootT);
        if (newGO != null)
        {
            MapElementCell cell = newGO.AddComponentOnce<MapElementCell>();
            if (cell == null)
            {
                AssetManager.Instance.Destory(newGO);
                Debug.LogErrorFormat("[{0}]MapElementCell獲得エラー",this.GetType().Name);
                return;
            }

            EmptyMapCell baseCell = GetBaseCellByID(emptyCellId);
            if (baseCell == null)
            {
                Debug.LogErrorFormat("[{0}]生成エラーが発生，id = {1}", this.GetType().Name, emptyCellId);
                AssetManager.Instance.Destory(newGO);
                return;
            }
            cell.SetPosition(baseCell);
            cell.SetGridPos(baseCell.GetGridPos());
            cell.DoInit(cellId, aboveEmptyCellId);
            cell.Trans.rotation = cellRot;

            cell.SetBaseEmptyCellID(baseCell.CellID);

            if (mapCellDic.ContainsKey(cell.CellID) == false)
            {
                mapCellDic.Add(cell.CellID, cell);
            }

            if (recordOperate)
            {
                //プレイヤーの操作を保存
                OperateDataManager.Instance.OperationRecord(true, E_OperateType.CreateElement,
                    cell.CellID,
                    cell.GetBaseEmptyCellID(),
                    cell.Trans.rotation,
                    MapEditorElementManager.Instance.GetElementPrefabID(cell.elementType),
                    cell.GetAboveEmptyCellID());
            }
        }
    }

    /// <summary>
    /// 巻き戻し（オブジェクを削除
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="recordOperate">是否记录删除操作</param>
    public void DeleteElement(MapElementCell cell, bool recordOperate = false)
    {
        if (cell == null) return;

        if (mapCellDic.ContainsKey(cell.CellID))
        {
            mapCellDic.Remove(cell.CellID);
        }

        if (recordOperate)
        {
            //削除処理を保存
            OperateDataManager.Instance.OperationRecord(true,E_OperateType.DeleteElement,
                cell.CellID,
                cell.GetBaseEmptyCellID(),
                cell.Trans.rotation,
                MapEditorElementManager.Instance.GetElementPrefabID(cell.elementType),
                cell.GetAboveEmptyCellID());
        }

        cell.UnregisterElementCorrelate(false);
        cell.DeleteSelf();
    }

    public void DeleteElement(int cellId, bool recordOperate = false)
    {
        MapElementCell cell = GetMapElementByID(cellId);

        DeleteElement(cell,recordOperate);
    }

    public void DeleteElementN(int cellId,bool recordOperate = true)
    {
        MapElementCell cell = GetMapElementByID(cellId);

        DeleteElementN(cell,recordOperate);
    }

    /// <summary>
    /// オブジェク削除処理
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="recordOperate"></param>
    public void DeleteElementN(MapElementCell cell, bool recordOperate = true)
    {
        if (cell == null) return;

        if (mapCellDic.ContainsKey(cell.CellID))
        {
            mapCellDic.Remove(cell.CellID);
        }

        if (recordOperate)
        {
            //削除処理を保存
            OperateDataManager.Instance.OperationRecord(false, E_OperateType.DeleteElement,
                cell.CellID,
                cell.GetBaseEmptyCellID(),
                cell.Trans.rotation,
                MapEditorElementManager.Instance.GetElementPrefabID(cell.elementType),
                cell.GetAboveEmptyCellID());
        }

        cell.UnregisterElementCorrelate(false);
        cell.DeleteSelf();
    }

    public Vector2Int GetMapSize()
    {
        return mapSize;
    }

    public Vector3 GetMapStartPoint()
    {
        return startPoint;
    }

    public Vector3 GetBaseElementSize()
    {
        return elementSize;
    }

    public bool GetMapXOffsetSign()
    {
        return xOffsetSign;
    }

    public bool GetMapYOffsetSign()
    {
        return yOffsetSign;
    }

    public Dictionary<int,MapElementCell> GetMapElementDic()
    {
        if (mapCellDic == null) mapCellDic = new Dictionary<int, MapElementCell>();

        return mapCellDic;
    }


    #endregion 外部方法

    #region test_method

    public void DeleteTest()
    {
        if (lastedCreatedElement == null) return;

        lastedCreatedElement.UnregisterElementCorrelate(false);
        lastedCreatedElement.DeleteSelf();

        lastedCreatedElement = null;
    }

    #endregion test_method
}
