using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoTest_1_Unit : MonoBehaviour
{
    Unit unit;
    public bool curMovementState = false;

    public MapCell startCell;

    [Header("Movement param")]
    public float moveSpeed;
    public float rotSpeed;

    public MapCell curMoveStartCell;
    public MapCell curMoveEndCell;
    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 curTargetDir;
    public float moveTime;
    public float curMoveTime = 0;
    #region 生命周期

    private void Update()
    {
        if (curMovementState)
        {
            curMoveTime += Time.deltaTime;
            curMoveTime = Mathf.Clamp(curMoveTime, 0, moveTime);
            float lerpValue = curMoveTime / moveTime;
            this.transform.position = Vector3.Lerp(startPosition, endPosition, lerpValue);
            this.transform.forward = Vector3.Lerp(this.transform.forward, curTargetDir, Time.deltaTime * rotSpeed);

            if (curMoveTime >= moveTime)
            {
                if (UpdateMovementRoad(curMoveEndCell) == false)
                {
                    StopMovement();
                }
            }
        }
    }

    #endregion 生命周期

    bool UpdateMovementRoad(MapCell cell)
    {
        if (cell == null) return false;

        MapCell nextCell = cell.GetNextCell();
        if (nextCell == null) return false;

        if (nextCell.CellType == E_CellType.TargetCell)
        {
            this.transform.forward = (nextCell.GetRoadPosition() - cell.GetRoadPosition()).normalized;
            return false;
        }
        else
        {

            curMoveStartCell = cell;
            curMoveEndCell = nextCell;

            startPosition = curMoveStartCell.GetRoadPosition();
            endPosition = curMoveEndCell.GetRoadPosition();

            curTargetDir = endPosition - startPosition;
            float roadLength = curTargetDir.magnitude;
            moveTime = (float)curTargetDir.magnitude / moveSpeed;
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

    #region 外部方法

    public void DoInit(Unit u)
    {
        if (u == null)
        {
            Debug.LogError("[DemoTest_1_Unit]unit is null, init failed");
            return;
        }

        unit = u;
        StopMovement();
    }

    public void StartMovement()
    {
        if (curMovementState) return;
        curMovementState = true;

        curMoveStartCell = startCell;

        if (UpdateMovementRoad(curMoveStartCell)==false)
        {
            Debug.LogError("[DemoTest_1_Unit]Move End");
            StopMovement();
        }

        this.transform.forward = curTargetDir;

        if (unit != null)
        {
            unit.unitAnim.animator.SetTrigger("Run");
        }
    }

    public void StopMovement()
    {
        if (curMovementState == false) return;
        curMovementState = false;

        if (unit != null)
        {
            //unit.unitAnim.animator.SetTrigger("Run");
        }
    }

    public void SetStartPoint(MapCell cell)
    {
        if (cell == false)
        {
            Debug.LogError("[DemoTest_1_Unit]start point is null");
            return;
        }

        startCell = cell;

        this.transform.position = cell.GetRoadPosition();
    }

    #endregion 外部方法
}
