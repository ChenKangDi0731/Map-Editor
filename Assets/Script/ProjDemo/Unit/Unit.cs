using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{

    #region component 

    private GameObject _gameObject;
    public GameObject unitGo { get { return _gameObject; } }

    private Transform _transform;
    public Transform unitTrans
    {
        get
        {
            if (_transform == null && unitGo != null)
            {
                _transform = unitGo.transform;
            }
            return _transform;
        }
    }

    #endregion component

    #region script_component

    public Unit_Data unitData;//单元数据类
    public Unit_Anim unitAnim;
    public Unit_AI unitAI;

    private UnitConfig _unitConfig;
    public UnitConfig config { get { return _unitConfig; } }

    #endregion script_component

    #region state_param

    private bool _unitActiveState;//用于判断unit激活状态（是否死亡等）
    public bool unitActiveState { get { return _unitActiveState; } }

    #endregion state_param

    public virtual void DoInit(int unitID,string unitName)
    {

        if (unitData == null) unitData = new Unit_Data();
        unitData.DoInit(this,unitID,unitName);

        if (unitAnim == null) unitAnim = new Unit_Anim();
        unitAnim.DoInit(this);

        if (unitAI == null) unitAI = new Unit_AI();
        unitAI.DoInit(this);

        _unitActiveState = true;
    }

    #region 生命周期

    public virtual void DoUpdate(float deltaTime)
    {

    }

    public virtual void DoFixedUpdate(float fixedUpdateTime)
    {

    }

    public virtual void LateUpdate(float deltaTime)
    {

    }

    #endregion 生命周期

    #region 外部方法

    public virtual void SetGroup(E_Group group)
    {
        if (unitData != null)
        {
            unitData.unitGroup = group;
        }
    }

    public virtual void BindGameObject(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("[Unit]gameobject is null, Bind gameobject failed,unitId = " + unitData.unitID);
            return;
        }

        _gameObject = go;
        _gameObject.name = unitData.unitName + "_" + unitData.unitID.ToString();

        UnitConfig c = _gameObject.GetComponent<UnitConfig>();
        if (c != null)
        {
            _unitConfig = c;
            _unitConfig.DoInit(this);
            //设定动画状态机
            if (unitAnim != null && _unitConfig.animator!=null)
            {
                unitAnim.SetAnimator(_unitConfig.animator);
            }
        }

        //在初始化AI前先初始化unit数据
        if (unitData != null)
        {
            unitData.InitData();
        }

        //初始化AI
        if (unitAI != null)
        {
            unitAI.SetAI();
        }
    }

    public virtual void UnbindGameObject()
    {
        _gameObject = null;
        _transform = null;

        if (unitData != null)
            unitData.SetInactive();
        if (unitAnim != null)
            unitAnim.SetInactive();
        if (unitAI != null)
            unitAI.SetInactive();
        if (config != null)
            config.ClearData();

        _unitConfig = null;
    }

    /// <summary>
    /// Unit激活状态设置
    /// </summary>
    public void SetUnitActiveState(bool state)
    {
        _unitActiveState = state;
    }


    #endregion 外部方法

    #region 其他方法



    #endregion 其他方法
}
