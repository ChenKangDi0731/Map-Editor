using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Anim
{

    private Unit _unit;
    public Unit unit
    {
        get { return _unit; }
        private set { _unit = value; }
    }

    public Animator animator;

    public virtual void DoInit(Unit u)
    {
        if (u == null)
        {
            Debug.LogError("[Unit_Anim]Init failed, unit is null");
            return;
        }

    }

    public void SetAnimator(Animator a)
    {
        if (a == null)
        {
            Debug.LogError("[Unit_Anim]Set animator failed,unitId = " + _unit.unitData.unitID);
        }
        animator = a;
    }

    public virtual void SetInactive()
    {
        animator = null;
    }
}
