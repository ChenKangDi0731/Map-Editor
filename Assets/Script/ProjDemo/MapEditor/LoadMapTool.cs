using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LoadMapTool : MonoBehaviour
{
    public string mapXMLFileName = string.Empty;

    public Transform mapRootT;
    public Vector3 mapCenter = Vector3.zero;

    public List<ElementConfig> elementConfigList;

    Dictionary<int, string> prefabsPathDic = new Dictionary<int, string>();


    [ContextMenu("load all prefabs config")]
    public void LoadAllPrefabConfig()
    {

        if (prefabsPathDic == null) prefabsPathDic = new Dictionary<int, string>();
        prefabsPathDic.Clear();

        //文件目录
        string commonPath = "/Resources/";
        string path = Application.dataPath + commonPath;

        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int index = 0; index < files.Length; index++)
            {
                string name = files[index].Name;
                string tempPath = files[index].FullName;
                tempPath = tempPath.Replace(@"\",@"/");
                string extension = ".prefab";
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                if (name.EndsWith(extension))
                {
                    if (name.Contains("#"))
                    {

                        //UnityEditorInternal.

                        //Debug.LogError("full path = " + tempPath);
                        //Debug.LogError("index of string = " + prefabsPath);
                        int substringIndex = tempPath.IndexOf("Prefabs");
                        //Debug.LogError(substringIndex);

                        string tempPrefabPath = tempPath.Substring(substringIndex);
                        tempPrefabPath = tempPrefabPath.Replace(extension, "");
                        //Debug.LogError("directory = " + tempPrefabPath);
                        
                        GameObject tempGO = Resources.Load<GameObject>(tempPrefabPath) as GameObject;
                        if (tempGO == null)
                        {
                            Debug.LogError("prefab load info failed , name = " + name);
                            continue;
                        }

                        ElementConfig config = tempGO.GetComponent<ElementConfig>();
                        if (config == null)
                        {
                            Debug.LogError("Get Prefab ElementConfig component failed , name = " + name);
                            //UnityEngine.Object.DestroyImmediate(tempGO);
                            continue;
                        }

                        if (prefabsPathDic.ContainsKey((int)config.ElementType)==false)
                        {
                            prefabsPathDic.Add((int)config.ElementType, tempPrefabPath);
                        }
                        config = null;
                        //UnityEngine.Object.DestroyImmediate(tempGO);
                        tempGO = null;

                    }
                }
            }

        }
        else
        {
            Debug.LogError("path does not exist");
        }

        Resources.UnloadUnusedAssets();

        Debug.LogError("prefabPathDic count = " + prefabsPathDic.Count);
    }

    [ContextMenu("load map(xml)")]
    public void LoadMap()
    {
        /*
        if (elementConfigList == null || elementConfigList.Count == 0)
        {
            Debug.LogError("[LoadMapTool]elementConfigList is null, load map failed");
            return;
        }
        */

        if (mapRootT == null)
        {
            mapRootT = this.gameObject.transform;
        }

        if (prefabsPathDic == null)
        {
            Debug.LogError("[LoadMapTool] prefabPathDic is null, load map failed");
            return;
        }

        Dictionary<int, GameObject> prefabsDic = new Dictionary<int, GameObject>();
        for(int index = 0; index < elementConfigList.Count; index++)
        {
            ElementConfig config = elementConfigList[index];
            if (config == null) continue;

            if (prefabsDic.ContainsKey((int)config.ElementType))
            {
                continue;
            }

            prefabsDic.Add((int)config.ElementType, config.gameObject);
        }

        if (string.IsNullOrEmpty(mapXMLFileName))
        {
            return;
        }
        MapInfoData data = null;
        XMLMapRegistrar.LoadMapInfo(mapXMLFileName, out data);

        if (data.IsCellDataNull())
        {
            Debug.LogError("[LoadMapTool]load map xml failed, create map failed");
            return;
        }

        float halfElementX = data.elementSize.x / 2;
        float halfElementY = data.elementSize.y / 2;

        Vector3 startCellCenter = data.mapStartPoint + new Vector3(halfElementX * data.mapXOffsetSign, 0, halfElementY * data.mapYOffsetSign);

        GameObject tempPrefabGO = null;
        GameObject cellGO = null;
        MapCell cell = null;
        Vector3 newCellPos = Vector3.zero;

        for (int index = 0; index < data.cellDataList.Count; index++)
        {
            MapCellData cellData = data.cellDataList[index];
            if (cellData == null) continue;

            E_ElementType elementType = (E_ElementType)cellData.cellType;

            if (prefabsPathDic.ContainsKey((int)elementType) == false || string.IsNullOrEmpty(prefabsPathDic[(int)elementType]))
            {
                Debug.LogError("[LoadMapTool]Load element prefab failed , elementType = " + elementType);
                continue;
            }
            //加载预制
            tempPrefabGO = Resources.Load<GameObject>(prefabsPathDic[(int)elementType]);
            if (tempPrefabGO == null)
            {
                Debug.LogError("[LoadMapTool]Load prefab failed, elementType = " + elementType);
                continue;
            }
            //创建GO
            cellGO = GameObject.Instantiate(tempPrefabGO);

            if (cellGO == null)
            {
                Debug.LogError("Get mapcell create mapCell go failed, create map failed ,cell id = " + cellData.cellID);
                continue;
            }
            cell = cellGO.AddComponentOnce<MapCell>();
            if (cell == null)
            {
                Debug.LogError("Get mapcell component failed, create map failed ,cell id = " + cellData.cellID);
                continue;
            }

            cell.CreatedInit(cellData.cellID);
            cell.SetPosition(cellData.cellPosition);
            cell.SetRotation(cellData.cellEuler);
            cell.SetGridPos(cellData.cellGridPos);
            cell.elementType = elementType;

            cell.gameObject.transform.SetParent(mapRootT);
        }

        if (GameConfig.Instance.sceneMapDataList != null)
        {
            SceneMapData newData = new SceneMapData();
            newData.mapName = mapXMLFileName;
            newData.mapSize = data.mapSize;
            newData.mapStartPoint = data.mapStartPoint;
            newData.elementSize = data.elementSize;
            for (int index = 0; index < GameConfig.Instance.sceneMapDataList.Count; index++)
            {
                SceneMapData mapD = GameConfig.Instance.sceneMapDataList[index];
                if (mapD == null) continue;
                if (mapD.mapName == mapXMLFileName)
                {
                    Debug.LogError("The sceneMapData had same name , name = " + mapXMLFileName);
                    newData.mapName = newData.mapName + UnityEngine.Random.Range(1, 100).ToString();
                }
            }
            GameConfig.Instance.sceneMapDataList.Add(newData);
        }

    }

    [ContextMenu("test method")]
    public void TestMethod()
    {
        Debug.LogError(GameConfig.Instance != null);
        //string str = "Assets/Resources/Prefabs";

        //Debug.LogError(str.IndexOf("Resources/"));
    }
    
}

public class MapInfoData
{

    public Vector2Int mapSize = Vector2Int.zero;
    public Vector3 mapStartPoint = Vector3.zero;
    public Vector3 elementSize = Vector3.zero;
    public int mapXOffsetSign;
    public int mapYOffsetSign;

    public List<MapCellData> cellDataList;
    public MapInfoData()
    {
        cellDataList = new List<MapCellData>();
    }

    public void AddMapCellData(MapCellData data)
    {
        if (data == null)
        {
            Debug.LogError("[MapInfoData] add mapCellData falied, data is null");
            return;
        }
        if (cellDataList == null) cellDataList = new List<MapCellData>();

        cellDataList.Add(data);
    }


    public bool IsCellDataNull()
    {
        return cellDataList == null || cellDataList.Count == 0;
    }
}

public class MapCellData
{
    public int cellID;
    public int cellType;
    public Vector3 cellPosition;
    public Vector3 cellEuler;
    public Vector3Int cellGridPos;
}