using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// unit 数据
/// </summary>
public class UnitData_Setup
{

    public static Dictionary<ulong, UnitData_Setup> data_list = new Dictionary<ulong, UnitData_Setup>();

    public static string Name()
    {
        return "UnitData_Setup.csv";
    }

    public static string Path()
    {
        return CSVHelper.Combine(Name());
    }

    public static UnitData_Setup GetData(int id)
    {
        ulong key = CSVHelper.GetKey(id);
        return null;
    }

    private static UnitData_Setup Get(ulong key)
    {
        if (!isLoad)
        {
            Load();
        }
        UnitData_Setup data;
        if (data_list.TryGetValue(key, out data))
        {
            return data;
        }
        return null;
    }

    private static bool isLoad = false;

    #region param

    #endregion param

    public static bool Load()
    {
        if (isLoad) return false;
        byte[] datas = CSVHelper.GetBytes(Path());

        return Load(datas);
    }

    private static bool Load(byte[] datas)
    {
        return true;
    }
}
