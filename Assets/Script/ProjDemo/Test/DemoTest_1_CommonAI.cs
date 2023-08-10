using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion;


#region Init_Operate


[Name("Init Test AI")]
[Category("DemoTest/Common")]
public class InitTestAI : ActionTask<Transform>
{
    public BBParameter<Unit> unit;

    public BBParameter<SpawnPointInfo> spawnPoint;
    public BBParameter<MapCell> startCell;

    public BBParameter<bool> useCellTrans;

    //public BBParameter<int> maxHp;
    //public BBParameter<int> curHp;
    public BBParameter<int> atk;


    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (unit.value == null)
        {
            Debug.LogError("[InitTestAI]init test ai failed, unit is null");
            EndAction(false);
            return;
        }


        //初始化出生点信息
        if (spawnPoint.value == null || agent == null)
        {
            Debug.LogError("[InitTestAI]init test ai failed");
            EndAction(false);
            return;
        }

        if (spawnPoint.value.useCell == false || spawnPoint.value.bindCellID < 0)
        {
            Debug.LogError("[InitTestAI]init test ai failed, spawnPoint info error, spawnPointId = " + spawnPoint.value.PointID);
            EndAction(false);
            return;
        }

        startCell.value = MapManager.Instance.GetCellWithID(spawnPoint.value.bindCellID);

        if (useCellTrans.value == true)
        {
            if (startCell.value == null)
            {
                Debug.LogError("[InitTestAI]init test ai failed, get SpawnCell failed , spawnPointID = " + spawnPoint.value.PointID);
                EndAction(false);
                return;
            }

            Vector3 targetDir = Vector3.forward;
            if (startCell.value.GetNextCellDir(out targetDir))
            {
                this.agent.forward = targetDir;
            }
            this.agent.position = startCell.value.GetRoadPosition();

        }
        else
        {
            this.agent.position = spawnPoint.value.pointTrans.position;
            this.agent.forward = spawnPoint.value.pointTrans.forward;
        }

        if (unit.value.unitData != null)
        {
            //maxHp.value = unit.value.unitData.maxHP;
            //curHp.value = maxHp.value;
            atk.value = unit.value.unitData.Atk;
        }


        EndAction(true);
    }

}


#endregion Init_Operate

#region Movement

[Name("Find Next Road")]
[Category("DemoTest/Normal")]
public class FindNextRoad : ActionTask<Transform>
{
    public BBParameter<MapCell> startCell;
    public BBParameter<float> moveSpeed;
    public BBParameter<float> rotSpeed;

    public BBParameter<bool> curMovementState = false;

    MapCell curMoveStartCell;
    public BBParameter<MapCell> curMoveEndCell;
    public BBParameter<bool> needAdjustLastDir;
    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 curTargetDir;
    public float moveTime;
    public float curMoveTime = 0;

    bool moving = false;
    bool adjustTargetDir = false;
    Vector3 lastDir = Vector3.forward;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (agent == null)
        {
            Debug.LogError("[DemoTest_1_CactusMonsterAI]unit transform is null, move action failed");
            EndAction(false);
            return;
        }

        if (startCell.value == null)
        {
            Debug.LogError("[DemoTest_1_CactusMonsterAI]move start cell is null, plz check");
            EndAction(false);
            return;
        }

        curMovementState.value = true;

        curMoveStartCell = startCell.value;
        if (UpdateMovementRoad(curMoveStartCell) == false)
        {
            Debug.LogError("[DemoTest_1_CactusMonsterAI]can not get next cell, plz check");
            EndAction(false);
            return;
        }

        this.agent.forward = curTargetDir;
        adjustTargetDir = false;
        moving = true;
    }

    protected override void OnUpdate()
    {
        if (curMovementState.value)
        {
            if (moving)
            {
                curMoveTime += Time.deltaTime;
                curMoveTime = Mathf.Clamp(curMoveTime, 0, moveTime);
                float lerpValue = curMoveTime / moveTime;
                this.agent.position = Vector3.Lerp(startPosition, endPosition, lerpValue);
                this.agent.forward = Vector3.Lerp(this.agent.forward, curTargetDir, Time.deltaTime * rotSpeed.value);

                if (curMoveTime >= moveTime)
                {
                    if (UpdateMovementRoad(curMoveEndCell.value) == false)
                    {
                        StopMovement();
                    }
                }
            }
            else if (moving == false && adjustTargetDir == true)
            {
                this.agent.forward = Vector3.Lerp(this.agent.forward, lastDir, Time.deltaTime * rotSpeed.value);
                if (Vector3.Dot(this.agent.forward, lastDir) >= 0.98f)
                {
                    StopMovement();
                }
            }
            else
            {
                EndAction();
            }

        }


    }

    bool UpdateMovementRoad(MapCell cell)
    {
        if (cell == null) return false;

        MapCell nextCell = cell.GetNextCell();
        if (nextCell == null) return false;

        if (nextCell.CellType == E_CellType.TargetCell)
        {
            if (needAdjustLastDir.value == true)
            {
                lastDir = (nextCell.GetRoadPosition() - cell.GetRoadPosition()).normalized;
                adjustTargetDir = true;
                moving = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {

            curMoveStartCell = cell;
            curMoveEndCell = nextCell;

            startPosition = curMoveStartCell.GetRoadPosition();
            endPosition = curMoveEndCell.value.GetRoadPosition();

            curTargetDir = endPosition - startPosition;
            float roadLength = curTargetDir.magnitude;
            moveTime = (float)curTargetDir.magnitude / moveSpeed.value;
            //Debug.Log("road length = " + roadLength + " , moveTime = " + moveTime);

            curTargetDir = curTargetDir.normalized;

            curMoveTime = 0;

            if (moveTime == 0)
            {
                //Debug.LogError("update road");

                Debug.LogError("update road , cellid_1 = " + cell.cellID + " , cellid_2 = " + nextCell.cellID);
                return false;
            }

            return true;
        }
    }

    public void StopMovement()
    {
        if (curMovementState.value == false) return;
        curMovementState = false;

        EndAction(true);
    }

}

[Name("Rotate To Unit")]
[Category("DemoTest/Normal")]
public class RotToUnit : ActionTask
{

    public BBParameter<Unit> ownerUnit;
    public BBParameter<Unit> targetUnit;

    public BBParameter<float> rotSpeed;

    bool rotStart = false;
    Vector3 targetDir;
    Vector3 startDir;
    float lerpValue = 0;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if(ownerUnit.value==null || targetUnit.value == null)
        {
            //Debug.LogError("[RotToUnit]rotate failed, unit is null");
            EndAction(false);
            return;
        }

        targetDir = (targetUnit.value.unitTrans.position - ownerUnit.value.unitTrans.position).normalized;
        if (Vector3.Dot(ownerUnit.value.unitTrans.forward, targetDir) >= 0.95f)
        {
            EndAction(true);
        }
        else
        {
            startDir = ownerUnit.value.unitTrans.forward;
            rotStart = true;
            lerpValue = 0;
        }
    }

    protected override void OnUpdate()
    {

        if (rotStart)
        {
            if (lerpValue>=1)
            {
                EndAction(true);
            }

            lerpValue += Time.deltaTime * rotSpeed.value;

            ownerUnit.value.unitTrans.forward = Vector3.Lerp(startDir, targetDir, lerpValue);
        }

    }

}

[Name("Rotate to Target")]
[Category("DemoTest/Unit")]
public class Rotate2Target : ActionTask
{

    public BBParameter<Unit> ownerUnit;
    public BBParameter<Vector3> targetDir;
    public BBParameter<float> rotSpeed;

    Vector3 startDir;
    Vector3 dir;
    float lerpValue;
    bool startRot = false;
    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (ownerUnit.value == null)
        {
            Debug.LogError("[Rotate2Target]rotate operation failed , unit reference is null");
            EndAction(false);
            return;
        }

        startDir = ownerUnit.value.unitTrans.forward;
        dir = targetDir.value;
        dir.y = startDir.y;

        lerpValue = 0;
        startRot = true;
    }

    protected override void OnUpdate()
    {
        if (startRot)
        {
            lerpValue += rotSpeed.value * Time.deltaTime;
            ownerUnit.value.unitTrans.forward = Vector3.Lerp(startDir, dir, lerpValue);
            if (lerpValue >= 1)
            {
                EndAction(true);
            }
        }
    }

}

#endregion Movement


#region Battle(Action)


[Name("Find Nearest Unit")]
[Category("DemoTest/Battle")]
public class SearchUnit : ActionTask
{
    public BBParameter<Unit> ownerUnit;
    public BBParameter<float> searchRadius;
    public BBParameter<bool> useSearchCenter;
    public BBParameter<Vector3> searchCenter;
    public BBParameter<List<E_UnitType>> searchTypeList;

    public BBParameter<Unit> targetUnit;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (searchTypeList.value == null || searchTypeList.value.Count==0)
        {
            Debug.Assert(ownerUnit.value == null, "[SearchEnemy]searchTypeList error , plz check, unitId = " + ownerUnit.value.unitData.unitID);
            EndAction(false);
            return;
        }

        if (ownerUnit.value == null)
        {
            Debug.LogError("[SearchEnemy]search enemy failed, unit is null");
            EndAction(false);
            return;
        }

        //检索
        Unit u = null;
        if (useSearchCenter.value)
        {
            u = BattleTools.SearchNearestUnit(searchTypeList.value, searchRadius.value, ownerUnit.value, searchCenter.value, true, false);
        }
        else
        {
            u = BattleTools.SearchNearestUnit(searchTypeList.value, searchRadius.value, ownerUnit.value, Vector3.zero, false, false);
        }

        if (u == null)
        {
            EndAction(false);
            return;
        }


        targetUnit.value = u;

        EndAction(true);
    }
}

[Name("Find Crystal")]
[Category("DemoTest/Battle")]
public class SearchCrystal : ActionTask
{
    public BBParameter<Unit> ownerUnit;
    public BBParameter<float> searchRadius;

    public BBParameter<Vector3> searchCenter;
    public BBParameter<bool> useSearchCenter;
    public BBParameter<bool> searchOppsiteGroup;
    public BBParameter<E_Group> searchGroup;

    //return value
    public BBParameter<SceneCrystal> targetCrystal;
    public BBParameter<GameObject> targetGO;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (ownerUnit.value == null)
        {
            Debug.LogError("[SearchCrystal]search failed , unit refernece is null");
            EndAction(false);
            return;
        }

        E_Group group = searchOppsiteGroup.value ? GameDefine.GetOppsiteGroup(ownerUnit.value.unitData.unitGroup) : searchGroup.value;

        SceneCrystal crystal = BattleTools.SearchNearestCrystal(ownerUnit.value, searchRadius.value, group, searchCenter.value, useSearchCenter.value);
        if (crystal == null)
        {
            EndAction(false);
            return;
        }

        targetCrystal.value = crystal;
        targetGO.value = targetCrystal.value.gameObject;

        EndAction(true);

    }

}

public class UnitNormalAttack : ActionTask
{
    public BBParameter<Unit> ownerUnit;
    public BBParameter<Unit> targetUnit;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if(ownerUnit.value==null || targetUnit.value == null)
        {
            EndAction(false);
            return;
        }

        BattleTools.Attack(ownerUnit.value, targetUnit.value);
        EndAction(true);
    }

}

[Name("Get Unit Attack Point")]
[Category("DemoTest/Battle")]
public class GetUnitAttackPoint : ActionTask
{
    public BBParameter<Unit> targetUnit;

    public BBParameter<Vector3> attackPoint;

    protected override void OnExecute()
    {
        if (targetUnit.value == null)
        {
            Debug.LogError("[GetUnitAttackPoint]unit is null, get attack point failed");
            EndAction(false);
            return;
        }

        attackPoint.value = targetUnit.value.unitTrans.position + (new Vector3(0, 0.5f, 0));

        EndAction(true);
    }

}

[Name("Get Attack GameObject")]
[Category("DemoTest/Normal")]
public class GetAttackGO : ActionTask
{
    public BBParameter<object> obj;
    public BBParameter<GameObject> targetGO;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info +"("+obj+")"+ "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (obj.value == null)
        {
            Debug.LogError("[GetAttackGO]Get attack go failed, obj reference is null");
            EndAction(false);
            return;
        }

        if(obj.value is Unit)
        {
            Unit unit = obj.value as Unit;
            targetGO = unit.unitGo;
        }else if(obj.value is SceneCrystal)
        {
            SceneCrystal crystal = obj.value as SceneCrystal;
            targetGO.value = crystal.gameObject;
        }
        else
        {
            EndAction(false);
            return;
        }

        EndAction(true);

    }
}

[Category("DemoTest/Normal")]
public class GetUnitDir : ActionTask
{
    public BBParameter<Unit> ownerUnit;
    public BBParameter<Vector3> unitDir;

    public BBParameter<bool> needIgnoreY;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "(" + ownerUnit + ")" + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (ownerUnit.value == null)
        {
            Debug.LogError("[GetUintDir]unit reference is null");
            EndAction(false);
        }

        Vector3 dir= ownerUnit.value.unitTrans.forward;
        if (needIgnoreY.value)
        {
            dir.y = 0;
        }

        unitDir.value = dir;

        EndAction(true);
    }
}

#endregion Battle(Action)

#region Battle(Condition)


[Name("Check Unit State")]
[Category("DemoTest/Unit")]
public class CheckUnitState : ConditionTask
{
    public BBParameter<Unit> targetUnit;
    public BBParameter<bool> state;

    protected override string info => "<color=#dddd00><b>" + targetUnit + " == " + state + "</b></color>";
    protected override bool OnCheck()
    {
        if (targetUnit.value == null) return false;

        return targetUnit.value.unitActiveState;
    }

}


[Name("Check Unit HP")]
[Category("DemoTest/Unit")]
public class CheckUnitHp : ConditionTask
{
    public BBParameter<Unit> targetUnit;
    public BBParameter<CompareMethod> compareType;
    public BBParameter<int> compareValue;

    protected override string info => "<color=#dd0000><b>if( " + targetUnit + "HP" + OperationTools.GetCompareString(compareType.value) + compareValue + " )</b></color>";

    protected override bool OnCheck()
    {
        if (targetUnit.value == null) return false;

        return OperationTools.Compare(targetUnit.value.unitData.curHp, compareValue.value, compareType.value);

        //return targetUnit.value.unitData.curHp > compareValue.value;
    }
}


[Name("Check Attack Distance")]
[Category("DemoTest/Unit")]
public class CheckAttackDist : ConditionTask
{
    public BBParameter<Unit> ownerUnit;
    public BBParameter<Unit> targetUnit;
    public BBParameter<float> attackRadius;

    protected override bool OnCheck()
    {
        if(ownerUnit.value==null || targetUnit.value == null)
        {
            return false;
        }

        float dist1 = (ownerUnit.value.unitTrans.position - targetUnit.value.unitTrans.position).magnitude;
        //Debug.LogWarning("dist = " + dist1 + " , attackRadius = " + attackRadius.value);

        return (ownerUnit.value.unitTrans.position - targetUnit.value.unitTrans.position).magnitude <= attackRadius.value;
    }
}

[Name("Check Attack Distance(GameObject")]
[Category("DemoTest/Unit")]
public class CheckAttackDist_GO : ConditionTask
{
    public BBParameter<Unit> ownerUnit;
    public BBParameter<GameObject> targetGO;
    public BBParameter<float> attackRadius;

    protected override string info
    {
        get { return "<color=#dddd00>" + base.info + "</color>"; }
    }

    protected override bool OnCheck()
    {
        if (ownerUnit.value == null || targetGO.value==null)
        {
            return false;
        }

        float dist = (ownerUnit.value.unitTrans.position - targetGO.value.transform.position).magnitude;

        return dist <= attackRadius.value;
    }
}

[Name("Check Crystal HP")]
[Category("DemoTest/Unit")]
public class CheckCrystalHP : ConditionTask
{
    public BBParameter<SceneCrystal> crystal;
    public BBParameter<CompareMethod> compareType;
    public BBParameter<int> compareValue;

    protected override string info => "<color=#dd0000><b>if( " + crystal + ".HP " + OperationTools.GetCompareString(compareType.value) + compareValue + " )</b></color>";

    protected override bool OnCheck()
    {
        if (crystal.value == null)
        {
            return false;
        }

        return OperationTools.Compare(crystal.value.CrystalHp, compareValue.value, compareType.value);

        //return crystal.value.CrystalHp > compareValue.value;
    }
}


#endregion Battle(Condition)

#region Other


[Name("Set Unit Value")]
[Category("DemoTest/Normal")]
public class SetUnitValue : ActionTask
{

    public BBParameter<Unit> targetUnit;
    public BBParameter<Unit> Value;
    protected override string info => "<color=#0000dd><b>" + targetUnit + " = " + Value + "</b></color>";

    protected override void OnExecute()
    {
        targetUnit.value = Value.value;
        EndAction(true);
    }

}


[Name("Check Unit Value")]
[Category("DemoTest/Unit")]
public class CheckUnit : ConditionTask
{
    public BBParameter<Unit> targetUnit;
    public BBParameter<Unit> Value;

    protected override string info => "<color=#dddd00><b>" + targetUnit + " == " + Value + "</b></color>";

    protected override bool OnCheck()
    {
        return targetUnit.value == Value.value;
    }
}


public class ReturnValue : ActionTask
{
    public BBParameter<bool> value;

    protected override string info => "$<color=#11ff33><b>[Return " + value+"]</b></color>";

    protected override void OnExecute()
    {
        EndAction(value.value);
    }
}

#endregion Other


#region Math

public class Math_Vector3_Dot : ConditionTask
{
    public BBParameter<Vector3> value_1;
    public BBParameter<Vector3> value_2;
    public BBParameter<CompareMethod> compareType;
    public BBParameter<float> compareValue;

    protected override string info => "<color=#dddd00><b>Vector3.Dot(" + value_1 + ", " + value_2 + ")" + OperationTools.GetCompareString(compareType.value) + compareValue + "</b></color>";

    protected override bool OnCheck()
    {
        float dotValue = Vector3.Dot(value_1.value, value_2.value);
        return OperationTools.Compare(dotValue, compareValue.value, compareType.value, 0.001f);

    }

}


#endregion Math