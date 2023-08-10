using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo_TileManager : MonoBehaviour
{
    public List<Demo_Tile> tileList = new List<Demo_Tile>();
    public Demo_Tile startTile;
    // Start is called before the first frame update
    public void DoInit()
    {
        if (tileList == null) tileList = new List<Demo_Tile>();
        tileList.AddRange(this.GetComponentsInChildren<Demo_Tile>());
        Debug.Log("<color=#0000ff>get tile count = " + tileList.Count + "</color>");

        for(int index = 0; index < tileList.Count; index++)
        {
            if (tileList[index] == null) continue;
            if (tileList[index].isStartTile)
            {
                if (startTile != null)
                {
                    Debug.LogError("has couple start tile?");
                }
                startTile = tileList[index];
            }
        }
        if (startTile == null)
        {
            Debug.LogError("<color=#ff0000>start tile not set </color>");
        }

    }


    #region 外部方法

    public Demo_Tile GetStartTile()
    {
        return startTile;
    }

    #endregion 外部方法

}
