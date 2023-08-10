using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Data
{

    private int _unitID;
    public int unitID
    {
        get { return _unitID; }
        private set { _unitID = value; }
    }

    private string _unitName;
    public string unitName
    {
        get { return _unitName; }
        private set { _unitName = value; }
    }

    private Unit _unit;
    public Unit unit
    {
        get { return _unit; }
        private set { _unit = value; }
    }

    public E_Group unitGroup;
    public E_UnitType unitType;

    //hp
    private int _maxHp;
    public int maxHP { get { return _maxHp; } }
    private int _curHp;
    public int curHp
    {
        get { return _curHp; }
        set
        {
            if (value < 0) _curHp = 0;
            else _curHp = Mathf.Clamp(value, 0, maxHP);
        }
    }

    //atk
    public int Atk { get; set; }

    public virtual void DoInit(Unit parentUnit, int unitID, string name)
    {
        if (parentUnit == null || string.IsNullOrEmpty(name))
        {
            Debug.LogError("[Unit_Data]init component failed");
            return;
        }
        unit = parentUnit;
        unitName = name;
    }

    /// <summary>
    /// 初始化unit数据（从DataManager中读取
    /// </summary>
    public void InitData()
    {
        _maxHp = DataManager.Instance.GetHpValue(unitType);
        _curHp = _maxHp;

        Atk = DataManager.Instance.GetAtkValue(unitType);
    }

    public virtual void SetInactive()
    {

    }
}
