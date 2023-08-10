using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingTest_Group : MonoBehaviour
{

    [SerializeField]bool isInit = false;//init sign

    public List<FlockingTest_Unit> unitList = new List<FlockingTest_Unit>();

    public GameObject chasee;//TODO

    #region flocking_param
    public float groupRadius = 0;//组半径（用于动态调整单元数量的情况）
    int flockingUnitCount = 0;

    [SerializeField]Vector3 flockingCenter = Vector3.zero;
    public Vector3 center { get { return flockingCenter; } }
    [SerializeField]Vector3 flockingForward = Vector3.zero;
    public Vector3 forward { get { return flockingForward; } }

    #endregion flocking_param

    #region flockingUnit_param

    public float unitSpeed;
    [Range(0, 1)] public float alignmentFactor = 0.5f;
    #endregion flockingUnit_param

    #region test_move_param

    [Range(0,1)]
    public float lerpSpeed;

    #endregion

    [Header("test")]
    public GameObject centerObj;

    #region lifeCycle

    private void Awake()
    {
        //DebugTools.LogError("test", DebugColor.Red);
        //DoInit();
    }

    private void Update()
    {
        return;
        if (isInit == false) return;

        UpdateFlockingCenter();

        for(int index = 0; index < unitList.Count; index++)
        {
            if (unitList[index] != null)
            {
                unitList[index].DoUpdate(Time.deltaTime);
            }
        }
        if (centerObj != null)
        {
            centerObj.transform.position = center;
        }
    }

    private void FixedUpdate()
    {
        return;
        if (isInit == false) return;

        for(int index = 0; index < unitList.Count; index++)
        {
            if (unitList[index] == null) continue;

            unitList[index].DoFixedUpdate(Time.fixedDeltaTime);
        }

    }

    public void DoUpdate(float deltaTime)
    {
        if (isInit == false) return;

        //UpdateFlockingCenter();

        for (int index = 0; index < unitList.Count; index++)
        {
            if (unitList[index] != null)
            {
                unitList[index].DoUpdate(deltaTime);
            }
        }
        if (centerObj != null)
        {
            centerObj.transform.position = center;
        }
    }

    public void DoFixedUpdate(float deltaFixedTime)
    {
        if (isInit == false) return;

        for (int index = 0; index < unitList.Count; index++)
        {
            if (unitList[index] == null) continue;

            unitList[index].DoFixedUpdate(deltaFixedTime);
        }
    }

    #endregion lifeCycle

    public void DoInit()
    {
        if (unitList == null) unitList = new List<FlockingTest_Unit>();

        for(int index = 0; index < unitList.Count; index++)
        {
            if (unitList[index] == null) continue;

            unitList[index].DoInit(this);
        }

        isInit = true;
    }

    void UpdateFlockingCenter()
    {
        if (unitList == null) return;

        flockingCenter = Vector3.zero;
        flockingForward = Vector3.zero;
        flockingUnitCount = 0;
        int boidCount = 0;
        for(int index = 0; index < unitList.Count; index++)
        {
            if (unitList[index] == null) continue;

            flockingUnitCount++;

            flockingForward += unitList[index].forward;
            flockingCenter += unitList[index].pos;

            boidCount++;
        }

        if (flockingUnitCount != 0)
        {
            flockingForward /= flockingUnitCount; //@
            flockingCenter /= flockingUnitCount;
        }

        //flockingCenter += new Vector3(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)) * 10 * Time.deltaTime;
    }

    public void SetCenter(Vector3 pos)
    {
        flockingCenter = pos;
    }

    #region 外部方法

    public void UpdateCenter()
    {
        UpdateFlockingCenter();
    }

    public void MoveCenter(Vector3 deltaMove)
    {
        //Debug.LogWarning("delta move = " + deltaMove);
        flockingCenter += deltaMove;
    }

    public List<FlockingTest_Unit>.Enumerator GetUnitListEnumerator()
    {
        if (unitList == null) unitList = new List<FlockingTest_Unit>();
        return unitList.GetEnumerator();
    }

    #endregion 外部方法


}
