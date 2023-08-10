using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking_MovingTest : MonoBehaviour
{
    public FlockingTest_Group group;
    public Demo_TileManager tileManager;
    public Demo_Tile curTile = null;
    public Demo_Tile targetTile = null;

    public bool isStartMovement = false;

    #region movement_param

    [Header("移动参数")]
    public float moveSpeed;
    public float endMoveDist = 0.01f;
    float sqrEndMoveDist;

    [SerializeField]Vector3 curMoveDir = Vector3.zero;
    [SerializeField]Vector3 deltaMoveVector = Vector3.zero;
    #endregion movement_param

    private void Awake()
    {
        DoInit();
        InputManager.Instance.DoInit();

    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.A))
        {
            StartMovement();
            return;
        }

        if (isStartMovement == false) return;

        if(curTile == null || targetTile == null)
        {
            isStartMovement = false;
            return;
        }

        if (group == null)
        {
            isStartMovement = false;
            return;
        }

        //group.UpdateCenter();//更新中心点
        deltaMoveVector = curMoveDir * moveSpeed * Time.deltaTime;
        group.MoveCenter(deltaMoveVector);
        group.DoUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (isStartMovement == false) return;

        if (group == null || curTile == null || targetTile == null)
        {
            return;
        }

        if ((group.center - targetTile.tilePos).sqrMagnitude <= sqrEndMoveDist)
        {
            ChangeTargetTile();
        }
    }

    public void DoInit()
    {
        if (group != null)
        {
            group.DoInit();
        }

        if (tileManager != null)
        {
            tileManager.DoInit();
        }

        sqrEndMoveDist = endMoveDist * endMoveDist;
    }

    private void StartMovement()
    {
        if (group == null)
        {
            Debug.LogError("group is null ,move group failed");
            return;
        }

        if (tileManager == null)
        {
            Debug.LogError("tileManager is null, move group failed");
            return;
        }

        curTile = tileManager.GetStartTile();
        if (curTile == null || curTile.TryGetNextTile(out targetTile)==false)
        {
            Debug.LogError("start tile is null , move group failed");
            return;
        }

        curMoveDir = (targetTile.tilePos - curTile.tilePos).normalized;

        group.SetCenter(curTile.tilePos);

        isStartMovement = true;
    }

    private void ChangeTargetTile()
    {
        curTile = targetTile;
        targetTile = null;
        if (curTile == null)
        {
            isStartMovement = false;
            return;
        }

        if(curTile.TryGetNextTile(out targetTile) == false)
        {
            DebugTools.Log("Get to target", DebugColor.Red);
            isStartMovement = false;
            return;
        }

        curMoveDir = (targetTile.tilePos - curTile.tilePos).normalized;

    }

}
