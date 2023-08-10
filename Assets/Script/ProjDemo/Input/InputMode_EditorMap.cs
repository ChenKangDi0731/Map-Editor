using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMode_EditorMap : InputMode
{
    #region input_event

    public event Action<E_InputEvent,bool> inputEventInvoke;

    #endregion input_event

    public override void DoInit(InputManager manager)
    {
        base.DoInit(manager);

        if (manager == null) return;

        inputEventInvoke += manager.InvokeEvent;
    }

    public override void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            inputEventInvoke?.Invoke(E_InputEvent.Up,true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            inputEventInvoke?.Invoke(E_InputEvent.Down,true);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            inputEventInvoke?.Invoke(E_InputEvent.Button1, true);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            inputEventInvoke?.Invoke(E_InputEvent.Button2, true);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            inputEventInvoke?.Invoke(E_InputEvent.Button3, true);
        }
        

    }

    public override void MouseInput()
    {

        if (Input.GetMouseButtonDown(0))
        {
            inputEventInvoke?.Invoke(E_InputEvent.MouseLeft,true);
        }else if (Input.GetMouseButtonUp(0))
        {
            inputEventInvoke?.Invoke(E_InputEvent.MouseLeft, false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            inputEventInvoke?.Invoke(E_InputEvent.MouseRight, true);
        }else if (Input.GetMouseButtonUp(1))
        {
            inputEventInvoke?.Invoke(E_InputEvent.MouseRight, false);
        }

        if (Input.GetMouseButtonDown(2))
        {
            inputEventInvoke?.Invoke(E_InputEvent.MouseMid, true);
        }
        else if (Input.GetMouseButtonUp(2))
        {
            inputEventInvoke?.Invoke(E_InputEvent.MouseMid, false);
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            inputEventInvoke?.Invoke(E_InputEvent.MouseScrollWheel, false);
        }else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            inputEventInvoke?.Invoke(E_InputEvent.MouseScrollWheel, true);
        }


    }

    public override void MousePositionUpdate()
    {

    }

}
