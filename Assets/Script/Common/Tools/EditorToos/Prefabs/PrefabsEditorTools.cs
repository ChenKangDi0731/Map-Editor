using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// <summary>
/// 全てのプリハブデータを読み込み
/// </summary>
public class PrefabsEditorTools : MonoBehaviour
{

    #region IDチェック

    [MenuItem("Customs Menu/Prefabs/Check All Prefabs")]
    public static bool CheckAllPrefabs()
    {
        //https://blog.csdn.net/a1256242238/article/details/72820155

        List<int> idList = new List<int>();
        List<string> repeatIdList = new List<string>();

        string commonPath = "/Resources/";
        string path = Application.dataPath + commonPath;
        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            string extension = ".prefab";

            for (int index = 0; index < files.Length; index++)
            {
                string name = files[index].Name;

                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                if (name.EndsWith(extension))
                {
                    if (name.Contains("#"))
                    {
                        int sharpStrIndex = name.IndexOf("#");
                        int extLastIndex = name.LastIndexOf(".");

                        name = name.Remove(extLastIndex);

                        if (name.Length > (sharpStrIndex + 1))
                        {
                            string subStr = name.Substring(sharpStrIndex + 1);
                            int prefabsId = -1;
                            try
                            {
                                prefabsId = int.Parse(subStr);

                                if (idList.Contains(prefabsId) == false)
                                {
                                    idList.Add(prefabsId);
                                }
                                else
                                {
                                    repeatIdList.Add(name);
                                }

                            }
                            catch
                            {
                                continue;
                            }
                        }


                    }
                }
            }

        }
        else
        {
            Debug.LogError("path does not exist");
        }

        if (repeatIdList.Count > 0)
        {
            //print

            for (int index = 0; index < repeatIdList.Count; index++)
            {
                if (string.IsNullOrEmpty(repeatIdList[index])) continue;
                Debug.LogErrorFormat("<color=#0000ff>has repeat prefabs id , id = {0} </color>", repeatIdList[index]);
            }

            return false;
        }

        return true;

    }

    public static bool CheckAllPrefabs(out List<EditorAssetsInfo> assetsInfoList)
    {
        //https://blog.csdn.net/a1256242238/article/details/72820155

        assetsInfoList = new List<EditorAssetsInfo>();

        List<int> idList = new List<int>();
        List<string> repeatIdList = new List<string>();

        string commonPath = "/Resources/";
        string path = Application.dataPath + commonPath;

        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int index = 0; index < files.Length; index++)
            {
                string name = files[index].Name;
                string extension = ".prefab";
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                if (name.EndsWith(extension))
                {
                    if (name.Contains("#"))
                    {
                        int sharpStrIndex = name.IndexOf("#");
                        int extLastIndex = name.LastIndexOf(".");

                        name = name.Remove(extLastIndex);

                        if (name.Length > (sharpStrIndex + 1))
                        {
                            string idStr = name.Substring(sharpStrIndex + 1);
                            string directoryStr = files[index].DirectoryName;

                            int prefabsId = -1;
                            try
                            {
                                prefabsId = int.Parse(idStr);

                                if (idList.Contains(prefabsId) == false)
                                {
                                    string subPath = directoryStr.Remove(0, path.Length);
                                    subPath = subPath.Replace("\\", "/");

                                    EditorAssetsInfo newInfo = new EditorAssetsInfo(prefabsId);
                                    newInfo.SetAssetName(name);
                                    newInfo.SetAssetPath(subPath);
                                    newInfo.SetAssetResourceExtension(extension);

                                    assetsInfoList.Add(newInfo);

                                    idList.Add(prefabsId);
                                }
                                else
                                {
                                    repeatIdList.Add(name);
                                }

                            }
                            catch
                            {
                                continue;
                            }
                        }


                    }
                }
            }

        }
        else
        {
            Debug.LogError("path does not exist");
        }

        if (repeatIdList.Count > 0)
        {
            //print

            for (int index = 0; index < repeatIdList.Count; index++)
            {
                if (string.IsNullOrEmpty(repeatIdList[index])) continue;
                Debug.LogErrorFormat("<color=#0000ff>has repeat prefabs id , id = {0} </color>", repeatIdList[index]);
            }

            return false;
        }

        return true;

    }


    #endregion IDチェック

    #region XMLで保存
    [MenuItem("Customs Menu/Prefabs/Register Prefabs")]
    public static void RegisterPrefabsResource()
    {

        List<EditorAssetsInfo> infoList = null;
        if(PrefabsEditorTools.CheckAllPrefabs(out infoList) == false)
        {
            infoList = null;
            return;
        }

        Debug.LogError("prefabs info count = " + infoList.Count);

        XMLAssetsRegistrar.WritePrefabsInfo(ref infoList);

        infoList.Clear();


    }

    #endregion XMLで保存
}

