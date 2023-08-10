using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マスデータ（マップのセール
/// </summary>
public class MapElementCell : BaseMapCell
{
    #region config_param

    ElementConfig config;
    public E_ElementType elementType;
    public Vector3 elementSize = Vector3.one;
    //public bool canSetElement = true;

    #endregion config_param

    List<int> baseEmptyCellIDList = new List<int>();

    [SerializeField]BaseMapCell aboveEmptyMapCell;

    bool isSelect = false;

    public override void DoInit(int id,params object[] args)
    {
        cellType = E_MapCellType.MapCell;

        base.DoInit(id);

        if (config == null) {
            config = this.gameObject.GetComponent<ElementConfig>();
        }
        if (config == null)
        {
            Debug.LogError("ElementConfig獲得エラー，id = " + id);
            return;
        }

        elementType = config.ElementType;
        elementSize = config.ElementSize;
        if (config.CanSetElement)
        {
            bool needSetEmptyCellId = aboveEmptyMapCell == null;

            if (aboveEmptyMapCell == null)
            {
                GameObject emptyCellGO = AssetManager.Instance.Create(EditorMapControl.Instance.EmptyElementID);
                if (emptyCellGO != null)
                {
                    aboveEmptyMapCell = emptyCellGO.GetComponent<EmptyMapCell>();
                }
            }
            if (aboveEmptyMapCell == null)
            {
                Debug.LogError("EmptyMapCell生成エラーが発生，mapElement id = " + id);
            }
            else
            {
                if (needSetEmptyCellId)
                {
                    aboveEmptyMapCell.DoInit(MapEditorElementManager.Instance.nextElementID);
                }
                else
                {
                    int emptyCellID = (int)args[0];
                    if (emptyCellID == -1)
                    {
                        Debug.LogErrorFormat("[{0}]マスidは-1，オブジェクトid = {1}", this.GetType().Name, cellID);
                        aboveEmptyMapCell.DoInit(MapEditorElementManager.Instance.nextElementID);
                    }
                    else
                    {
                        aboveEmptyMapCell.DoInit((int)args[0]);
                    }
                }
                EditorMapControl.Instance.RegisterEmptyCell((EmptyMapCell)aboveEmptyMapCell);

                aboveEmptyMapCell.SetLocalPosition(new Vector3(0, elementSize.y + 0.01f, 0) + transform.position);
                aboveEmptyMapCell.SetGridPos(this.GetGridPos());

                aboveEmptyMapCell.RegisterElement(this, false);

                ShowEmptyElement(true);
            }
        }
        else
        {
            UnregisterElementCorrelate(true);
            if (aboveEmptyMapCell != null)
            {
                AssetManager.Instance.Destory(aboveEmptyMapCell.gameObject);
                aboveEmptyMapCell = null;
            }
        }

        if (baseEmptyCellIDList == null) baseEmptyCellIDList = new List<int>();
        baseEmptyCellIDList.Clear();

        isSelect = false;
    }

    #region 内部方法

    void ShowEmptyElement(bool show)
    {
        if (aboveEmptyMapCell == null) return;

        aboveEmptyMapCell.ShowCell(show,true);
    }

    #endregion 内部方法

    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetPosition(EmptyMapCell cell)
    {
        if (cell == null) return;

        transform.position = cell.GetElementPosition();
        cell.RegisterElement(this,true);
    }

    public override void SetRotation(Quaternion rot)
    {
        Trans.rotation = rot;
    }

    public override void SetSelect(bool state)
    {
        if (isSelect == state) return;
        isSelect = state;
        Debug.Log("element set select true , elementID = " + cellID);
    }

    public override void ShowCell(bool show, bool syncCollider = false)
    {

    }

    public override Vector3 GetElementPosition()
    {
        return Trans.position;
    }

    public override void RegisterElement(BaseMapCell cell, bool isAboveElement = false)
    {
        if (cell == null) return;

        base.RegisterElement(cell, isAboveElement);

        if (isAboveElement)
        {
            ShowEmptyElement(false);
        }
    }

    public override void RegisterElementCorrelate(BaseMapCell cell, bool isAboveElement = false)
    {
        if (cell == null) return;

        base.RegisterElementCorrelate(cell, isAboveElement);

        if (isAboveElement)
        {
            ShowEmptyElement(false);
        }
    }

    public override void UnregisterElement(bool isAboveElement)
    {

        base.UnregisterElement(isAboveElement);

        if (isAboveElement)
        {
            if (aboveEmptyMapCell != null)
            {
                aboveEmptyMapCell.UnregisterElement(true);
            }
            ShowEmptyElement(true);
        }
    }

    public override void UnregisterElementCorrelate(bool isAboveElement)
    {

        base.UnregisterElementCorrelate(isAboveElement);

    }

    public void SetBaseEmptyCellID(int id)
    {
        if (baseEmptyCellIDList == null) baseEmptyCellIDList = new List<int>();
        if (baseEmptyCellIDList.Contains(id)) return;

        baseEmptyCellIDList.Add(id);
    }

    public void SetBaseEmptyCellID(List<int> idList)
    {
        baseEmptyCellIDList = idList;
    }

    public int GetBaseEmptyCellID()
    {
        if (baseEmptyCellIDList == null || baseEmptyCellIDList.Count == 0) return -1;

        return baseEmptyCellIDList[0];
    }

    public int GetAboveEmptyCellID()
    {
        if (aboveEmptyMapCell == null)
        {
            return -1;
        }

        return aboveEmptyMapCell.CellID;
    }

    public override void DeleteSelf()
    {
        ShowEmptyElement(false);
        if (aboveEmptyMapCell != null)
        {
            EditorMapControl.Instance.UnRegisterEmptyCell((EmptyMapCell)aboveEmptyMapCell);
        }

        if (belowCell == null)
        {
            for (int index = 0; index < baseEmptyCellIDList.Count; index++)
            {
                EditorMapControl.Instance.ShowEmptyCellByID(baseEmptyCellIDList[index], true);
            }
            baseEmptyCellIDList.Clear();
        }

        if (aboveCell != null)
        {
            BaseMapCell tempCell = aboveCell;
            UnregisterElementCorrelate(true);

            tempCell.DeleteSelf();
        }

        base.DeleteSelf();
    }
}
