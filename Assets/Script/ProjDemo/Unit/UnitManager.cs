using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクター管理マネージャー（ゲームモード
/// ＊ゲームモードはマップをテストするために実装したものです＊
/// </summary>
public class UnitManager : Singleton<UnitManager>
{
    int defaultUnitIDBase = -1;
    int curUnitIDBase = -1;

    public Dictionary<int, Unit> unitDic = new Dictionary<int, Unit>();

    #region 外部方法

    public void DoInit()
    {
        if (unitDic == null) unitDic = new Dictionary<int, Unit>();
        unitDic.Clear();

        curUnitIDBase = defaultUnitIDBase;
    }


    public Unit CreateUnit(int unitPrefabsID,E_Group group=E_Group.None)
    {

        Unit u = new Unit();
        int id = ++curUnitIDBase;
        GameObject newUnitGO = AssetManager.Instance.Create(unitPrefabsID);
        if (newUnitGO == null)
        {
            Debug.LogError("[UnitManager]Create new unitGO failed, unitPrefabsID = " + unitPrefabsID);
            return null;
        }
        string name = newUnitGO.name;

        u.DoInit(id, name);
        u.BindGameObject(newUnitGO);
        u.SetGroup(group);

        if (unitDic.ContainsKey(id))
        {
            Debug.LogError("[UnitManager]create unit id repeat,plz check, id = " + id);
        }
        else
        {
            unitDic.Add(id, u);
        }

        return u;
    }

    public void DeleteUnit(int unitId)
    {

    }

    public void DeleteUnit(Unit unit)
    {

    }

    #region unit_operate

    public void SetUnitInactive(Unit u, bool unregisterUnit = false)
    {
        if (u == null) return;
        if (unitDic.ContainsKey(u.unitData.unitID))
        {
            u.SetUnitActiveState(false);
        }

        if (unregisterUnit)
        {
            GameObject unitGo = u.unitGo;

            ClearUnitData(u);
            unitDic.Remove(u.unitData.unitID);

            AssetManager.Instance.Destory(unitGo);
        }

    }

    public void SetUnitInactive(int unitId, bool unregisterUnit = false)
    {
        if (unitDic == null) return;
        Unit u = null;
        if (unitDic.TryGetValue(unitId, out u) == false)
        {
            Debug.LogError("[UnitManager]Set unit die state failed, unitId = " + unitId);
            return;
        }

        SetUnitInactive(u, unregisterUnit);
    }

    public void ClearInactiveUnit()
    {

    }

    #endregion unit_operate

    #region searchUnit

    public Unit GetUnitWithID(int id)
    {
        if (unitDic == null) return null;

        Unit u = null;
        if (unitDic.TryGetValue(id, out u) == false)
        {
            Debug.LogError("[UnitManager]Get unit manager failed, id = " + id);
        }

        return u;
    }

    public List<Unit> GetUnitsWithType(E_UnitType type, bool includeInactive = false)
    {
        List<Unit> unitList = new List<Unit>();
        if (unitDic != null)
        {

            foreach (var temp in unitDic)
            {
                if (temp.Value == null) continue;
                Unit cur = temp.Value;
                if (includeInactive == false && cur.unitActiveState == false) continue;

                if (cur.unitData.unitType == type)
                {
                    unitList.Add(cur);
                }
            }
        }
        return unitList;
    }

    public List<Unit> GetUnits(bool includeInactive = false)
    {
        List<Unit> unitList = new List<Unit>();
        if (unitDic != null)
        {

            if (includeInactive)
            {
                unitList.AddRange(unitDic.Values);
            }
            else
            {
                foreach (var temp in unitDic)
                {
                    if (temp.Value == null) continue;
                    Unit cur = temp.Value;
                    if (includeInactive && cur.unitActiveState == false) continue;

                    unitList.Add(cur);
                }
            }
        }

        return unitList;
    }

    #endregion searchUnit


    #endregion 外部方法

    void ClearUnitData(Unit u)
    {
        if (u == null) return;
        u.UnbindGameObject();
    }



    public int GetUnitCount()
    {
        if (unitDic == null) return 0;
        return unitDic.Count;
    }

}
