using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using UnityEngine;
/// <summary>
/// 
/// </summary>
public class MapEditorElementManager : MonoSingleton<MapEditorElementManager>
{
    Dictionary<int, EditorElementData> elementDataDic = new Dictionary<int, EditorElementData>();
    List<int> elementTypeList = new List<int>();

    #region element_param

    #region placementMode_param

    int curElementPrefabID;
    int curElementIndex;
    E_ElementType curElementType;
    string curElementStr = string.Empty;

    #endregion placementMode_param

    #region operateMode_param

    MapElementCell curOperateCell;


    #endregion operateMode_param


    int elementIDCounter = 0;
    public int nextElementID
    {
        get { return elementIDCounter++; }
    }

    #endregion element_param

    #region ui参数

    public Rect buttonRect = new Rect(20, 20, 200, 50);

    #endregion ui参数

    #region lifeCycle

    public void DoUpdate(float deltaTime)
    {
       
    }

    public void DoFixedUpdate(float fixedUpdateTime)
    {

    }

    private void OnGUI()
    {
        return;
        if (GUI.Button(buttonRect, curElementStr))
        {
            ChangeOperateElement(true);
        }
    }

    #endregion lifeCycle

    void ChangeOperateElement(bool changeForward)
    {
        if (elementTypeList == null || elementTypeList.Count == 0)
        {
            Debug.LogError("[MapEditorElementManager]リスト取得エラー");
            return;
        }

        curElementIndex += (changeForward) ? elementTypeList.Count + 1 : elementTypeList.Count - 1;
        curElementIndex %= elementTypeList.Count;

        curElementType = (E_ElementType)elementTypeList[curElementIndex];
        if (elementDataDic.ContainsKey((int)curElementType) && elementDataDic[(int)curElementType] != null)
        {
            curElementPrefabID = elementDataDic[(int)curElementType].elementID;
            curElementStr = curElementType.ToString();
        }
        else
        {
            Debug.LogError("[MapEditorElementManager]オブジェクト切り替えエラー = " + curElementType);
        }
    }
   
    #region 外部方法

    public void DoInit(GameObject goRoot = null)
    {
        if (goRoot != null)
        {
            this.transform.SetParent(goRoot.transform);
        }

        EditorMapControl.Instance.DoInit();
        EditorMapControl.Instance.SetElementRoot(GameConfig.Instance.sceneObjRoot);
        InitOperateElementData();

        curElementIndex = -1;
        ChangeOperateElement(true);
    }


    void InitOperateElementData()
    {
        if (elementDataDic == null) elementDataDic = new Dictionary<int, EditorElementData>();
        elementDataDic.Clear();
        if (elementTypeList == null) elementTypeList = new List<int>();
        elementTypeList.Clear();

        if (MapEditorManager.Instance.operateData != null)
        {
            GameObject curObj = null;
            ElementConfig config = null;

            List<int> operateElementIDList = MapEditorManager.Instance.operateData.EditorElementIDList;
            for (int index = 0; index < operateElementIDList.Count; index++)
            {
                curObj = AssetManager.Instance.Create(operateElementIDList[index]);
                if (curObj == null)
                {
                    continue;
                }

                config = curObj.GetComponent<ElementConfig>();
                if (config == null)
                {
                    Debug.LogError("ElementConfig獲得エラー, id =" + operateElementIDList[index]);
                    continue;
                }

                //检查是否已经添加过相同类型的元素
                if (elementDataDic.ContainsKey((int)config.ElementType))
                {
                    Debug.LogError("データはもう存在してる， id = " + operateElementIDList[index]);
                    continue;
                }

                //计算元素大小，添加数据
                Collider c = curObj.GetComponent<Collider>();
                if (c == null)
                {
                    Debug.LogError("コライダー獲得エラー,id = " + operateElementIDList[index]);
                    continue;
                }

                Vector3 elementSize = Vector3.one;
                Vector3 elementColliderCenter = Vector3.zero;
                Vector3 elementLocalScale = curObj.transform.localScale;
                if(c is BoxCollider)
                {
                    elementSize = (c as BoxCollider).size;
                    elementColliderCenter = (c as BoxCollider).center;
                }
                else if(c is CapsuleCollider)
                {
                    elementSize = (c as CapsuleCollider).bounds.size;
                    elementColliderCenter = (c as CapsuleCollider).center;
                }
                else
                {
                    elementSize = c.bounds.size;
                    elementColliderCenter = Vector3.zero;
                }
                elementSize.x *= elementLocalScale.x;
                elementSize.y *= elementLocalScale.y;
                elementSize.z *= elementLocalScale.z;

                EditorElementData data = new EditorElementData(config.ElementType, operateElementIDList[index]);
                data.SetSize(elementSize);
                data.SetCenter(elementColliderCenter);

                Debug.LogWarningFormat("<color=#0000ff>elemnt type = {0} , element Size = {1} </color>", config.ElementType, elementSize);

                elementDataDic.Add((int)config.ElementType, data);
                if (elementTypeList.Contains((int)config.ElementType) == false)
                {
                    elementTypeList.Add((int)config.ElementType);
                }
                
                AssetManager.Instance.Destory(curObj);

            }
        }


        Debug.LogWarning("element data dic count  = " + elementDataDic.Count);
    }

    EditorElementData GetElementDataByID(int id)
    {
        if (elementDataDic == null || elementDataDic.Count == 0)
        {
            Debug.LogError("データ存在しない， id = " + id);
            return null;
        }

        EditorElementData returnData = null;
        if (elementDataDic.TryGetValue(id, out returnData) == false)
        {
            Debug.LogError("データ獲得エラー，id = " + id);
        }

        return returnData;
    }

    #region element_placementMode

    public GameObject CreateElement(Vector3 createPos)
    {
        return null;
    }
    
    public GameObject CreateElement(EmptyMapCell cell,params object[] args)
    {
        if (cell == null) return null;

        return EditorMapControl.Instance.CreateElement(cell,curElementPrefabID,args);
    }


    public void DeleteElement(MapElementCell cell)
    {
        if (cell == null)
        {
            return;
        }

    }


    public void ChangeOperateElement(E_ElementType changeType)
    {
        if (elementTypeList == null || elementTypeList.Count == 0)
        {
            Debug.LogError("[MapEditorElementManager]操作オブジェクトの切り替えにエラーが発生");
            return;
        }

        curElementType = changeType;

        if (elementDataDic.ContainsKey((int)curElementType) && elementDataDic[(int)curElementType] != null)
        {
            curElementPrefabID = elementDataDic[(int)curElementType].elementID;
            curElementStr = curElementType.ToString();
        }

    }

    public void ChangeOperateElement(int changeTypeIndex)
    {
        ChangeOperateElement((E_ElementType)changeTypeIndex);
    }

    #endregion element_placementMode

    #region element_operateMode

    public void SelectElement(MapElementCell cell)
    {
        if (cell == null) return;

        curOperateCell = cell;
        Debug.LogError("select cell, cell id = " + cell.CellID);
    }

    public void ResetOperation()
    {

    }

    #endregion element_operateMode

    public int GetCurOperateElementID()
    {
        return curElementPrefabID;
    }

    public int GetElementPrefabID(E_ElementType elementType)
    {
        if (elementDataDic.ContainsKey((int)elementType) && elementDataDic[(int)elementType]!=null)
        {
            return elementDataDic[(int)elementType].elementID;
        }
        else
        {
            Debug.LogErrorFormat("[{0}]マップの中タイプ{1}が存在しない，ID獲得エラー", this.GetType().Name, elementType);
            return -1;
        }
    }

    #endregion 外部方法

}


public class EditorElementData
{

    E_ElementType elementType;
    private int elementPrefabID;
    public int elementID
    {
        get { return elementPrefabID; }
    }

    private Vector3 size;
    public Vector3 elementSize
    {
        private set { size = value; }
        get
        {
            return size;
        }
    }

    private Vector3 colliderCenter;
    public Vector3 center
    {
        private set { colliderCenter = value; }
        get
        {
            return colliderCenter;
        }
    }

    public float colliderX
    {
        get
        {
            return center.x + elementSize.x;
        }
    }

    public float colliderY
    {
        get
        {
            return center.y + elementSize.y;
        }
    }

    public float colliderZ
    {
        get
        {
            return center.z + elementSize.z;
        }
    }

    public EditorElementData(E_ElementType type,int id)
    {
        elementType = type;
        elementPrefabID = id;
    }


    public void SetSize(Vector3 s)
    {
        elementSize = s;
    }

    public void SetCenter(Vector3 c)
    {
        center = c;
    }


}