using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;

public class Unit_AI
{

    private Unit _unit;
    public Unit unit
    {
        get { return _unit; }
        private set { _unit = value; }
    }

    BehaviourTreeOwner btOwner;
    public BehaviourTreeOwner BtOwner
    {
        get { return btOwner; }
    }
    public void DoInit(Unit u)
    {
        if (u == null)
        {
            Debug.LogError("[Unit_AI]Unit is null,Init failed");
            return;
        }

        _unit = u;
    }

    public void SetAI(BehaviourTreeOwner bt)
    {
        if (bt == null)
        {
            Debug.LogError("[Unit_AI]Set behaviourTreeOwner failed, unitId = " + unit.unitData.unitID);
            return;
        }

        EnableAI(true);

    }

    public void SetAI()
    {
        if(unit==null || unit.unitGo == null)
        {
            Debug.LogError("[Unit_AI]Set behaviourTreeOwner failed");
            return;
        }

        btOwner = unit.unitGo.GetComponent<BehaviourTreeOwner>();


        EnableAI(false);
    }

    public void EnableAI(bool enable)
    {
        if (btOwner == null) return;

        btOwner.enabled = enable;
    }

    public virtual void SetInactive()
    {
        EnableAI(false);
        if (btOwner != null)
        {
            IBlackboard bb = btOwner.blackboard;
            if (bb != null)
            {
                Variable<SpawnPointInfo> spawnPoint = bb.GetVariable<SpawnPointInfo>(GameDefine.spawnPointStr);
                if(spawnPoint!=null && spawnPoint.value != null)
                {
                    SpawnManager.Instance.UpdateDataUnitCount(spawnPoint.value, true);
                }
            }
        }
        btOwner = null;
    }

}
