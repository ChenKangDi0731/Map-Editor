using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// バトルツール（AI用（ゲームモード用
/// </summary>
public static class BattleTools
{


    #region BehaviourTree_Method

    public static Unit SearchNearestUnit(E_UnitType type, float radius, Unit ownerUnit, Vector3 searchCenter, bool useSearchCenter=false, bool includeInactive=false)
    {
        if (radius <= 0) return null;
        if (ownerUnit == null) return null;

        List<Unit> unitList = null;
        if (type == E_UnitType.None)
        {
            unitList = UnitManager.Instance.GetUnits(includeInactive);
        }
        else
        {
            unitList = UnitManager.Instance.GetUnitsWithType(type, includeInactive);
        }
        if (unitList.Count == 0) return null;

        List<Unit> tempList = new List<Unit>();
        Unit targetUnit = null;

        for(int index = 0; index < unitList.Count; index++)
        {
            Unit cur = unitList[index];
            if (cur == null || cur==ownerUnit) continue;

            if ((cur.unitTrans.position - searchCenter).magnitude <= radius)
            {
                tempList.Add(cur);
            }
        }

        //待完善
        for(int index = 0; index < tempList.Count; index++)
        {
            Unit cur = tempList[index];
            if (cur == null) continue;

            //检查unit是否在攻击范围内的grid

        }

        if (tempList.Count > 0)
        {
            targetUnit = tempList[0];
        }

        return targetUnit;
    }

    public static Unit SearchNearestUnit(List<E_UnitType> typeList, float radius, Unit ownerUnit, Vector3 searchCenter, bool useSearchCenter=false,bool includeInactive = false)
    {
        if (typeList == null || typeList.Count == 0)
        {
            return null;
        }

        if (radius <= 0) return null;
        if (ownerUnit == null) return null;

        List<Unit> unitList = null;

        for (int typeIndex = 0; typeIndex < typeList.Count; typeIndex++)
        {

            if (typeList[typeIndex] == E_UnitType.None)
            {
                unitList = UnitManager.Instance.GetUnits(includeInactive);
                break;
            }
            else
            {
                unitList = new List<Unit>();
                Unit[] unitArray = UnitManager.Instance.GetUnitsWithType(typeList[typeIndex], includeInactive).ToArray();
                unitList.AddRange(unitArray);
            }

        }

        if (unitList == null || unitList.Count == 0) return null;

        List<Unit> tempList = new List<Unit>();
        Unit targetUnit = null;

        if (useSearchCenter==false)
        {
            searchCenter = ownerUnit.unitTrans.position;
        }

        for (int index = 0; index < unitList.Count; index++)
        {
            Unit cur = unitList[index];
            if (cur == null || cur == ownerUnit || cur.unitData.unitGroup == ownerUnit.unitData.unitGroup) continue;
            if ((cur.unitTrans.position - searchCenter).magnitude <= radius)
            {
                tempList.Add(cur);
            }
        }

        for (int index = 0; index < tempList.Count; index++)
        {
            Unit cur = tempList[index];
            if (cur == null) continue;

        }

        if (tempList.Count > 0)
        {
            targetUnit = tempList[0];
        }

        return targetUnit;
    }

    public static SceneCrystal SearchNearestCrystal(Unit ownerUnit,float searchRadius,E_Group searchGroup, Vector3 searchCenter,bool useSearchCenter = false)
    {

        if (ownerUnit == null) return null;
        if (GameSceneManager.Instance.curScene == null)
        {
            Debug.LogError("[SearchNearestCrystal]search crystal failed, cur scene is null");
            return null;
        }

        if (useSearchCenter == false)
        {
            searchCenter = ownerUnit.unitTrans.position;
        }

        Dictionary<int, SceneCrystal>.Enumerator enumerator = GameSceneManager.Instance.curScene.GetEnumeratorWithGroup(searchGroup);

        SceneCrystal targetCrystal = null;
        float nearestDist = searchRadius;
        while (enumerator.MoveNext())
        {
            SceneCrystal cur = enumerator.Current.Value;
            if (cur != null)
            {
                float dist = (cur.transform.position - searchCenter).magnitude;
                if (dist <= nearestDist)
                {
                    targetCrystal = cur;
                    nearestDist = dist;
                }
            }
        }

        return targetCrystal;
    }


    #endregion BehaviourTree_Method

    public static void Attack(Unit attacker, Unit target)
    {
        if (attacker == null || target == null)
        {
            Debug.LogError("[BattleTools]unit is null ,attack operate failed");
            return;
        }

        int attackAtk = attacker.unitData.Atk;
        target.unitData.curHp -= attackAtk;
        //unit die
        if (target.unitData.curHp <= 0)
        {
            UnitManager.Instance.SetUnitInactive(target, false);
        }

    }

}
