using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Analytics;

/// <summary>
/// XMLツール
/// </summary>
public class XMLAssetsRegistrar
{
    static bool useRootFolder = true;
    static string xmlPath;
    static string xmlName="AssetInfoTables.xml";
    static string tableRootNodeName = "AssetInfoTable";
    static string assetNameNodeStr = "Name";
    static string assetExtensionNodeStr = "EXT";
    static string assetPathNodeStr = "Path";
    static string assetIDNodeStr = "ID";
    static string assetIDNodeAttributeStr = "id";

    public static void WritePrefabsInfo(ref List<EditorAssetsInfo> infoList)
    {
        if (infoList == null) return;

        if (XMLAssetsRegistrar.useRootFolder==false && string.IsNullOrEmpty(XMLAssetsRegistrar.xmlPath)) {
            Debug.LogError("[editor]------- XMLAssetsRegistrar path not set");
            return;
        }

        string savePath = useRootFolder ? Application.streamingAssetsPath : Application.streamingAssetsPath + XMLAssetsRegistrar.xmlPath;
        savePath = savePath +"/"+ XMLAssetsRegistrar.xmlName;

        XmlDocument xmlDoc = new XmlDocument();

        XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        xmlDoc.AppendChild(declaration);

        XmlElement root = xmlDoc.CreateElement("AssetInfoTable");
        XmlAttribute rootTimeAttribute = xmlDoc.CreateAttribute("Create_Time");
        rootTimeAttribute.Value = DateTime.Now.ToString();
        root.Attributes.Append(rootTimeAttribute);
        xmlDoc.AppendChild(root);
        EditorAssetsInfo info = null;
        for(int index = 0; index < infoList.Count; index++)
        {
            info = infoList[index];
            if (info == null || info.CheckInfo()==false) continue;

            XmlElement idNode = xmlDoc.CreateElement(XMLAssetsRegistrar.assetIDNodeStr);//id
            XmlElement nameNode = xmlDoc.CreateElement(XMLAssetsRegistrar.assetNameNodeStr);//名前
            XmlElement extensionNode = xmlDoc.CreateElement(XMLAssetsRegistrar.assetExtensionNodeStr);//拡張子
            XmlElement pathNode = xmlDoc.CreateElement(XMLAssetsRegistrar.assetPathNodeStr);//パス

            XmlAttribute idNodeAttribute = xmlDoc.CreateAttribute(XMLAssetsRegistrar.assetIDNodeAttributeStr);
            idNodeAttribute.Value = info.IDStr;
            idNode.Attributes.Append(idNodeAttribute);

            nameNode.InnerText = info.NameWithoutExtension;
            extensionNode.InnerText = info.Extension;
            pathNode.InnerText = info.PathWithoutName;

            idNode.AppendChild(nameNode);
            idNode.AppendChild(extensionNode);
            idNode.AppendChild(pathNode);

            root.AppendChild(idNode);
        }

        xmlDoc.Save(savePath);

    }

    public static void LoadPrefabsInfo(out Dictionary<int,AssetInfo> assetInfoDic)
    {

        assetInfoDic = new Dictionary<int, AssetInfo>();

        string filePath = useRootFolder ? Application.streamingAssetsPath : Application.streamingAssetsPath + xmlPath;
        filePath = filePath +"/"+ xmlName;

        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;

            XmlReader reader = XmlReader.Create(filePath, settings);

            xmlDoc.Load(reader);

            XmlNode root = xmlDoc.SelectSingleNode(XMLAssetsRegistrar.tableRootNodeName);
            if (root == null)
            {
                Debug.LogError("[load AssetInfoTable.xml] can not found root node ,node name = " + XMLAssetsRegistrar.tableRootNodeName);
                return;
            }

            XmlNodeList nodeList = root.ChildNodes;

            //node info
            /*
            string assetNameStr = string.Empty;
            string assetPathStr = string.Empty;
            string assetExtension = string.Empty;
            */
            string assetIDStr = string.Empty;
            int assetID = -1;

            int nodeIndex = 0;
            AssetInfo info = null;

            foreach(XmlNode node in nodeList)
            {
                if (node == null) continue;
                if (node.Name == XMLAssetsRegistrar.assetIDNodeStr)
                {
                    XmlNodeList assetNodeList = node.ChildNodes;

                    if (node.Attributes.Count == 0)
                    {
                        Debug.LogError("ID取得エラー,index = " + nodeIndex);
                        continue;
                    }

                    if (node.Attributes[0].Name != XMLAssetsRegistrar.assetIDNodeAttributeStr)
                    {
                        Debug.LogError("ID取得エラー,index = " + nodeIndex);
                        continue;
                    }

                    assetIDStr = node.Attributes[0].Value;
                    if(int.TryParse(assetIDStr, out assetID)==false)
                    {
                        Debug.LogError("------- AssetInfoTable読み込みエラー， inedx = " + nodeIndex);
                        continue;
                    }

                    info = new AssetInfo(assetID);

                    foreach (XmlNode assetNode in assetNodeList)
                    {
                        if (assetNode == null) continue;
                        if (assetNode.Name == XMLAssetsRegistrar.assetNameNodeStr)
                        {
                            //assetNameStr = assetNode.InnerText;
                            info.SetAssetName(assetNode.InnerText);
                        }else if (assetNode.Name == XMLAssetsRegistrar.assetPathNodeStr)
                        {
                            //assetPathStr = assetNode.InnerText;
                            info.SetAssetPath(assetNode.InnerText);
                        }else if (assetNode.Name == XMLAssetsRegistrar.assetExtensionNodeStr)
                        {
                            //assetExtension = assetNode.InnerText;
                            info.SetAssetResourceExtension(assetNode.InnerText);
                        }
                    }

                    if (assetInfoDic.ContainsKey(assetID) == false)
                    {
                        assetInfoDic.Add(assetID, info);
                    }
                    else
                    {
                        Debug.LogError("AssetInfoデータエラー，ID = " + assetID);
                    }

                }

                nodeIndex++;

            }



        }
        else
        {
            Debug.LogError("can not found AssetsInfoTable.xml， path = "+filePath);
        }
    }

}

public class EditorAssetsInfo
{
    /// <summary>
    /// 预制ID
    /// </summary>
    private int prefabId;
    public int ID { get { return prefabId; } }

    /// <summary>
    /// 预制ID字符串
    /// </summary>
    public string IDStr { get { return ID.ToString(); } }

    /// <summary>
    /// 资源名字
    /// </summary>
    public string FullName { get { return NameWithoutExtension + Extension; } }

    /// <summary>
    /// 资源名字（不带扩展名）
    /// </summary>
    private string assetNameWithoutExtension;
    public string NameWithoutExtension { get { return assetNameWithoutExtension; } }

    /// <summary>
    /// 资源扩展名
    /// </summary>
    private string assetExtension;
    public string Extension { get { return assetExtension; } }

    /// <summary>
    /// 资源路径（带文件名）
    /// </summary>
    private string assetPathWithoutName;
    public string PathWithoutName { get { return assetPathWithoutName; } }

    /// <summary>
    /// 资源完整路径
    /// </summary>
    public string FullPath { get { return PathWithoutName + FullName; } }

    public EditorAssetsInfo(int id)
    {
        prefabId = id;
    }

    #region 设置资源信息

    public void SetAssetName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogErrorFormat("<color=#00ff00> ---------- Asset name string is null ,id = {0} </color>", ID);
            return;
        }

        assetNameWithoutExtension = name;
    }

    public void SetAssetResourceExtension(string ext)
    {
        if (string.IsNullOrEmpty(ext))
        {
            Debug.LogErrorFormat("<color=#00ff00> ---------- Asset extension string is null ,id = {0} </color>", ID);
            return;
        }

        assetExtension = ext;
    }

    public void SetAssetPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogErrorFormat("<color=#00ff00> ---------- Asset path string is null ,id = {0} </color>", ID);
            return;
        }

        assetPathWithoutName = path;
    }


    #endregion 设置资源信息

    //检查是否有空字符串
    public bool CheckInfo()
    {
        if(string.IsNullOrEmpty(IDStr) || string.IsNullOrEmpty(assetNameWithoutExtension) || string.IsNullOrEmpty(assetExtension) || string.IsNullOrEmpty(assetPathWithoutName))
        {
            return false;
        }

        return true;
    }

}