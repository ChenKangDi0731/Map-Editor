using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


#region movement


public class DragonRotToUnit : ActionTask
{

    public BBParameter<Unit> ownerUnit;
    public BBParameter<Unit> targetUnit;
    public BBParameter<float> angleRange;

    //return value
    public BBParameter<Vector3> targetDir;
    public BBParameter<bool> needChangeDir;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (ownerUnit.value == null || targetUnit.value == null)
        {
            needChangeDir.value = false;
            EndAction(false);
            return;
        }

        SpineRotControl spineControl = ownerUnit.value.config.spineControl;
        if (spineControl == null)
        {
            Debug.LogError("[RotToUnit]owner spineControl component is null,rotate failed, unitId = " + ownerUnit.value.unitData.unitID);
            needChangeDir.value = false;
            EndAction(false);
            return;
        }

        Vector3 dir2Target = (targetUnit.value.unitTrans.position - ownerUnit.value.unitTrans.position).normalized;

        //TODO

        EndAction(true);
    }

}

#endregion movement

#region attack

/// <summary>
/// Fire Ball Attack
/// </summary>
[Name("Dragon Fire Ball Attack(Position)")]
[Category("DemoTest/Unit/RedDragon")]
public class DragonFireBallAttack_Pos : ActionTask
{
    public BBParameter<Unit> ownerUnit;
    public BBParameter<Vector3> attackPoint;

    public BBParameter<int> fireBallPrefabsId;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if(ownerUnit.value==null || attackPoint.value == null)
        {
            Debug.LogError("[DragonFireBallAttack]Dragon attack failed");
            EndAction(false);
            return;
        }

        UnitConfig config = ownerUnit.value.config;
        if(config is Config_RedDragon)
        {
            Config_RedDragon config2 = config as Config_RedDragon;
            if (config2 == null)
            {
                Debug.LogError("[DragonFireBallAttack] unit config type errro");
                EndAction(false);
                return;
            }

            Vector3 attackDir;

            if (config2.firePoint == null)
            {
                Debug.LogError("[DragonFireBallAttack] fire point reference is null,attack failed");
                attackDir = (attackPoint.value - ownerUnit.value.unitTrans.position).normalized;
            }
            else
            {
                attackDir = (attackPoint.value - config2.firePoint.transform.position).normalized;
            }

            LongDistAttack fireBall = BattleManager.Instance.CreateLongDistAttack(fireBallPrefabsId.value, E_LongDistAtkType.Dragon_FireBall,config2.firePoint.transform.position, ownerUnit.value, attackDir);
            if (fireBall == null)
            {
                Debug.LogError("[DragonFireBallAttack]Create LDA failed");
                EndAction(false);
                return;
            }

            EndAction(true);
            return;
        }

        EndAction(false);
    }
}

/// <summary>
/// Fire Ball Attack
/// </summary>
[Name("Dragon Fire Ball Attack(Vector3)")]
[Category("DemoTest/Unit/RedDragon")]
public class DragonFireBallAttack_GO : ActionTask
{
    public BBParameter<Unit> ownerUnit;
    public BBParameter<GameObject> attackPoint;

    public BBParameter<int> fireBallPrefabsId;

    protected override string info
    {
        get { return "<color=#9932cc><b>" + base.info + "</b></color>"; }
    }

    protected override void OnExecute()
    {
        if (ownerUnit.value == null || attackPoint.value == null)
        {
            Debug.LogError("[DragonFireBallAttack]Dragon attack failed");
            EndAction(false);
            return;
        }

        UnitConfig config = ownerUnit.value.config;
        if (config is Config_RedDragon)
        {
            Config_RedDragon config2 = config as Config_RedDragon;
            if (config2 == null)
            {
                Debug.LogError("[DragonFireBallAttack] unit config type errro");
                EndAction(false);
                return;
            }

            Vector3 attackDir;

            if (config2.firePoint == null)
            {
                Debug.LogError("[DragonFireBallAttack] fire point reference is null,attack failed");
                attackDir = (attackPoint.value.transform.position - ownerUnit.value.unitTrans.position).normalized;
            }
            else
            {
                attackDir = (attackPoint.value.transform.position - config2.firePoint.transform.position).normalized;
            }

            LongDistAttack fireBall = BattleManager.Instance.CreateLongDistAttack(fireBallPrefabsId.value, E_LongDistAtkType.Dragon_FireBall, ownerUnit.value.unitGo, attackDir);
            if (fireBall == null)
            {
                Debug.LogError("[DragonFireBallAttack]Create LDA failed");
                EndAction(false);
                return;
            }

            EndAction(true);
            return;
        }

        EndAction(false);
    }
}

#endregion attack
