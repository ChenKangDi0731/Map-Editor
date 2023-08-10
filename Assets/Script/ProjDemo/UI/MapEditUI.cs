using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditUI : BaseUI
{
    #region component
    public Button modeSwitchButton;
    public Text modeText;

    public Button elementSwitchLeftButton;
    public Button elementSwitchRightButton;
    public Text operateElementText;

    public InputField prefabID_InputField;

    public Button mapSaveButton;
    public InputField mapNameText;

    [SerializeField] Text operateText;

    #endregion component

[SerializeField]E_EditMode curEditMode = E_EditMode.Placement;
    int modeCount;
    int curEditModeIndex = -1;

    E_ElementType curElementType = E_ElementType.None;
    int elementTypeCount;
    int curOperateElementTypeIndex = -1;

    string defaultMapName = "defaultEmpty";
    string mapName;

    //string[] elementTypeStrings = null;
    List<string> elementTypeStrList = new List<string>();

    Dictionary<string, int> strMappingEnumDic = new Dictionary<string, int>();

    #region 其他参数

    Transform prefabPoint;
    [SerializeField] GameObject curPreviewPrefab;

    [SerializeField] Color operateColor_1 = Color.red;
    [SerializeField] Color operateColor_2 = Color.blue;
    bool operateColor_sign = true;
    #endregion 其他参数


    private void Awake()
    {
        //DoInit();
    }
    public override void DoInit()
    {
        base.DoInit();

        E_EditMode e = new E_EditMode();
        string[] values = System.Enum.GetNames(e.GetType());
        modeCount = values.Length - 1;

        E_ElementType e2 = new E_ElementType();
        string[] values2 = System.Enum.GetNames(e2.GetType());
        elementTypeCount = values2.Length - 1;
        try
        {
            if (strMappingEnumDic == null) strMappingEnumDic = new Dictionary<string, int>();
            for (int index = 0; index < values2.Length; index++)
            {
                strMappingEnumDic.Add(values2[index], (int)Enum.Parse(typeof(E_ElementType), values2[index]));
            }

            //elementTypeStrings = values2;
            elementTypeStrList = new List<string>(values2);

        }catch(Exception exception)
        {
            Debug.LogError(exception.Message);
        }

        SwitchOperateMode();

        RightSwitch();

        if (modeSwitchButton != null)
        {
            modeSwitchButton.onClick.AddListener(SwitchOperateMode);
        }

        curEditMode = (E_EditMode)curEditModeIndex;

        if (modeText != null)
        {
            modeText.text = curEditMode.ToString();
        }

        if (elementSwitchLeftButton != null)
        {
            elementSwitchLeftButton.onClick.AddListener(LeftSwitch);
        }
        if (elementSwitchRightButton != null)
        {
            elementSwitchRightButton.onClick.AddListener(RightSwitch);
        }

        if (prefabID_InputField != null)
        {
            prefabID_InputField.onEndEdit.AddListener(ChangeSelectedPrefabs);
        }

        if (mapNameText != null)
        {
            mapNameText.onEndEdit.AddListener(ChangeMapName);
        }
        if (mapSaveButton != null)
        {
            mapSaveButton.onClick.AddListener(SaveMapInfo);
        }
    }

    #region ui_method

    public void LeftSwitch()
    {
        curOperateElementTypeIndex = (curOperateElementTypeIndex + elementTypeCount - 1) % elementTypeCount;

        UpdateElementType();
    }

    public void RightSwitch()
    {
        curOperateElementTypeIndex = (curOperateElementTypeIndex + 1) % elementTypeCount;

        UpdateElementType();
    }

    void UpdateElementType()
    {
        string elementTypeStr = elementTypeStrList[curOperateElementTypeIndex];
        if (strMappingEnumDic.ContainsKey(elementTypeStr) == false)
        {
            Debug.LogError("Change element type failed");
            return;
        }

        curElementType = (E_ElementType)strMappingEnumDic[elementTypeStr];
        if (operateElementText != null)
        {
            operateElementText.text = curElementType.ToString();
        }

        MapEditorElementManager.Instance.ChangeOperateElement(curElementType);
        ChangePreviewPrefab();
    }

    void ChangePreviewPrefab()
    {
        if (curPreviewPrefab != null)
        {
            AssetManager.Instance.Destory(curPreviewPrefab.gameObject);
        }

        curPreviewPrefab = AssetManager.Instance.Create(MapEditorElementManager.Instance.GetCurOperateElementID());

        if (prefabPoint == null) {
            
            if (GameConfig.Instance.mapEditorOperateData.previewPrefabPoint == null)
            {
                Debug.LogError("[GameConfig]prefabs preview create point is null");
                return;
            }

            prefabPoint = GameConfig.Instance.mapEditorOperateData.previewPrefabPoint;
        }

        curPreviewPrefab.transform.SetParent(prefabPoint);
        curPreviewPrefab.transform.localPosition = Vector3.zero;
        curPreviewPrefab.transform.localRotation = Quaternion.identity;
        curPreviewPrefab.transform.localScale = Vector3.one;

    }

    public void SwitchOperateMode()
    {
        curEditModeIndex = (curEditModeIndex+1) % modeCount;

        curEditMode = (E_EditMode)curEditModeIndex;

        if (modeText != null)
        {
            modeText.text = curEditMode.ToString();
        }

        MapEditorManager.Instance.ChangeEditMode(curEditMode);
    }

    public void SaveMapInfo()
    {
        Debug.LogError("save map test");
        string saveMapName = string.IsNullOrEmpty(mapName) ? defaultMapName : mapName;

        XMLMapRegistrar.SetXMLName(saveMapName);

        XMLMapRegistrar.WriteMapInfo(EditorMapControl.Instance.GetMapSize(),
            EditorMapControl.Instance.GetMapStartPoint(),
            EditorMapControl.Instance.GetMapXOffsetSign(),
            EditorMapControl.Instance.GetMapYOffsetSign(),
            EditorMapControl.Instance.GetBaseElementSize(),
            EditorMapControl.Instance.GetMapElementDic());
    }

    public void ChangeSelectedPrefabs(string idStr)
    {
        if (string.IsNullOrEmpty(idStr))
        {
            return;
        }

        try
        {
            int id = int.Parse(idStr);
            E_ElementType changeType = (E_ElementType)id;

            if (elementTypeStrList.Contains(changeType.ToString())==false)
            {
                throw  new Exception();
            }

            if (strMappingEnumDic.ContainsKey(changeType.ToString()) == false)
            {
                throw new Exception();
            }

            //int changeID = strMappingEnumDic[changeType.ToString()];
            int changeIndex = elementTypeStrList.IndexOf(changeType.ToString());

            curOperateElementTypeIndex = changeIndex;

            UpdateElementType();

        }
        catch
        {
            Debug.LogError("input id cannot found");
        }
    }

    public void ChangeMapName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }
        mapName = name;

        Debug.LogError("change name = " + mapName);
    }

    #endregion ui_method

    public void SetOperateText(string str)
    {
        if (str == null) return;
        operateText.color = operateColor_sign ? operateColor_1 : operateColor_2;
        operateColor_sign = !operateColor_sign;
        operateText.text = str;
    }
}
