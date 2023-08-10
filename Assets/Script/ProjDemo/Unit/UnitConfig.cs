using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitConfig : MonoBehaviour
{
    private Unit _unit;
    public Unit unit
    {
        get { return _unit; }
        private set { _unit = value; }
    }

    public E_UnitType unitType;
    public Animator animator;

    public SpineRotControl spineControl;//部位旋转控制脚本【暂时放在此处，待优化】

    public virtual void DoInit(Unit u)
    {
        if (u == null)
        {
            Debug.LogError("[UnitConfig]Init failed , unit is null");
            return;
        }
        _unit = u;
        if (_unit.unitData != null)
        {
            _unit.unitData.unitType = unitType;
        }
    }

    public virtual void ClearData()
    {
        _unit = null;
    }

    #region 测试方法

    [ContextMenu("TestMethod/Set Unit Inactive")]
    public void SetUnitInactive()
    {
        _unit.unitData.curHp = 0;
        UnitManager.Instance.SetUnitInactive(_unit,true);
    }

    #endregion 测试方法
}
