using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class FlockingTest_Unit : MonoBehaviour
{
    [SerializeField]bool isInit = false;//init sign

    FlockingTest_Group parentGroup;

    #region component

    private Transform _transform;
    public Transform unitTrans
    {
        get
        {
            if (_transform == null) _transform = unitGO.transform;
            return _transform;
        }
    }

    private GameObject _gameObject;
    public GameObject unitGO
    {
        get
        {
            if (_gameObject == null) _gameObject = this.gameObject;
            return _gameObject; 
        }
    }

    public Vector3 pos {
        get
        {
            return unitTrans.position;
        }
    }

    public Vector3 forward
    {
        get
        {
            return unitTrans.forward;
        }
    }


    #endregion component

    #region flocking_param
    public bool isUseWideView;//视野选择
    [SerializeField]float unitRadius;//单元半径
    [SerializeField] float unitMoveSpeed;
    public float radius { get { return unitRadius; } }
    [SerializeField] float separationFactor;//分离因子
    [SerializeField] float separationForce;
    [SerializeField] float cohesionFactor;//聚合因子
    [SerializeField] float cohesionForce;
    [SerializeField] float alignmentFactor;//对齐因子

    //视角
    public float wideAngle;
    public float narrowAngle;
    [SerializeField] float wideAngle_cos;
    [SerializeField] float narrowAngle_cos;

    public float wideView_radiusFactor;
    public float narrowView_radiusFactor;

    [Header("test")]
    [SerializeField]Vector3 unitVelocity = Vector3.zero;
    public Vector3 velocity { get { return unitVelocity; } }

    [SerializeField]Vector3 force = Vector3.zero;

    //计算参数
    Vector3 position2OtherUnit = Vector3.zero;
    float dist2OtherUnit = 0;
    float sqrDist2OtherUnit = 0;

    Vector3 groupCenterPoint = Vector3.zero;
    Vector3 groupVelocity = Vector3.zero;
    Vector3 sum_unitVelocity = Vector3.zero;
    Vector3 sum_unitPosition = Vector3.zero;
    int neighboorCount = 0;
    float neighboorCount_I=0;
    #endregion flocking_param


    #region lifeCycle

    public void DoInit(FlockingTest_Group group)
    {
        parentGroup = group;

        wideAngle_cos = Mathf.Cos(wideAngle*Mathf.Deg2Rad);
        narrowAngle_cos = Mathf.Cos(narrowAngle*Mathf.Deg2Rad);

        transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-2, 2), 0, UnityEngine.Random.Range(-2, 2));

        isInit = true;
    }

    public void DoUpdate(float deltaTime)//@
    {
        FlockingCalc();
        MoveUnit(deltaTime);
    }

    public void DoFixedUpdate(float fixedUpdateTime)
    {
        //FlockingCalc();
    }


    #endregion lifeCycle


    void LimitSpeed()
    {
        if (isInit == false || parentGroup == null)
            return;
        //if (unitVelocity.magnitude > parentGroup.unitSpeed)
        //{
        //    unitVelocity = velocity.normalized * parentGroup.unitSpeed;
        //}
        //unitVelocity += new Vector3(UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f), UnityEngine.Random.Range(-0.05f, 0.05f));//@
        unitVelocity.x += UnityEngine.Random.Range(-0.05f, 0.05f);
        unitVelocity.y += UnityEngine.Random.Range(-0.05f, 0.05f);
        unitVelocity.z += UnityEngine.Random.Range(-0.05f, 0.05f); 
        unitVelocity = velocity.normalized * parentGroup.unitSpeed;
    }

    #region 外部方法

    public void FlockingCalc()
    {
        if (isInit == false || parentGroup == null) return;

        List<FlockingTest_Unit>.Enumerator enumerator = parentGroup.GetUnitListEnumerator();

        bool isInView = false;
        float viewCosValue = (isUseWideView) ? wideAngle_cos : narrowAngle_cos;
        float viewRadius = (isUseWideView) ? wideView_radiusFactor : narrowView_radiusFactor;
        float sqrViewRadius = viewRadius * viewRadius;

        float dotValue = 0;

        //初始化数值
        neighboorCount = 0;
        force = Vector3.zero;
        sum_unitPosition = Vector3.zero;
        sum_unitVelocity = Vector3.zero;
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            if (current == null || current == this) continue;


            //计算单位之间的距离
            position2OtherUnit = current.unitTrans.position - transform.position;
            //dist2OtherUnit = position2OtherUnit.magnitude;//@
            sqrDist2OtherUnit = position2OtherUnit.sqrMagnitude;

            //是否在视角内
            dotValue = Vector3.Dot(transform.forward, position2OtherUnit.normalized);

            isInView = dotValue >= viewCosValue;
            //isInView = true;
            if (isInView && sqrDist2OtherUnit <= sqrViewRadius)//@
            {
                if (sqrDist2OtherUnit == 0)
                {
                    sqrDist2OtherUnit = 0.001f;
                }
                //分离
                force += separationForce * (Mathf.Clamp01(((sqrViewRadius - sqrDist2OtherUnit) * separationFactor)) / sqrDist2OtherUnit) * (position2OtherUnit.normalized * -1);

                neighboorCount++;
                sum_unitPosition += current.pos;
                sum_unitVelocity += current.velocity;

            }
            
        }
        if (neighboorCount != 0)
        {

            neighboorCount_I = 1.0f / neighboorCount;

            groupCenterPoint = sum_unitPosition*neighboorCount_I;//
            groupVelocity = sum_unitVelocity * neighboorCount_I;

            //聚合
            //固定群体
            force += (parentGroup.center - transform.position).normalized * cohesionForce;
            //分离群体
            //force += (groupCenterPoint - transform.position).normalized * cohesionForce;
        }
        else
        {
            force = Vector3.zero;
        }

    }


    public void MoveUnit(float deltaTime)
    {
        unitVelocity += force * unitMoveSpeed ;
        LimitSpeed();//限制速度

        transform.position = transform.position + unitVelocity * deltaTime ;
        if (groupVelocity.sqrMagnitude != 0)
        {
            transform.forward = Vector3.Lerp(unitVelocity,groupVelocity, parentGroup.alignmentFactor);//方向插值
        }
        else
        {
            transform.forward = unitVelocity;
        }

    }

    #endregion 外部方法

}