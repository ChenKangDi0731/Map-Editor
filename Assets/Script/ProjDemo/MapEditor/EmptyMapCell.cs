using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyMapCell : BaseMapCell
{

    #region component

    public Collider c;
    public SpriteRenderer r;

    #endregion component

    /// <summary>
    /// 是否底层辅助元素
    /// </summary>
    public bool isBaseCell
    {
        private set;
        get;
    }

    bool curSelectState = false;
    public Sprite emptyImage;
    public Sprite selectImage;

    #region lifeCycle

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion lifeCycle


    void ActiveCollider(bool active)
    {
        if (c == null) return;
        if (c.enabled != active)
        {
            c.enabled = active;
        }
    }

    #region 外部方法

    public override void DoInit(int id,params object[] args)
    {
        cellType = E_MapCellType.Empty;

        base.DoInit(id);

        //解除绑定
        UnregisterElement(true);
        UnregisterElement(false);

        SetSelect(true);
        SetSelect(false);

        ShowCell(true,true);

        SetBaseCellState(false);
    }

    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetElement()
    {

    }

    public void SetBaseCellState(bool state)
    {
        isBaseCell = state;
    }

    public override void SetSelect(bool isSelect)
    {
        if (curSelectState == isSelect) return;
        curSelectState = isSelect;
        if (r == null) return;
        Sprite changeTexture = isSelect ? selectImage : emptyImage;
        if (changeTexture != null)
        {
            r.sprite = changeTexture;
        }
    }


    public override void ShowCell(bool show, bool syncCollider = false)
    {
        if (r == null) return;
        if (isShowCell == show) return;
        isShowCell = show;

        if (r.enabled != show)
        {
            r.enabled = show;
        }
        if (syncCollider)
        {
            ActiveCollider(show);
        }

        //Debug.LogErrorFormat("Show Empty Cell {0} ,id = {1} ", show, CellID);
    }

    public override Vector3 GetElementPosition()
    {
        return transform.position;
    }

    public override void RegisterElement(BaseMapCell cell, bool isAboveElement = false)
    {
        base.RegisterElement(cell, isAboveElement);

        if (isAboveElement)
        {
            ShowCell(false,true);
        }
    }

    public override void RegisterElementCorrelate(BaseMapCell cell, bool isAboveElement = false)
    {
        base.RegisterElementCorrelate(cell, isAboveElement);

        if (isAboveElement)
        {
            ShowCell(false,true);
        }
    }

    public override void UnregisterElement(bool isAboveElement)
    {
        base.UnregisterElement(isAboveElement);

        if (isAboveElement)
        {
            ShowCell(true,true);
        }
    }

    public override void UnregisterElementCorrelate(bool isAboveElement)
    {
        base.UnregisterElementCorrelate(isAboveElement);

        if (isAboveElement)
        {
            ShowCell(true,true);
        }
    }


    #endregion 外部方法


}
