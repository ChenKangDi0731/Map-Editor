using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーンオブジェクトクラス
/// </summary>
public class BaseMapCell : MonoBehaviour
{
    [SerializeField]protected int cellID;

    [SerializeField] protected Vector3Int gridPos;

    public int CellID
    {
        get { return cellID; }
    }

    #region component_param

    private Transform cellTrans;
    public Transform Trans
    {
        get
        {
            if (cellTrans == null) cellTrans = GameObj.transform;
            return cellTrans;
        }
    }

    private GameObject cellGO;
    public GameObject GameObj
    {
        get { return this.gameObject; }
    }

    #endregion component_param


    public E_MapCellType cellType;

    public BaseMapCell aboveCell;
    public BaseMapCell belowCell;

    protected bool isShowCell = false;
    public virtual void DoInit(int id,params object[] args)
    {
        cellID = id;
        //this.GameObj.name = this.GameObj.name + "_" + id.ToString();
    }

    public virtual void SetGridPos(int x, int y,int z)
    {
        gridPos.x = x;
        gridPos.y = y;
        gridPos.z = z;
    }

    public virtual Vector3Int GetGridPos()
    {
        return gridPos;
    }

    public virtual void SetGridPos(Vector3Int pos)
    {
        gridPos = pos;
    }

    public virtual void SetPosition(Vector3 pos)
    {
        
    }

    public virtual void SetRotation(Quaternion rot)
    {

    }

    public virtual void SetLocalPosition(Vector3 pos)
    {
        transform.localPosition = pos;
    }

    public virtual void SetSelect(bool isSelect)
    {

    }

    public virtual void ShowCell(bool show,bool syncCollider = false)
    {

    }

    public virtual Vector3 GetElementPosition()
    {
        return transform.position;
    }

    public virtual Vector3 GetElementEulerAngles()
    {
        return transform.rotation.eulerAngles;
    }


    #region link_element_method


    public virtual void RegisterElement(BaseMapCell cell, bool isAboveElement = false)
    {
        if (cell == null) return;

        UnregisterElementCorrelate(isAboveElement);

        if (isAboveElement)
        {
            aboveCell = cell;
        }
        else
        {
            belowCell = cell;
        }
    }

    public virtual void RegisterElementCorrelate(BaseMapCell cell,bool isAboveElement = false)
    {
        if (cell == null) return;

        UnregisterElementCorrelate(isAboveElement);

        if (isAboveElement)
        {
            aboveCell = cell;
        }
        else
        {
            belowCell = cell;
        }

        cell.UnregisterElementCorrelate(!isAboveElement);
        cell.RegisterElement(this, !isAboveElement);
    }

    public virtual void UnregisterElement(bool isAboveElement)
    {
        //解绑
        if (isAboveElement)
        {
            if (aboveCell == null) return;
            aboveCell = null;
        }
        else
        {
            if (belowCell == null) return;
            belowCell = null;
        }
    }

    public virtual void UnregisterElementCorrelate(bool isAboveElement)
    {
        if (isAboveElement)
        {
            if (aboveCell == null) return;

            aboveCell.UnregisterElement(false);
            aboveCell = null;
        }
        else
        {
            if (belowCell == null) return;

            belowCell.UnregisterElement(true);
            belowCell = null;
        }
    }

    public virtual BaseMapCell GetAboveCell()
    {
        return aboveCell;
    }

    public virtual BaseMapCell GetBelowCell()
    {
        return belowCell;
    }

    #endregion link_element_method

    #region other_method

    public virtual void DeleteSelf()
    {
        AssetManager.Instance.Destory(this.gameObject);
    }

    #endregion other_method


}

public enum E_MapCellType
{
    Empty=0,
    MapCell=1,
}