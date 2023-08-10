using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    #region Attack_Param

    static int default_longAttackID = 100;
    static public int longAttackID;

    Dictionary<int, LongDistAttack> longDistAttackDic = new Dictionary<int, LongDistAttack>();

    #endregion Atatck_Param

    #region 生命周期

    public void DoInit()
    {
        longAttackID = default_longAttackID;

    }

    public void DoUpdate(float deltatime)
    {

        if (longDistAttackDic != null)
        {
            foreach (var temp in longDistAttackDic)
            {
                if (temp.Value == null) continue;
                temp.Value.DoUpdate(deltatime);
            }
        }
    }

    public void DoFixedUpdate(float fixedUpdateTime)
    {

        if (longDistAttackDic != null)
        {
            foreach (var temp in longDistAttackDic)
            {
                if (temp.Value == null) continue;
                temp.Value.DoFixedUpdate(fixedUpdateTime);
            }
        }
    }

    public void DoLateUpdate(float deltatime)
    {

        if (longDistAttackDic != null)
        {
            foreach (var temp in longDistAttackDic)
            {
                if (temp.Value == null) continue;
                temp.Value.DoLateUpdate(deltatime);
            }
        }
    }

    #endregion 生命周期

    #region Attack

    public LongDistAttack CreateLongDistAttack(int assetPrefabsID, E_LongDistAtkType type,Vector3 firePoint, Unit attacker,Vector3 dir)
    {

        GameObject atkGO = AssetManager.Instance.Create(assetPrefabsID);

        if (atkGO == null)
        {
            Debug.LogError("[BattleManager]create attack failed, attack type = " + type + " , assetPrefabsID = " + assetPrefabsID);
            return null;
        }

        LongDistAttack atk = atkGO.GetComponent<LongDistAttack>();
        if (atk == null)
        {
            Debug.LogError("[BattleManger]Get LongDistAttack component faield , assetID = " + assetPrefabsID);

            AssetManager.Instance.Destory(atkGO);
            return null;
        }

        switch (type) {
            case E_LongDistAtkType.Dragon_FireBall:

                break;
            default:
                break;
        }

        atk.DoInit(longAttackID++, type, attacker.unitGo, dir);
        atk.SetPosition(firePoint);

        RegisterLongDistAttack(atk);

        return atk;
    }

    public LongDistAttack CreateLongDistAttack(int assetPrefabsID, E_LongDistAtkType type, GameObject attacker,Vector3 dir)
    {

        return null;
    }

    public void RegisterLongDistAttack(LongDistAttack attack)
    {
        if (attack == null) return;

        if (longDistAttackDic == null) longDistAttackDic = new Dictionary<int, LongDistAttack>();
        if (longDistAttackDic.ContainsKey(attack.attackID))
        {
            Debug.LogError("[BattleManager]Register longDistAttack failed");
            return;
        }

        longDistAttackDic.Add(attack.attackID, attack);
    }

    public void UnregisterLongDistAttack(LongDistAttack attack)
    {
        if (attack == null) return;

        UnregisterLongDistAttack(attack.attackID);
    }

    public void UnregisterLongDistAttack(int id)
    {
        if (longDistAttackDic == null) return;

        LongDistAttack attack = null;
        if (longDistAttackDic.TryGetValue(id,out attack) == false)
        {
            return;
        }
        longDistAttackDic.Remove(attack.attackID);

        attack.ActiveAttack(false);

        AssetManager.Instance.Destory(attack.gameObject);
    }

    #endregion Attack

    #region effect




    #endregion effect

}
