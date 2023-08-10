using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongDistAttack : MonoBehaviour
{
    #region component

    [SerializeField]protected Rigidbody r;
    [SerializeField]protected Collider c;

    #endregion component

    private int _attackID;
    public int attackID
    {
        get { return _attackID; }
    }
    [SerializeField] protected E_LongDistAtkType attackType;
    [SerializeField] protected LayerMask attackMask;
    //public Unit attacker;
    public GameObject attackerGO;

    [Header("位移参数")]
    [SerializeField]protected float moveSpeed;

    protected Vector3 moveDir;

    [Header("基本参数")]
    [SerializeField]protected float baseDamage;
    [SerializeField]protected float calcDamage;//【测试显示】

    [Header("其他参数")]
    [SerializeField] protected GameObject hitEffectPoint;

    bool isAttacking = false;
    bool attackEnd = false;

    public virtual void DoInit(int id, E_LongDistAtkType atkType, GameObject attacker,Vector3 dir)
    {
        _attackID = id;
        attackType = atkType;

        attackerGO = attacker;
        moveDir = dir;

        isAttacking = false;
        attackEnd = false;

        ActiveAttack(true);
    }


    protected virtual void CalcDmg(E_LongDistAtkType atkType,GameObject attacker)
    {
        switch (atkType)
        {
            case E_LongDistAtkType.Dragon_FireBall:

                break;

            default:

                break;

        }


    }


    protected virtual bool HitDetect(Collider c)
    {
        if (c == null) return false;

        Debug.LogWarning("2 Hit ,id = " + attackID + " , hit obj name = " + c.gameObject.name);
        return true;
    }

    /// <summary>
    /// 击中处理
    /// </summary>
    /// <param name="c"></param>
    protected virtual void Hit(Collider c)
    {

    }

    #region 生命周期

    private void Awake()
    {
        
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        
    }

    public virtual void DoUpdate(float deltatime)
    {
        if (isAttacking)
        {
            transform.position = transform.position + ((moveSpeed * deltatime) * moveDir);
        }
    }

    public virtual void DoFixedUpdate(float fixedUpdateTime)
    {
        if (isAttacking)
        {

        }
    }

    public virtual void DoLateUpdate(float deltatime)
    {
        if (isAttacking)
        {

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking == false)
        {
            ActiveCollider(false);
            //ActiveAttack(false);
            return;
        }

        if (HitDetect(other))
        {
            attackEnd = true;
            ActiveAttack(false);
        }
    }


    #endregion 生命周期

    #region 外部方法

    public void ActiveAttack(bool active)
    {
        if (attackEnd)
        {
            isAttacking = false;
            ActiveCollider(false);
        }

        isAttacking = active;

        ActiveCollider(active);
    }

    public void ActiveCollider(bool active)
    {
        if (c != null)
        {
            if (c.enabled != active)
            {
                c.enabled = active;
            }
        }
    }

    public virtual void SetPosition(Vector3 position)
    {
        transform.position = position;
    }


    #endregion 外部方法


    #region test_method

    [ContextMenu("Test method")]
    public void TestMehotd()
    {
        gameObject.MapAllComponent<Transform>(t => { 
        
        
        });
        //Debug.LogError("Transform count = " + tArray.Length);
    }

    #endregion test_method
}
