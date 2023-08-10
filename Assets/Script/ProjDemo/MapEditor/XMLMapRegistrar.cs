using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

public class XMLMapRegistrar
{
    static bool useRootFolder = true;
    static string xmlPath;
    static string xmlName;
    static string xmlExtension = ".xml";
    static string default_xmlName = "map_";
    static string tableRootStr="MapInfo";
    static string tableCreateTimeStr="CreateTime";
    static string mapSizeStr="MapSize";
    static string mapXOffsetSignStr = "MapXOffsetSign";
    static string mapYOffsetSignStr = "MapYOffsetSign";
    static string mapStartPointStr="MapStartPoint";
    static string mapBaseCellSizeStr="BaseCellSize";

    static string xStr="x";
    static string yStr="y";
    static string zStr="z";

    static string cellStr="Cell";
    static string cellIDAttributeStr="id";
    static string cellTypeAttributeStr="CellType";
    static string cellPosStr="CellPos";
    static string cellRotEulerStr = "CellRotEuler";
    static string cellGridPosStr= "CellGridPos";
    public static void SetXMLName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        xmlName = name;
    }

    /// <summary>
    /// 存储地图（XML）
    /// </summary>
    /// <param name="mapSize"></param>
    /// <param name="startPoint"></param>
    /// <param name="cellDic"></param>
    public static void WriteMapInfo(Vector2Int mapSize,Vector3 startPoint,bool mapXOffsetSign,bool mapYOffsetSign,Vector3 cellSize,Dictionary<int,MapElementCell> cellDic)
    {
        if (cellDic == null || cellDic.Count == 0) return;

        if(useRootFolder==false && string.IsNullOrEmpty(XMLMapRegistrar.xmlPath))
        {
            Debug.LogError("[XMLMapRegistrar] path not set");
            return;
        }

        if (string.IsNullOrEmpty(xmlName))
        {
            xmlName = default_xmlName + DateTime.Now.ToString();
        }

        string savePath = useRootFolder ? Application.streamingAssetsPath : Application.streamingAssetsPath + XMLMapRegistrar.xmlPath;
        savePath = savePath + "/" + XMLMapRegistrar.xmlName + XMLMapRegistrar.xmlExtension;

        XmlDocument xmlDoc = new XmlDocument();

        XmlDeclaration declaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
        xmlDoc.AppendChild(declaration);

        XmlElement root = xmlDoc.CreateElement(XMLMapRegistrar.tableRootStr);

        //创建时间
        XmlAttribute tableCreateTimeAttribute = xmlDoc.CreateAttribute(XMLMapRegistrar.tableCreateTimeStr);
        tableCreateTimeAttribute.Value = DateTime.Now.ToString();
        root.Attributes.Append(tableCreateTimeAttribute);

        //地图大小
        XmlElement mapSizeNode = xmlDoc.CreateElement(XMLMapRegistrar.mapSizeStr);
        XmlAttribute mapSizeX_attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.xStr);
        mapSizeX_attribute.Value = mapSize.x.ToString();
        XmlAttribute mapSizeY_attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.yStr);
        mapSizeY_attribute.Value = mapSize.y.ToString();
        XmlAttribute mapXOffset_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.mapXOffsetSignStr);
        mapXOffset_Attribute.Value = (mapXOffsetSign) ? "1" : "-1";
        XmlAttribute mapYOffset_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.mapYOffsetSignStr);
        mapYOffset_Attribute.Value = (mapYOffsetSign) ? "1" : "-1";
        mapSizeNode.Attributes.Append(mapSizeX_attribute);
        mapSizeNode.Attributes.Append(mapSizeY_attribute);
        mapSizeNode.Attributes.Append(mapXOffset_Attribute);
        mapSizeNode.Attributes.Append(mapYOffset_Attribute);

        //地图起始点
        XmlElement mapStartPointNode = xmlDoc.CreateElement(XMLMapRegistrar.mapStartPointStr);
        XmlAttribute mapStartPX_attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.xStr);
        mapStartPX_attribute.Value = startPoint.x.ToString();
        XmlAttribute mapStartPY_attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.yStr);
        mapStartPY_attribute.Value = startPoint.y.ToString();
        XmlAttribute mapStartPZ_attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.zStr);
        mapStartPZ_attribute.Value = startPoint.z.ToString();
        mapStartPointNode.Attributes.Append(mapStartPX_attribute);
        mapStartPointNode.Attributes.Append(mapStartPY_attribute);
        mapStartPointNode.Attributes.Append(mapStartPZ_attribute);

        //元素单位大小
        XmlElement cellBaseSizeNode = xmlDoc.CreateElement(XMLMapRegistrar.mapBaseCellSizeStr);
        XmlAttribute cellBaseSizeX_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.xStr);
        cellBaseSizeX_Attribute.Value = cellSize.x.ToString();
        XmlAttribute cellBaseSizeY_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.yStr);
        cellBaseSizeY_Attribute.Value = cellSize.y.ToString();
        XmlAttribute cellBaseSizeZ_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.zStr);
        cellBaseSizeZ_Attribute.Value = cellSize.z.ToString();
        cellBaseSizeNode.Attributes.Append(cellBaseSizeX_Attribute);
        cellBaseSizeNode.Attributes.Append(cellBaseSizeY_Attribute);
        cellBaseSizeNode.Attributes.Append(cellBaseSizeZ_Attribute);

        root.Attributes.Append(tableCreateTimeAttribute);
        root.AppendChild(mapSizeNode);
        root.AppendChild(mapStartPointNode);
        root.AppendChild(cellBaseSizeNode);
        xmlDoc.AppendChild(root);

        foreach (var cell in cellDic)
        {
            if (cell.Value == null) continue;

            XmlElement cellNode = xmlDoc.CreateElement(XMLMapRegistrar.cellStr);
            XmlAttribute cellIDAttribute = xmlDoc.CreateAttribute(XMLMapRegistrar.cellIDAttributeStr);
            cellIDAttribute.Value = cell.Value.CellID.ToString();

            //类型
            XmlAttribute cellPrefabTypeAttribute = xmlDoc.CreateAttribute(XMLMapRegistrar.cellTypeAttributeStr);
            cellPrefabTypeAttribute.Value = ((int)cell.Value.elementType).ToString();

            Vector3 cellPos = cell.Value.GetElementPosition();
            //位置
            XmlElement cellPosNode = xmlDoc.CreateElement(XMLMapRegistrar.cellPosStr);
            XmlAttribute cellPosX_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.xStr);
            cellPosX_Attribute.Value = cellPos.x.ToString();
            XmlAttribute cellPosY_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.yStr);
            cellPosY_Attribute.Value = cellPos.y.ToString();
            XmlAttribute cellPosZ_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.zStr);
            cellPosZ_Attribute.Value = cellPos.z.ToString();
            cellPosNode.Attributes.Append(cellPosX_Attribute);
            cellPosNode.Attributes.Append(cellPosY_Attribute);
            cellPosNode.Attributes.Append(cellPosZ_Attribute);

            //旋转
            Vector3 cellEuler = cell.Value.GetElementEulerAngles();
            XmlElement cellRotationNode = xmlDoc.CreateElement(XMLMapRegistrar.cellRotEulerStr);
            XmlAttribute cellRotEulerX_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.xStr);
            cellRotEulerX_Attribute.Value = cellEuler.x.ToString();
            XmlAttribute cellRotEulerY_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.yStr);
            cellRotEulerY_Attribute.Value = cellEuler.y.ToString();
            XmlAttribute cellRotEulerZ_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.zStr);
            cellRotEulerZ_Attribute.Value = cellEuler.z.ToString();
            cellRotationNode.Attributes.Append(cellRotEulerX_Attribute);
            cellRotationNode.Attributes.Append(cellRotEulerY_Attribute);
            cellRotationNode.Attributes.Append(cellRotEulerZ_Attribute);

            Vector3Int cellGridPos = cell.Value.GetGridPos();
            //地图位置
            XmlElement cellGridPosNode = xmlDoc.CreateElement(XMLMapRegistrar.cellGridPosStr);
            XmlAttribute cellGridPosX_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.xStr);
            cellGridPosX_Attribute.Value = cellGridPos.x.ToString();
            XmlAttribute cellGridPosY_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.yStr);
            cellGridPosY_Attribute.Value = cellGridPos.y.ToString();
            XmlAttribute cellGridPosZ_Attribute = xmlDoc.CreateAttribute(XMLMapRegistrar.zStr);
            cellGridPosZ_Attribute.Value = cellGridPos.z.ToString();
            cellGridPosNode.Attributes.Append(cellGridPosX_Attribute);
            cellGridPosNode.Attributes.Append(cellGridPosY_Attribute);
            cellGridPosNode.Attributes.Append(cellGridPosZ_Attribute);

            cellNode.Attributes.Append(cellIDAttribute);
            cellNode.Attributes.Append(cellPrefabTypeAttribute);
            cellNode.AppendChild(cellPosNode);
            cellNode.AppendChild(cellRotationNode);
            cellNode.AppendChild(cellGridPosNode);

            root.AppendChild(cellNode);
        }

        xmlDoc.Save(savePath);
        Debug.LogError("Save map path = " + savePath);
    }

    public static void LoadMapInfo(string xmlFileName,out MapInfoData mapData)
    {
        mapData = new MapInfoData();

        string filePath = useRootFolder ? Application.streamingAssetsPath : Application.streamingAssetsPath + XMLMapRegistrar.xmlPath;

        if (string.IsNullOrEmpty(xmlFileName))
        {
            filePath = filePath + "/" + XMLMapRegistrar.xmlName + XMLMapRegistrar.xmlExtension;
        }
        else
        {
            filePath = filePath + "/" + xmlFileName + XMLMapRegistrar.xmlExtension;
        }

        if (File.Exists(filePath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;

            XmlReader reader = XmlReader.Create(filePath, settings);

            xmlDoc.Load(reader);

            XmlNode root = xmlDoc.SelectSingleNode(XMLMapRegistrar.tableRootStr);
            if (root == null)
            {
                Debug.LogError("[XMLMapRegistrar]load map info failed");
                return;
            }

            XmlNodeList nodeList = root.ChildNodes;

            string idStr = string.Empty;
            string typeStr = string.Empty;
            string xS = string.Empty;
            string yS = string.Empty;
            string zS = string.Empty;

            foreach(XmlNode node in nodeList)
            {
                if (node == null) continue;
                try
                {
                    if (node.Name == XMLMapRegistrar.mapSizeStr)//地图大小
                    {
                        xS = node.Attributes[0].Value;
                        yS = node.Attributes[1].Value;

                        mapData.mapSize.x = int.Parse(xS);
                        mapData.mapSize.y = int.Parse(yS);

                        mapData.mapXOffsetSign = int.Parse(node.Attributes[2].Value);
                        mapData.mapYOffsetSign = int.Parse(node.Attributes[3].Value);

                    }
                    else if(node.Name == XMLMapRegistrar.mapStartPointStr)//地图起始点
                    {
                        xS = node.Attributes[0].Value;
                        yS = node.Attributes[1].Value;
                        zS = node.Attributes[2].Value;

                        mapData.mapStartPoint.x = float.Parse(xS);
                        mapData.mapStartPoint.y = float.Parse(yS);
                        mapData.mapStartPoint.z = float.Parse(zS);
                    }else if (node.Name == XMLMapRegistrar.mapBaseCellSizeStr)//地图基本元素大小
                    {
                        xS = node.Attributes[0].Value;
                        yS = node.Attributes[1].Value;
                        zS = node.Attributes[2].Value;

                        mapData.elementSize.x = float.Parse(xS);
                        mapData.elementSize.y = float.Parse(yS);
                        mapData.elementSize.z = float.Parse(zS);
                    }else if (node.Name == XMLMapRegistrar.cellStr)//元素信息
                    {
                        MapCellData cellData = new MapCellData();

                        idStr = node.Attributes[0].Value;
                        cellData.cellID = int.Parse(idStr);

                        typeStr = node.Attributes[1].Value;
                        cellData.cellType = int.Parse(typeStr);

                        XmlNodeList cellNodeList = node.ChildNodes;

                        foreach (XmlNode cellChildNode in cellNodeList)
                        {
                            if (cellChildNode == null) continue;
                            if (cellChildNode.Name == XMLMapRegistrar.cellPosStr)
                            {
                                xS = cellChildNode.Attributes[0].Value;
                                yS = cellChildNode.Attributes[1].Value;
                                zS = cellChildNode.Attributes[2].Value;

                                cellData.cellPosition.x = float.Parse(xS);
                                cellData.cellPosition.y = float.Parse(yS);
                                cellData.cellPosition.z = float.Parse(zS);
                            }
                            else if (cellChildNode.Name == XMLMapRegistrar.cellRotEulerStr)
                            {
                                xS = cellChildNode.Attributes[0].Value;
                                yS = cellChildNode.Attributes[1].Value;
                                zS = cellChildNode.Attributes[2].Value;

                                cellData.cellEuler.x = float.Parse(xS);
                                cellData.cellEuler.y = float.Parse(yS);
                                cellData.cellEuler.z = float.Parse(zS);
                            }
                            else if (cellChildNode.Name == XMLMapRegistrar.cellGridPosStr)
                            {
                                xS = cellChildNode.Attributes[0].Value;
                                yS = cellChildNode.Attributes[1].Value;
                                zS = cellChildNode.Attributes[2].Value;

                                cellData.cellGridPos.x = int.Parse(xS);
                                cellData.cellGridPos.y = int.Parse(yS);
                                cellData.cellGridPos.z = int.Parse(zS);
                            }
                        }

                        mapData.AddMapCellData(cellData);
                    }


                }catch(Exception e)
                {
                    Debug.LogError("[XMLMapRegistrar]load map info error, info = " + e.Message);
                    Debug.LogError("[XMLMapRegistrar]exception info = " + e.StackTrace);
                    mapData.cellDataList = null;
                    return;
                }

            }

        }
    }

}
