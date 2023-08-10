using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップマネージャー（ゲームモード
/// ＊ゲームモードはマップをテストするために実装したものです＊
/// </summary>
public class MapManager : Singleton<MapManager>
{
    SceneMapData curSceneMapData;

    Dictionary<long, MapCell> mapCellDic = new Dictionary<long, MapCell>();
    int dicKeyOffset = 1000;
    long dicKeyOffset_q2=1000000;
    long dicKeyOffset_q3=1000000000;

    //[cellId:int,cell:MapCell]
    Dictionary<int, MapCell> mapCellIdDic = new Dictionary<int, MapCell>();
    //[zoneId:int,cellList:List<MapCell>]
    Dictionary<int, List<MapCell>> zoneEndCellDic = new Dictionary<int, List<MapCell>>();
    List<MapCell> startCellList = new List<MapCell>();
    List<MapCell> targetCellList = new List<MapCell>();
    List<E_CellDir> searchSequence = new List<E_CellDir> { E_CellDir.Left, E_CellDir.Backward, E_CellDir.Right, E_CellDir.Forward /* ,E_CellDir.Up,E_CellDir.Down */};

    #region 测试参数

    int testMapLevelIndex = 1;
    SceneMapData testMapData;

    #endregion 测试参数


    #region 生命周期
    public void DoUpdate(float deltatime)
    {

    }

    public void DoFixedUpdate(float fixedUpdateTime)
    {

    }

    public void LatedUpdate(float deltatime)
    {

    }

    #endregion 生命周期

    void InitMap()
    {
        if (mapCellDic == null || mapCellIdDic == null)
        {
            Debug.LogError("[MapManager]Init map failed , mapCellDic or mapCellIdDic is null");
            return;
        }

        foreach(var temp in mapCellIdDic)
        {
            if (temp.Value == null) continue;
            temp.Value.DoInit();


            if (temp.Value.CellType == E_CellType.StartCrystal || temp.Value.CellType == E_CellType.TargetCrystal)
            {
                CreateSceneCrystalCell(temp.Value);//创建水晶对象，绑定
            }
            
        }

        curSceneMapData = new SceneMapData();
        curSceneMapData.mapName = "testDemo";
        curSceneMapData.mapSize = new Vector2Int(20, 20);

    }

    void ConnectMapCell()
    {
        if (mapCellIdDic == null || mapCellIdDic.Count==0)
        {
            Debug.LogError("[MapManager]Connect map cell failed, cell count = 0");
            return;
        }

        foreach(var temp in mapCellIdDic)
        {
            if (temp.Value == null) continue;
            ConnectMapCell(temp.Value);
        }
    }

    void ConnectMapCell(MapCell cell)
    {
        if (curSceneMapData == null) return;
        if (cell == null) return;

        //Debug.LogError(0);

        Vector3Int gridPos = cell.GetGridPos();

        Vector3Int searchGridPos = gridPos;
        MapCell neighborCell = null;
        searchGridPos.x -= 1;
        if (searchGridPos.x >= 0 && searchGridPos.x < curSceneMapData.mapSize.x)
        {
            //left
            neighborCell = GetCellWithGridPos(searchGridPos);
            if (neighborCell != null)
            {
                //Debug.LogError(1);
                MapCellTool.ConnectCells(cell, neighborCell, E_CellDir.Left);
            }
        }

        searchGridPos = gridPos;
        searchGridPos.x += 1;
        if (searchGridPos.x >= 0 && searchGridPos.x < curSceneMapData.mapSize.x)
        {
            //right
            neighborCell = GetCellWithGridPos(searchGridPos);
            if (neighborCell != null)
            {
                //Debug.LogError(2);
                MapCellTool.ConnectCells(cell, neighborCell, E_CellDir.Right);
            }
        }

        searchGridPos = gridPos;
        searchGridPos.y -= 1;
        if (searchGridPos.y >= 0 && searchGridPos.y < curSceneMapData.mapSize.y)
        {
            //backward
            neighborCell = GetCellWithGridPos(searchGridPos);
            if (neighborCell != null)
            {
                //Debug.LogError(3);
                MapCellTool.ConnectCells(cell, neighborCell, E_CellDir.Backward);
            }
        }

        searchGridPos = gridPos;
        searchGridPos.y += 1;
        if (searchGridPos.y >= 0 && searchGridPos.y < curSceneMapData.mapSize.y)
        {
            //forward
            neighborCell = GetCellWithGridPos(searchGridPos);
            if (neighborCell != null)
            {
                //Debug.LogError(4);
                MapCellTool.ConnectCells(cell, neighborCell, E_CellDir.Forward);
            }
        }

        searchGridPos = gridPos;
        searchGridPos.z -= 1;
        if (searchGridPos.z >= 0)
        {
            //down
            neighborCell = GetCellWithGridPos(searchGridPos);
            if (neighborCell != null)
            {
                //Debug.LogError(5);
                MapCellTool.ConnectCells(cell, neighborCell, E_CellDir.Down);
            }
        }

        searchGridPos = gridPos;
        searchGridPos.z += 1;
        if (searchGridPos.z >= 0)
        {
            //up
            neighborCell = GetCellWithGridPos(searchGridPos);
            if (neighborCell != null)
            {
                //Debug.LogError(6);
                MapCellTool.ConnectCells(cell, neighborCell, E_CellDir.Up);
            }
        }
    }

    void UnbindConnectMapCell(MapCell cell)
    {

    }

    void InitMapRoad()
    {
        if (zoneEndCellDic == null || zoneEndCellDic.Count==0)
        {
            Debug.LogError("zoneStartEndCellDic is null, InitMapRoad failed");
            return;
        }

        Debug.LogError("zoneEndCellDic count = " + zoneEndCellDic.Count);

        //每个区域单独寻路
        foreach (var temp in zoneEndCellDic)
        {
            if (temp.Value == null) continue;
            if (temp.Value.Count == 0) continue;

            Debug.LogError("search zone id = " + temp.Key);

            //Stack<MapCell> cellStack = new Stack<MapCell>();
            Queue<MapCell> cellStack = new Queue<MapCell>();
            List<int> searchOverIDList = new List<int>();

            for (int index = 0; index < temp.Value.Count; index++)
            {
                MapCell curCell = temp.Value[index];
                if (curCell == null) continue;
                if (curCell.CellType != E_CellType.ZoneBorderEnd || curCell.needConnectOperate == false) continue;

                //cellStack.Push(curCell);
                cellStack.Enqueue(curCell);
            }

            while (cellStack.Count > 0)
            {
                //MapCell searchCell = cellStack.Pop();
                MapCell searchCell = cellStack.Dequeue();

                if (searchCell == null) continue;

                searchOverIDList.Add(searchCell.cellID);

                for(int searchIndex = 0; searchIndex < searchSequence.Count; searchIndex++)
                {
                    E_CellDir curSearchDir = searchSequence[searchIndex];
                    MapCell curNeighbor = searchCell.GetNeighborCell(curSearchDir);
                    if (curNeighbor == null || curNeighbor.zoneID != searchCell.zoneID) continue;

                    if (searchOverIDList.Contains(curNeighbor.cellID)) continue;

                    if (curNeighbor.CellType == E_CellType.NormalCell)
                    {
                        curNeighbor.SetNextDir(MapCellTool.GetInverseDir(curSearchDir));
                        //curNeighbor.SetNextDir(curSearchDir);

                        //cellStack.Push(curNeighbor);
                        cellStack.Enqueue(curNeighbor);
                        
                    }
                    else if (curNeighbor.CellType == E_CellType.ZoneBorderStart)
                    {
                        curNeighbor.SetNextDir(MapCellTool.GetInverseDir(curSearchDir));
                    }
                }
            }

        }

        ConnectZone();
    }

    void ConnectZone()
    {
        Queue<MapCell> searchQueue = new Queue<MapCell>();
        List<MapCell> searchDoneList = new List<MapCell>();
        //link target cell
        if(targetCellList!=null && targetCellList.Count != 0)
        {
            for(int index = 0; index < targetCellList.Count; index++)
            {
                MapCell curTargetCell = targetCellList[index];
                if (curTargetCell == null) continue;

                searchQueue.Enqueue(curTargetCell);

                while (searchQueue.Count > 0)
                {

                    MapCell curSearchCell = searchQueue.Dequeue();
                    if (curSearchCell == null) continue;
                    if (searchDoneList.Contains(curSearchCell)) continue;

                    searchDoneList.Add(curSearchCell);

                    List<MapCell> endBorderCellNeighbors = curSearchCell.GetNeighborCellWithType(E_CellType.ZoneBorderEnd);
                    for (int index_2 = 0; index_2 < endBorderCellNeighbors.Count; index_2++)
                    {
                        MapCell neighbor = endBorderCellNeighbors[index_2];
                        if (neighbor == null) continue;
                        if (searchDoneList.Contains(neighbor)) continue;

                        //if (neighbor.zoneID == curTargetCell.zoneID) continue;

                        if (neighbor.needConnectOperate)
                        {
                            neighbor.SetNextCell(curTargetCell);
                        }

                        if (searchDoneList.Contains(neighbor) == false)
                        {
                            searchQueue.Enqueue(neighbor);
                        }
                        
                    }
                }
            }

        }

        //link Start Cell


        //const cell

        //zoneBorderEndCell(设定endCell的下一个目标点）
        //searchQueue.Clear();

        if(zoneEndCellDic!=null && zoneEndCellDic.Count > 0)
        {
            foreach(var tempCell in zoneEndCellDic)
            {
                if (tempCell.Value == null) continue;
                List<MapCell> zoneEndCellList = tempCell.Value;

                for(int index_1 = 0; index_1 < zoneEndCellList.Count; index_1++)
                {
                    MapCell curEndCell = zoneEndCellList[index_1];
                    if (curEndCell == null) continue;
                    if (curEndCell.needConnectOperate == false) continue;


                    List<MapCell> endBorderCellNeighbors = curEndCell.GetAllNeighborCell();
                    for (int index_2 = 0; index_2 < endBorderCellNeighbors.Count; index_2++)
                    {
                        MapCell curNeighbor = endBorderCellNeighbors[index_2];
                        if (curNeighbor == null) continue;
                        if (curNeighbor.zoneID == curEndCell.zoneID) continue;


                        if (curNeighbor.CellType == E_CellType.ZoneBorderStart || curNeighbor.CellType == E_CellType.ConstCell)
                        {
                            curEndCell.SetNextCell(curNeighbor);
                            break;
                        }
                    }
                }
            }
        }
    }


    void CreateSceneCrystalCell(MapCell cell)
    {
        if (cell == null ||(cell.CellType!=E_CellType.StartCrystal && cell.CellType!=E_CellType.TargetCrystal))
        {
            return;
        }

        E_Group group = (cell.CellType == E_CellType.StartCrystal) ? E_Group.Group_1 : E_Group.Group_2;

        SceneCrystal crystal = cell.cellGO.AddComponentOnce<SceneCrystal>();
        if (crystal == null)
        {
            Debug.LogError("[CreateSceneCrystalCell]create crystal cell failed, cellId =" + cell.cellID);
            return;
        }

        crystal.Group = group;
        crystal.CrystalID = cell.cellID;

        crystal.DoInit();
    }

    #region 外部方法

    public void DoInit()
    {
        InitMap();
        ConnectMapCell();
        InitMapRoad();
    }

    public MapCell GetCellWithID(int id)
    {
        if (mapCellIdDic == null)
        {
            Debug.LogError("[MapManager]Get cell failed, mapCellDic is null");
            return null;
        }

        MapCell returnC = null;
        if (mapCellIdDic.TryGetValue(id, out returnC) == false)
        {
            Debug.LogError("[MapManager]Get cell failed, id = " + id);
        }

        return returnC;
    }

    public MapCell GetCellWithGridPos(Vector3Int gridPos)
    {

        if (mapCellDic == null)
        {
            Debug.LogError("[MapManager]Get cell failed, mapCellDic is null");
            return null;
        }
        long longIndex = dicKeyOffset_q3 + (gridPos.z + 1) + (gridPos.y + 1) * dicKeyOffset + (gridPos.x + 1) * dicKeyOffset * dicKeyOffset;
        MapCell cell = null;
        if (mapCellDic.TryGetValue(longIndex, out cell) == false)
        {
        }
        return cell;

    }

    public void RegisterCell(MapCell cell)
    {
        if (cell == null) return;

        if (mapCellDic == null) mapCellDic = new Dictionary<long, MapCell>();

        if (mapCellIdDic == null) mapCellIdDic = new Dictionary<int, MapCell>();

        if (zoneEndCellDic == null) zoneEndCellDic = new Dictionary<int, List<MapCell>>();

        if (mapCellIdDic.ContainsKey(cell.cellID))
        {
            Debug.LogError("[MapManager]Register cell failed , already had the same cell , id = " + cell.cellID);
            return;
        }
        mapCellIdDic.Add(cell.cellID, cell);

        Vector3Int gridPos = cell.GetGridPos();
        long longIndex = dicKeyOffset_q3 + (gridPos.z + 1) + (gridPos.y + 1) * dicKeyOffset + (gridPos.x + 1) * dicKeyOffset * dicKeyOffset;

        if (mapCellDic.ContainsKey(longIndex))
        {
            Debug.LogError("[MapManager]register cell grid Dic failed, already had the same cell, plz check,cellid = " + cell.cellID + "cellid2 = " + mapCellDic[longIndex].cellID+",index = "+longIndex);
            return;
        }

        mapCellDic.Add(longIndex, cell);

        if (cell.CellType == E_CellType.ZoneBorderEnd)
        {
            if (cell.zoneID == -1)
            {
                Debug.LogError("[MapManager]zoneBorderEndPoint not set zone id , cell id = " + cell.cellID);
                return;
            }
            if (zoneEndCellDic.ContainsKey(cell.zoneID) == false)
            {
                zoneEndCellDic.Add(cell.zoneID, new List<MapCell>());
            }
            else
            {
                if (zoneEndCellDic[cell.zoneID] == null) zoneEndCellDic[cell.zoneID] = new List<MapCell>();
            }
            zoneEndCellDic[cell.zoneID].Add(cell);
        }else if (cell.CellType == E_CellType.StartCell)
        {
            if (startCellList == null) startCellList = new List<MapCell>();

            if (startCellList.Contains(cell) == false)
            {
                startCellList.Add(cell);
            }


        }else if (cell.CellType == E_CellType.TargetCell)
        {
            if (targetCellList == null) targetCellList = new List<MapCell>();

            if (targetCellList.Contains(cell) == false)
            {
                targetCellList.Add(cell);
            }
        }
    }

    public void UnregisterCell(MapCell cell)
    {

    }

    public void UnregisterCell(int cellId)
    {

    }

    public void UnregisterCell(Vector3Int gridPos)
    {
        long longIndex = dicKeyOffset_q3 + (gridPos.z+1) + (gridPos.y+1) * dicKeyOffset + (gridPos.x+1) * dicKeyOffset * dicKeyOffset;
        if (mapCellDic.ContainsKey(longIndex))
        {
            mapCellDic.Remove(longIndex);
        }
    }

    #endregion 外部方法

    public class GridMapCellData
    {
        /// <summary>
        /// <height,cell>
        /// </summary>
        Dictionary<int, MapCell> heightDivideDic;

        public GridMapCellData()
        {
            heightDivideDic = new Dictionary<int, MapCell>();
        }

        public bool IsContains(int key)
        {
            return heightDivideDic.ContainsKey(key);
        }

        public MapCell GetValue(int key)
        {
            if (IsContains(key) == false)
            {
                Debug.LogError("GetValue failed, key = " + key);
                return null;
            }

            return heightDivideDic[key];
        }

        public void AddValue(int key, MapCell cell)
        {
            if (IsContains(key))
            {
                heightDivideDic[key] = cell;
            }
            else
            {
                heightDivideDic.Add(key, cell);
            }
        }
    }
}

[System.Serializable]
public class SceneMapData
{
    public string mapName;
    public int levelIndex = -1;
    public Vector2Int mapSize;
    public Vector3 mapStartPoint;
    public Vector3 elementSize;
}