using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OperateDataManager : Singleton<OperateDataManager>
{

    int maxOperatedDataStorageCount;

    //operated stack
    //Stack<OperateData> operatedStack = new Stack<OperateData>();
    CustomsStack<OperateData> operatedStack;
    //undo stack
    //Stack<OperateData> undoStack = new Stack<OperateData>();
    CustomsStack<OperateData> undoStack;

    bool isInit = false;

    #region 外部方法

    public void DoInit(int maxStorageCount=30)
    {
        maxOperatedDataStorageCount = maxStorageCount;
        operatedStack = new CustomsStack<OperateData>(maxOperatedDataStorageCount);
        undoStack = new CustomsStack<OperateData>(maxOperatedDataStorageCount);

        BaseObjPoolManager.Instance.InitBaseObjPool<OperateData>(30, 0);
        isInit = true;
    }

    public void Redo()
    {
        if (undoStack == null || undoStack.Count == 0) return;
        if (undoStack.Peek() == null)
        {
            Debug.LogErrorFormat("[{0}]巻き戻しエラー", this.GetType().Name);
            return;
        }

        ExecuteOperateData(undoStack.Pop(),true);
        GameConfig.Instance.SetOperateText("操作をやり直す");

        //Debug.Log("undoStack count = " + undoStack.Count);
    }
    public void Undo()
    {
        if (operatedStack == null || operatedStack.Count == 0) return;
        if (operatedStack.Peek() == null)
        {
            Debug.LogErrorFormat("[{0}]巻き戻しエラー", this.GetType().Name);
            return;
        }
        OperateData data = operatedStack.Pop();
        OperateData inverseData = GetInverOperateData(data);

        if (inverseData == null)
        {
            Debug.LogErrorFormat("[{0}]巻き戻しエラー");
            return;
        }

        undoStack.Push(data);

        ExecuteOperateData(inverseData,false);
        GameConfig.Instance.SetOperateText("操作を取り消す");
    }

    public void OperationRecord(bool isRedo,E_OperateType operateType,params object[] args)
    {
        if (operatedStack == null) operatedStack = new CustomsStack<OperateData>(maxOperatedDataStorageCount);

        if (isRedo == false)
        {
            undoStack.Clear();
        }

        OperateData data = null;
        
        try
        {
            switch (operateType)
            {
                case E_OperateType.CreateElement:
                    data = new CreateElementOperateData();
                    data.SetCellID((int)args[0]);
                    data.SetBaseCellID((int)args[1]);
                    data.SetCellRotation((Quaternion)args[2]);
                    data.SetElementPrefabID((int)args[3]);

                    ((CreateElementOperateData)data).SetAboveEmptyCellID((int)args[4]);

                    operatedStack.Push(data);
                    //this.PrintOperatedData(operatedStack.Peek());
                    break;

                case E_OperateType.DeleteElement:

                    data = new DeleteElementOperateData();
                    data.SetCellID((int)args[0]);
                    data.SetBaseCellID((int)args[1]);
                    data.SetCellRotation((Quaternion)args[2]);
                    data.SetElementPrefabID((int)args[3]);

                    ((DeleteElementOperateData)data).SetAboveEmptyCellID((int)args[4]);

                    operatedStack.Push(data);
                    //this.PrintOperatedData(operatedStack.Peek());

                    break;

                case E_OperateType.MoveElement:

                    break;

                case E_OperateType.RotateElement:

                    break;

                default:

                    break;
            }
        }
        catch(IndexOutOfRangeException e)
        {
            Debug.LogErrorFormat("[{0}]OperateData生成エラーが発生、info = {1}", this.GetType().Name, e.InnerException);
        }
    }

    #endregion 

    E_OperateType GetInverseOperateType(OperateData data)
    {
        return E_OperateType.None;
    }

    E_OperateType GetInverseOperateType(E_OperateType type)
    {
        return E_OperateType.None;
    }

    OperateData GetInverOperateData(OperateData data)
    {
        if (data == null) {
            return null;
        }

        OperateData returnData = null;

        switch (data.operateType)
        {
            case E_OperateType.CreateElement:

                returnData = new DeleteElementOperateData();
                returnData.SetData(data);

                //EditorMapControl.Instance.DeleteElement(returnData.CellID,false);

                //undoStack.Push(returnData);
                //Debug.LogError("push deleteElementOperate");
                break;

            case E_OperateType.DeleteElement:

                returnData = new CreateElementOperateData();
                returnData.SetData(data);

                //EditorMapControl.Instance.CreateElement(data.ElementPrefabID, data.BaseCellID, data.CellID, data.CellRot);

                //undoStack.Push(returnData);
                //Debug.LogError("push createElementOperate");
                break;

            case E_OperateType.MoveElement:

                break;

            case E_OperateType.ReplaceElement:

                break;

            case E_OperateType.RotateElement:

                break;
            default:

                return null;
        }

        return returnData;
    }

    void ExecuteOperateData(OperateData data,bool isRedo=false)
    {
        if (data == null) return;

        switch (data.operateType)
        {
            case E_OperateType.CreateElement:
                if (data is CreateElementOperateData == false)
                {
                    Debug.LogErrorFormat("[{0}]{1}タイプエラー", this.GetType().Name, data.operateType);
                    return;
                }
                CreateElementOperateData createOperateData = data as CreateElementOperateData;

                EditorMapControl.Instance.CreateElement(data.ElementPrefabID, data.BaseCellID, data.CellID, data.CellRot,createOperateData.AboveEmptyCellID,isRedo);

                break;

            case E_OperateType.DeleteElement:

                if (data is DeleteElementOperateData == false)
                {
                    Debug.LogErrorFormat("[{0}]{1}タイプエラー", this.GetType().Name, data.operateType);
                    return;
                }

                DeleteElementOperateData deleteOperateData = data as DeleteElementOperateData;

                EditorMapControl.Instance.DeleteElement(data.CellID,isRedo);

                break;

            case E_OperateType.RotateElement:

                break;

            case E_OperateType.MoveElement:

                break;

            case E_OperateType.ReplaceElement:

                break;

            default:
                break;
        }

    }

    #region test_method

    public void PrintOperatedData(OperateData data)
    {
        if (data == null) return;

        string str = string.Format("cellID={0},baseCellID={1},rot={2},prefabID={3}---operateType={4}，stackCount = {5}", 
            data.CellID, data.BaseCellID, data.CellRot, data.ElementPrefabID, data.operateType,operatedStack.Count);
        DebugTools.LogError(str, DebugColor.Green);
    }

    #endregion test_method

}



