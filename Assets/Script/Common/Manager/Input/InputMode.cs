using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMode
{
    public InputManager parentManager;

    #region state

    public bool isMainMode = false;//是否主要输入模式

    public bool mouseSwitch;
    public bool mousePositionUpdateSwitch;
    public bool keyboardSwitch;

    #endregion state

    #region data_store

    private Vector2 mousePos;
    public Vector2 mousePosition { get { return mousePos; } }


    #endregion data_store

    public virtual void DoInit(InputManager manager)
    {
        if (manager == null) return;

        parentManager = manager;
    }

    public void DoUpdate(float deltatime)
    {
        if (mouseSwitch) { MouseInput(); }
        if (keyboardSwitch) { KeyboardInput(); }

        if (mousePositionUpdateSwitch) { }
    }

    public virtual void MouseInput()
    {

    }

    public virtual void KeyboardInput()
    {

    }

    public virtual void MousePositionUpdate()
    {

    }


    #region 外部方法

    public virtual void InitInput(InputManager mgr)
    {

    }

    public virtual void UnInitInput(InputManager mgr)
    {

    }

    public virtual void MouseInputSwitch(bool state)
    {
        mouseSwitch = state;
        DebugTools.Log("mouse switch = " + state);
    }

    public virtual void MousePositionSwtich(bool state)
    {
        mousePositionUpdateSwitch = state;
    }

    public virtual void KeyboardInputSwitch(bool state)
    {
        keyboardSwitch = state;
        DebugTools.Log("keyboard switch = " + state);
    }

    public virtual void AllInputSwitch(bool state)
    {

    }

    #endregion 外部方法

}
