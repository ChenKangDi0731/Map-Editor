using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プリハブ管理クラス
/// </summary>
public class PrefabsOperateTool : MonoBehaviour
{

    #region crystal
    public bool setChildNodeActive = true;
    public E_PrefabOperateStr curType = E_PrefabOperateStr.Red;
    string str = "CrystalGroup_";
    string idLineSign = "#";
    string id_1_Str = "3";
    string id_2_Str = "000";
    public string idStr = string.Empty;
    [Header("--------")]
    //public GameObject root;
    public int startIndex;
    public bool createRoot;

    [Header("--------")]
    public string formatStr = string.Empty;
    public int ParamCount;
    public int MaxParamCount = 10;
    public List<string> formatValueList = new List<string>(10);

    [ContextMenu("Operate1[change Name]")]
    public void Operate1()
    {
        if(formatValueList==null || formatValueList.Count < ParamCount)
        {
            Debug.LogError("Change name failed");
            return;
        }

        if (formatValueList.Count < MaxParamCount)
        {
            int tempCount = MaxParamCount - formatValueList.Count;
            for(int index = 0; index < tempCount; index++)
            {
                formatValueList.Add("null");
            }
        }

        string resultStr = string.Format(formatStr, 
            formatValueList[0], 
            formatValueList[1], 
            formatValueList[2],
            formatValueList[3],
            formatValueList[4],
            formatValueList[5],
            formatValueList[6],
            formatValueList[7],
            formatValueList[8],
            formatValueList[9]);

        this.gameObject.name = curType.ToString() + str + idStr + idLineSign + id_1_Str + ((int)curType).ToString() + id_2_Str + idStr;

        //Debug.LogError("Result print = " + resultStr);
        if (setChildNodeActive)
        {
            if (gameObject.transform.childCount>=1)
            {
                for(int index = 0; index < transform.childCount; index++)
                {
                    if (transform.GetChild(index) != null)
                    {
                        transform.GetChild(index).gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    [ContextMenu("Operate2[change Name]")]
    public void Operate2()
    {
        if (formatValueList == null || formatValueList.Count < ParamCount)
        {
            Debug.LogError("Change name failed");
            return;
        }


        if (transform.childCount == 0)
        {
            return;
        }

        int childCount = transform.childCount;
        ObjSign[] childTransforms = transform.GetComponentsInChildren<ObjSign>();

        for (int i = 0; i < childCount; i++)
        {
            Transform curChild = childTransforms[i].transform;

            GameObject go = curChild.gameObject;

            if (createRoot)
            {
                go = new GameObject();
                go.transform.position = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.transform.rotation = Quaternion.identity;

                go.transform.SetParent(this.gameObject.transform);

                curChild.SetParent(go.transform);
                curChild.localPosition = Vector3.zero;
                curChild.localScale = Vector3.one;
                curChild.localRotation = Quaternion.identity;
            }

            go.gameObject.name = curType.ToString() + str + (startIndex + i).ToString() + idLineSign + id_1_Str + ((int)curType).ToString() + id_2_Str + (startIndex + i).ToString();

            //Debug.LogError("Result print = " + resultStr);
            if (setChildNodeActive)
            {
                if (gameObject.transform.childCount >= 1)
                {
                    for (int index = 0; index < transform.childCount; index++)
                    {
                        if (transform.GetChild(index) != null)
                        {
                            transform.GetChild(index).gameObject.SetActive(true);
                        }
                    }
                }
            }

        }
    }

    [ExecuteInEditMode]
    [ContextMenu("clear objSign script")]
    public void ClearScript()
    {
        ObjSign[] childList = transform.GetComponentsInChildren<ObjSign>();
        for(int index = 0; index < childList.Length; index++)
        {
            DestroyImmediate(childList[index]);
        }
    }

    public enum E_PrefabOperateStr
    {
        Red=1,
        Blue=2,
        Green=3,
        Orange=4,
        Pink=5,
    }

    #endregion crystal

    #region cube

    [Header("cube_param")]
    public int cubeStartId;
    public bool cube_createRoot = true;
    public string cubeID_str_1 = "50000";

    [ContextMenu("cube_method1(change name)")]
    public void Cube_Method1()
    {

        ObjSign[] childList = transform.GetComponentsInChildren<ObjSign>();
        for(int index = 0; index < childList.Length; index++)
        {
            Transform curChild = childList[index].transform;

            GameObject curGo = curChild.gameObject;

            if (cube_createRoot)
            {
                GameObject go = new GameObject();
                curGo = go;
                go.transform.SetParent(this.transform);

                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;

                curChild.SetParent(go.transform);
                curChild.localPosition = Vector3.zero;
                curChild.localRotation = Quaternion.identity;
                curChild.localScale = Vector3.one;
            }
            
            curGo.name = curChild.name + idLineSign + cubeID_str_1 + (cubeStartId + index).ToString();
        }
    }

    #endregion cube


    #region other_method

    public static string prefabDirectory = "/Rock";
    public static string prefabExtension = ".prefab";
    public static string folderName = "Rock";

    [MenuItem("Customs Menu/Tools/GeneratePrefab")]
    public static void GeneratePrefab()
    {
        GameObject[] goList = Selection.gameObjects;
        Debug.Log("length = " + goList.Length);
        for(int index = 0; index < goList.Length; index++)
        {
            /*
            string selectedAssetPath = AssetDatabase.GetAssetPath(goList[index]);
            if (string.IsNullOrEmpty(selectedAssetPath))
            {
                continue;
            }
            */

            string path1 = "Assets/Resources/Prefabs";
            string pathCell2 = "/Terrains";
            string pathCell3 = "/"+folderName;
            string modelPath = string.Concat(path1, pathCell2, pathCell3);
            /*
            string modelPath = string.Concat("Assets/Resources/Prefabs/Terrain", prefabDirectory);
            if (!Directory.Exists(modelPath))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Prefabs/Terrain", "test");
            }
            */

            if (!Directory.Exists(path1))
            {
                if (CreateFolder("Assets/Resources", "Prefabs") == false) return;
                if (CreateFolder("Assets/Resources/Prefabs", "Terrains") == false) return;
                if (CreateFolder("Assets/Resources/Prefabs/Terrains", folderName) == false) return;
            }
            else if (!Directory.Exists(path1 + pathCell2))
            {
                if (CreateFolder("Assets/Resources/Prefabs", "Terrains") == false) return;
                if (CreateFolder("Assets/Resources/Prefabs/Terrains", folderName) == false) return;
            }
            else if (!Directory.Exists(path1 + pathCell2 + pathCell3))
            {
                if (CreateFolder("Assets/Resources/Prefabs/Terrains", folderName) == false) return;
            }

            GameObject cloneGO = GameObject.Instantiate<GameObject>(goList[index]);
            cloneGO.name = cloneGO.name.Replace("(Clone)", string.Empty);

            string generatePrefabFullName = string.Concat(modelPath, "/", cloneGO.name, prefabExtension);
            Object prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(cloneGO, generatePrefabFullName, InteractionMode.UserAction);

            if (prefab != null)
            {
                Debug.LogError("Generate Prefab, name = " + cloneGO.name);
            }

            DestroyImmediate(cloneGO);
        }
    }

    public static bool CreateFolder(string parentPath,string folderName)
    {
        if (string.IsNullOrEmpty(parentPath) || string.IsNullOrEmpty(folderName))
        {
            return false;
        }

        AssetDatabase.CreateFolder(parentPath, folderName);
        return true;
    }

    #endregion other_method

}
