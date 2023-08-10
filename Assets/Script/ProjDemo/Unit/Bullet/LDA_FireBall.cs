using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LDA_FireBall : LongDistAttack
{
    [Header("FireBall_Param")]
    [SerializeField] float explodeRadius;
    [SerializeField] GameObject hitEffectGO;

    #region 生命周期

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        /*
        if (hitEffectGO != null)
        {
            hitEffectGO.MapAllComponent<ParticleSystem>(ps=>{
                ps.Stop();
            });
        }
        */

        base.OnDisable();
        
    }

    #endregion 生命周期


    protected override bool HitDetect(Collider c)
    {

        if (c == null) return false;

        Debug.LogWarning("1 Hit ,id = " + attackID + " , hit obj name = " + c.gameObject.name);

        //explode

        /*
        if (hitEffectGO != null)
        {
            //GameObject effect=AssetManager.Instance.Create()

            hitEffectGO.MapAllComponent<ParticleSystem>(ps =>
            {
                ps.Stop();
                ps.Play();
            });
        }
        */

        if (hitEffectPoint != null)
        {
            EffectControl effect = EffectManager.Instance.CreateEffect(E_EffectType.ParticleSystem, hitEffectGO, hitEffectPoint.transform, true);
            if (effect != null)
            {
                effect.transform.position = hitEffectPoint.transform.position;
                effect.transform.rotation = hitEffectPoint.transform.rotation;

            }
        }

        //hit detect
        Collider[] hitColliderArray = Physics.OverlapSphere(this.transform.position, explodeRadius, attackMask);
        if (hitColliderArray != null && hitColliderArray.Length != 0)
        {
            for(int index = 0; index < hitColliderArray.Length; index++)
            {
                Collider cur = hitColliderArray[index];
                if (cur == null) continue;

                Hit(cur);
            }
        }


        return true;
    }

    protected override void Hit(Collider c)
    {
        if (c == null) return;

        int goLayer = 1 << c.gameObject.layer;

        int layerDetect = goLayer & attackMask;

        if (layerDetect == 0)
        {
            return;
        }

        //raycast(检测是否直接伤害(只能用于go中心在碰撞体内的情况
        bool directlyHit = false;
        RaycastHit hitInfo;
        Vector3 dir = c.transform.position - this.transform.position;
        if(Physics.Raycast(this.transform.position,dir, out hitInfo, explodeRadius))
        {
            if (hitInfo.collider == c)
            {
                directlyHit = true;
            }
            else
            {
                directlyHit = false;
            }
        }


        if (directlyHit)
        {

        }
        else
        {

        }

    }

}
