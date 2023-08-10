using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DebugTools
{

    public static Dictionary<int, string> colorStr = new Dictionary<int, string> { { 0, "<color=#ff0000>" }, { 1, "<color=#00ff00>" }, { 2, "<color=#0000ff>" } };
    public static string endColorTagStr = "</color>";
    public static void LogError(string debugStr,DebugColor colorIndex=DebugColor.Black)
    {
        if (string.IsNullOrEmpty(debugStr))
        {
            Debug.LogError("debug Error");
            return;
        }
        string colorS = string.Empty;
        if(colorStr.TryGetValue((int)colorIndex,out colorS))
        {
            Debug.LogError(colorS + debugStr + endColorTagStr);
        }
        else
        {
            Debug.LogError(debugStr);
        }
        
    }

    public static void Log(string debugStr, DebugColor colorIndex = DebugColor.Black)
    {
        if (string.IsNullOrEmpty(debugStr))
        {
            Debug.LogError("debug Error");
            return;
        }
        string colorS = string.Empty;
        if (colorStr.TryGetValue((int)colorIndex, out colorS))
        {
            Debug.Log(colorS + debugStr + endColorTagStr);
        }
        else
        {
            Debug.Log(debugStr);
        }

    }

}

public enum DebugColor { 
    Red=0,
    Blue=1,
    Green=2,
    Black=3
}
