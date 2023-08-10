using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class OperateData:IBaseObjPoolCell
{

    public E_OperateType operateType
    {
        get;
        set;
    }

    private int cellID;
    public int CellID
    {
        get { return cellID; }
    }

    private int baseCellID;
    public int BaseCellID
    {
        get { return baseCellID; }
    }

    private Quaternion cellRotation;
    public Quaternion CellRot
    {
        get { return cellRotation; }
    }

    private int elementPrefabID;
    public int ElementPrefabID
    {
        get { return elementPrefabID; }
    }

    public OperateData()
    {
        operateType = E_OperateType.None;
    }

    public OperateData(E_OperateType type)
    {
        operateType = type;
    }

    public virtual void SetCellID(int id)
    {
        cellID = id;
    }

    public virtual void SetBaseCellID(int id)
    {
        baseCellID = id;
    }

    public virtual void SetCellRotation(Quaternion rot)
    {
        cellRotation = rot;
    }

    public virtual void SetElementPrefabID(int id)
    {
        elementPrefabID = id;
    }

    public virtual void SetData(OperateData data)
    {
        if (data == null) return;
        cellID = data.CellID;
        baseCellID = data.BaseCellID;
        cellRotation = data.CellRot;
        elementPrefabID = data.ElementPrefabID;
    }

    public virtual void Reset()
    {

    }

    public virtual void Recycle()
    {

    }

}

public class CreateElementOperateData : OperateData
{

    private int aboveEmptyCellId;
    public int AboveEmptyCellID
    {
        get { return aboveEmptyCellId; }
    }

    public CreateElementOperateData()
    {
        operateType = E_OperateType.CreateElement;
    }

    public override void SetData(OperateData data)
    {
        if (data == null || ((data is DeleteElementOperateData) == false && (data is CreateElementOperateData) == false)) return;
        if (data is DeleteElementOperateData)
        {
            DeleteElementOperateData tempData = data as DeleteElementOperateData;
            aboveEmptyCellId = tempData.AboveEmptyCellID;
        }
        if (data is CreateElementOperateData)
        {
            CreateElementOperateData tempData = data as CreateElementOperateData;
            aboveEmptyCellId = tempData.AboveEmptyCellID;
        }
        base.SetData(data);
    }

    public void SetAboveEmptyCellID(int id)
    {
        aboveEmptyCellId = id;
    }
}

public class DeleteElementOperateData : OperateData
{
    private int aboveEmptyCellId;
    public int AboveEmptyCellID
    {
        get { return aboveEmptyCellId; }
    }

    public DeleteElementOperateData()
    {
        operateType = E_OperateType.DeleteElement;
    }

    public override void SetData(OperateData data)
    {
        if (data == null || ((data is DeleteElementOperateData) == false && (data is CreateElementOperateData) == false)) return;
        if (data is DeleteElementOperateData)
        {
            DeleteElementOperateData tempData = data as DeleteElementOperateData;
            aboveEmptyCellId = tempData.AboveEmptyCellID;
        }
        if (data is CreateElementOperateData)
        {
            CreateElementOperateData tempData = data as CreateElementOperateData;
            aboveEmptyCellId = tempData.AboveEmptyCellID;
        }
        base.SetData(data);
    }

    public void SetAboveEmptyCellID(int id)
    {
        aboveEmptyCellId = id;
    }
}

public class MoveElementOperateData : OperateData
{
    public MoveElementOperateData()
    {
        operateType = E_OperateType.MoveElement;
    }
}

public class RotateElementOperateData : OperateData
{
    public RotateElementOperateData()
    {
        operateType = E_OperateType.RotateElement;
    }
}

public class ReplaceElementOperateData : OperateData
{
    public ReplaceElementOperateData()
    {
        operateType = E_OperateType.ReplaceElement;
    }
}

public enum E_OperateType
{
    None = 0,
    CreateElement = 1,
    DeleteElement = 2,
    RotateElement = 3,
    MoveElement = 4,
    ReplaceElement = 5,
}