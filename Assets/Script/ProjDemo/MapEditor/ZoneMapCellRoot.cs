using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMapCellRoot : MonoBehaviour
{
    public int zoneID;

    [Header("Cellのタイプを変える")]
    public E_CellType originType = E_CellType.NonInteractive;
    public E_CellType targetType = E_CellType.NonInteractive;
    [ContextMenu("Change Cell Type")]
    public void ChangeCellType()
    {
        this.MapAllComponent<MapCell>(m =>
        {
            if (m != null)
            {
                if (m.CellType == originType)
                {
                    m.SetCellType(targetType);
                }
            }
        });
    }

    [Header("Cellの方向を変える")]
    public E_CellType changeDirCellType = E_CellType.NonInteractive;
    public E_CellDir changeDir = E_CellDir.None;
    [ContextMenu("Set Cell Direction")]
    public void ChangeCellDir()
    {
        this.MapAllComponent<MapCell>(m =>
        {
            if (m != null)
            {
                if (m.CellType == changeDirCellType)
                {
                    m.SetNextDir(changeDir);
                }
            }

        });
    }

    [ContextMenu("Reset Param")]
    public void ResetAllTestMethodParam()
    {
        originType = E_CellType.NonInteractive;
        targetType = E_CellType.NonInteractive;

        changeDirCellType = E_CellType.NonInteractive;
        changeDir = E_CellDir.None;
    }

}
