using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadSignArrow : MonoBehaviour
{

    static Vector3 upEuler = new Vector3(-90, 0, 0);
    static Vector3 downEuler = new Vector3(90, 0, 0);
    static Vector3 forwardEuler = Vector3.zero;
    static Vector3 backwardEuler = new Vector3(0, 180, 0);
    static Vector3 leftEuler = new Vector3(0, -90, 0);
    static Vector3 rightEuler = new Vector3(0, 90, 0);

    public GameObject childObj;
    bool showState = true;

    [SerializeField] E_CellDir curDir = E_CellDir.None;

    [SerializeField] bool setLocalRot = false;
    [SerializeField]bool setDefaultRot = false;
    [SerializeField] Vector3 defaultEuler;
    Quaternion defaultRot;
    [SerializeField] Vector3 defaultForward;
    Quaternion curRot;

    Vector3 curEuler = Vector3.zero;

    void GetChildObj()
    {
        if (transform.childCount == 0) return;

        childObj = transform.GetChild(0).gameObject;

    }

    #region 生命周期

    private void Awake()
    {
        //设定朝向参数
        if (setDefaultRot)
        {
            if (setLocalRot)
            {
                this.transform.localRotation = Quaternion.Euler(defaultEuler);
            }
            else
            {
                this.transform.rotation = Quaternion.Euler(defaultEuler);
            }
        }
        else
        {
            if (setLocalRot)
            {
                defaultEuler = this.transform.localRotation.eulerAngles;
            }
            else
            {
                defaultEuler = this.transform.rotation.eulerAngles;
            }
        }

        defaultRot = Quaternion.Euler(defaultEuler);
    }

    #endregion 生命周期


    #region 外部方法
    public void DoInit()
    {
        GetChildObj();
        Show(false, true);
        SetDir(E_CellDir.None, true);
    }

    public void Show(bool show, bool forceSetting = false)
    {
        if (childObj == null)
        {
            GetChildObj();
        }

        if (showState == show && forceSetting == false) return;
        if (show && curDir == E_CellDir.None) return;
        showState = show;

        if (childObj.activeSelf != showState)
        {
            childObj.SetActive(showState);
        }
    }

    public void SetDir(E_CellDir dir, bool resetRot = false)
    {
        Quaternion targetRot;
        curDir = dir;
        if (resetRot)
        {
            curEuler = Vector3.zero;
            targetRot = defaultRot;
        }
        else
        {
            switch (dir)
            {
                case E_CellDir.Forward:
                    targetRot = defaultRot;
                    break;
                case E_CellDir.Backward:
                    targetRot = Quaternion.Euler(defaultEuler) * Quaternion.Euler(backwardEuler);
                    break;
                case E_CellDir.Left:
                    targetRot = Quaternion.Euler(defaultEuler) * Quaternion.Euler(leftEuler);
                    break;
                case E_CellDir.Right:
                    targetRot = Quaternion.Euler(defaultEuler) * Quaternion.Euler(rightEuler);
                    break;
                case E_CellDir.Up:
                    targetRot = Quaternion.Euler(defaultEuler) * Quaternion.Euler(upEuler);
                    break;
                case E_CellDir.Down:
                    targetRot = Quaternion.Euler(defaultEuler) * Quaternion.Euler(downEuler);
                    break;
                case E_CellDir.None:
                    targetRot = defaultRot;
                    Show(false);
                    return;
                default:
                    targetRot = defaultRot;
                    break;
            }
        }

        if (setLocalRot)
        {
            this.transform.localRotation = targetRot;
        }
        else
        {
            this.transform.rotation = targetRot;
        }
    }

    #endregion 外部方法
}
