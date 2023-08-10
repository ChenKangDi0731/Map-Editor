using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public interface ICustomAction
{
    void Execute();
}

public class CustomActionManager : Singleton<CustomActionManager>
{
    int default_actionID = 0;
    int actionID;
    int maxSetIDTimes = int.MaxValue;
    Dictionary<int, CustomAction> actionDic = new Dictionary<int, CustomAction>();

    bool totalActionState = false;

    #region 生命周期

    public void DoUpdate(float deltatime)
    {
        if (totalActionState)
        {
            /*
            foreach(var temp in actionDic)
            {
                if (temp.Value != null)
                {
                    temp.Value.DoUpdate(deltatime);
                }
            }
            */
            Dictionary<int,CustomAction>.ValueCollection.Enumerator tempEnumerator = actionDic.Values.GetEnumerator();
            while (tempEnumerator.MoveNext())
            {
                var cur = tempEnumerator.Current;
                if (cur != null)
                {
                    cur.DoUpdate(deltatime);
                }
            }
        }
    }

    public void DoFixedUpdate(float fixedUpdateTime)
    {
        if (totalActionState)
        {
            /*
            foreach(var temp in actionDic)
            {
                if (temp.Value != null)
                {
                    temp.Value.DoFixedUpdate(fixedUpdateTime);
                }
            }
            */

            Dictionary<int, CustomAction>.ValueCollection.Enumerator tempEnumerator = actionDic.Values.GetEnumerator();
            while (tempEnumerator.MoveNext())
            {
                var cur = tempEnumerator.Current;
                if (cur != null)
                {
                    cur.DoUpdate(fixedUpdateTime);
                }
            }
        }
    }

    #endregion 生命周期

    #region 外部方法

    public void DoInit()
    {
        actionID = default_actionID;
        totalActionState = true;

        if (actionDic == null) actionDic = new Dictionary<int, CustomAction>();
        actionDic.Clear();
    }

    public void SetState(bool state)
    {
        totalActionState = state;
    }

    public void RegisterAction(CustomAction action)
    {
        if (action == null) return;
        //int count = 0;
        //do
        //{
            //count++;
            action.SetActionID(actionID++);
        //} while (actionDic.ContainsKey(action.GetActionID()) == false && count < maxSetIDTimes);

        actionDic.Add(action.GetActionID(), action);

    }

    public void UnregisterAction(CustomAction action)
    {
        if (action == null) return;
        UnregisterAction(action.GetActionID());
    }

    public void UnregisterAction(int id)
    {
        if (actionDic.ContainsKey(id) == false)
        {
            return;
        }

        actionDic.Remove(id);
    }

    public void PauseActoin(int id)
    {

    }

    public void ContinueAction(int id)
    {

    }

    #endregion 外部方法
}

/// <summary>
/// 自定义Action执行类，可派生
/// </summary>
public class CustomAction : MonoBehaviour
{
    int actionID;

    float delayTime = 0;
    float timePass = 0;
    bool startCounting = false;
    bool executed = false;
    Action a;

    #region 生命周期

    protected virtual void OnEnable()
    {
        actionID = -1;
        delayTime = 0;
        timePass = 0;
        startCounting = false;
        executed = false;
        a = null;
    }

    protected virtual void OnDisable()
    {
        actionID = -1;
        delayTime = 0;
        timePass = 0;
        startCounting = false;
        executed = false;
        a = null;

        CustomActionManager.Instance.UnregisterAction(this);

        GameObject.Destroy(this);
    }

    public virtual void DoUpdate(float deltatime)
    {
        if (startCounting && executed == false)
        {
            timePass += deltatime;
            if (timePass >= delayTime)
            {
                TakeAction(a);
            }
        }
        else if (executed)//执行完的下一帧回收
        {
            CustomActionManager.Instance.UnregisterAction(this);
        }
    }

    public virtual void DoFixedUpdate(float fixedUpdateTime)
    {

    }

    protected virtual void TakeAction(Action action)
    {
        action?.Invoke();

        GameObject.Destroy(this);
    }

    #endregion 生命周期

    public virtual void TakeAction(Action action,float delay)
    {
        if (action == null) return;

        a = action;

        delayTime = (delay < 0) ? 0 : delay;
        timePass = 0;

        CustomActionManager.Instance.RegisterAction(this);

        startCounting = true;
    }

    #region 外部方法

    public void SetActionID(int id)
    {
        actionID = id;
    }

    public int GetActionID()
    {
        return actionID;
    }

    public void Pause()
    {

    }

    public void Continue()
    {

    }

    public void Stop()
    {

    }

    #endregion 外部方法
}
