using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// CSV解析类
/// </summary>
public class CSVHelper
{
    public enum e_csv_type
    {
        Full,  //全部版
        Simple,//精简版
    }

    public const string PATH_FOLDER_FULL = "Full";
    public const string PATH_FOLDER_SIMPLE = "Simple";
    public const string PATH_NAME = "Table";
    private static string PATH_PRE = "";

    static CSVHelper()
    {
        PATH_PRE = PATH_FOLDER_FULL;
        //Debug.Log("<color=#ffa500ff>默认数据类型为:" + e_csv_type.Full + "</color>");
    }

    public static void SetDataType(e_csv_type e_Csv_Type)
    {
        if (e_Csv_Type == e_csv_type.Full)
        {
            PATH_PRE = PATH_FOLDER_FULL;
        }
        else if (e_Csv_Type == e_csv_type.Simple)
        {
            PATH_PRE = PATH_FOLDER_SIMPLE;
        }
        Debug.Log("<color=#ffa500ff>当前数据类型为:" + e_Csv_Type + "</color>");
    }

    public static string Combine(string csv_name)
    {
        return Path.Combine(PATH_NAME, Path.Combine(PATH_PRE, csv_name));
    }

    public static string GetFolderPath(e_csv_type e_Csv_Type)
    {
        return Path.Combine(Application.streamingAssetsPath,Path.Combine(PATH_NAME, e_Csv_Type.ToString()));
    }

    public static byte[] GetBytes(string table_path)
    {
        //1.读取setup配置文件
        string fullPath = Path.Combine(Application.streamingAssetsPath,table_path).Replace('\\', '/');

        if (File.Exists(fullPath))
        {
            return File.ReadAllBytes(fullPath);
        }

       Debug.Log("CSVHelper.GetBytes ERROR, file not find, path="+fullPath);
        return null;
    }
    
    public static ulong GetKey(object key1)
    {
        return Convert.ToUInt64(key1);
    }

    public static ulong GetKey(object key1, object key2)
    {
        int bit1 = Marshal.SizeOf(key1);
        int bit2 = Marshal.SizeOf(key2);
        if (bit1 + bit2 > 8)
        {
           Debug.Log("键值出错");
        }

        return Convert.ToUInt64(key1)
            | (Convert.ToUInt64(key2) << (bit1 * 8));
    }

    public static ulong GetKey(object key1, object key2, object key3)
    {
        int bit1 = Marshal.SizeOf(key1);
        int bit2 = Marshal.SizeOf(key2);
        int bit3 = Marshal.SizeOf(key3);
        if (bit1 + bit2 + bit3 > 8)
        {
           Debug.Log("键值出错");
        }

        return Convert.ToUInt64(key1)
            | (Convert.ToUInt64(key2) << (bit1 * 8))
            | (Convert.ToUInt64(key3) << ((bit1 + bit2) * 8));
    }

    public static ulong GetKey(object key1, object key2, object key3, object key4)
    {
        int bit1 = Marshal.SizeOf(key1);
        int bit2 = Marshal.SizeOf(key2);
        int bit3 = Marshal.SizeOf(key3);
        int bit4 = Marshal.SizeOf(key4);
        if (bit1 + bit2 + bit3 + bit4 > 8)
        {
           Debug.Log("键值出错");
        }

        return Convert.ToUInt64(key1)
            | (Convert.ToUInt64(key2) << (bit1 * 8))
            | (Convert.ToUInt64(key3) << ((bit1 + bit2) * 8))
            | (Convert.ToUInt64(key4) << ((bit1 + bit2 + bit3) * 8));
    }

    public static ulong GetKey(object key1, object key2, object key3, object key4, object key5)
    {
        int bit1 = Marshal.SizeOf(key1);
        int bit2 = Marshal.SizeOf(key2);
        int bit3 = Marshal.SizeOf(key3);
        int bit4 = Marshal.SizeOf(key4);
        int bit5 = Marshal.SizeOf(key5);
        if (bit1 + bit2 + bit3 + bit4 + bit5 > 8)
        {
           Debug.Log("键值出错");
        }

        return Convert.ToUInt64(key1)
            | (Convert.ToUInt64(key2) << (bit1 * 8))
            | (Convert.ToUInt64(key3) << ((bit1 + bit2) * 8))
            | (Convert.ToUInt64(key4) << ((bit1 + bit2 + bit3) * 8))
            | (Convert.ToUInt64(key5) << ((bit1 + bit2 + bit3 + bit4) * 8));
    }
    
}
