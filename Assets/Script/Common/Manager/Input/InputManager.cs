using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
///　入力管理クラス
/// </summary>
public class InputManager : MonoSingleton<InputManager>
{
    //InputMode数据（待优化(scriptableObj
    #region inputMode_data
    public static InputModeData editorInputModeData = new InputModeData(E_InputMode.Editor);
    public static InputModeData gameInputModeData = new InputModeData(E_InputMode.Game);
    #endregion inputMode_data

    public bool inputSwitch = false;//入力スイッチ

    Dictionary<int, InputMode> modeInstanceDic = new Dictionary<int, InputMode>();//InputModeリスト

    public Dictionary<int, InputMode> modeDic = new Dictionary<int, InputMode>();//今のInputMode
    //public InputMode curMode;

    //入力イベント
    #region input_event

    //------------------mouse_event
    public event Action event_mouseLeftButtonDown;
    public event Action event_mouseLeftButtonUp;
    public event Action event_mouseRightButtonDown;
    public event Action event_mouseRightButtonUp;
    public event Action event_mouseMidButtonDown;
    public event Action event_mouseMidButtonUp;
    public event Action event_mouseScrollWheelDown;
    public event Action event_mouseScrollWheelUp;
    //------------------keyboard_event
    public event Action event_dirUp;
    public event Action event_dirDown;
    public event Action event_dirLeft;
    public event Action event_dirRight;

    public event Action<bool> event_Button1;
    public event Action<bool> event_Button2;
    public event Action<bool> event_Button3;
    public event Action<bool> event_Button4;
    public event Action<bool> event_Button5;
    public event Action<bool> event_Button6;

    #endregion input_event

    #region input_param_store

    #endregion input_param_store


    //test param
    public bool test;

    //test method

    public void DoInit(GameObject goRoot=null)
    {
        if (goRoot != null)
        {
            this.transform.SetParent(goRoot.transform);
        }

        inputSwitch = true;
        DebugTools.Log("init inputManager", DebugColor.Blue);
    }

    #region lifeCycle
    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inputSwitch == false) return;

        if (modeDic != null)
        {
            foreach (var mode in modeDic)
            {
                if (mode.Value == null) continue;
                mode.Value.DoUpdate(Time.deltaTime);
            }
        }

    }

    #endregion lifeCycle

    InputMode GetModeByType(E_InputMode modeType)
    {
        if (modeInstanceDic == null) modeInstanceDic = new Dictionary<int, InputMode>();

        InputMode returnMode = null;
        if (modeInstanceDic.TryGetValue((int)modeType, out returnMode) == false)
        {
            //Create input mode ,add to modeInstanceDic
        }

        return returnMode;
    }


    #region 外部方法

    public void RegisterInputMode(E_InputMode modeType, InputMode mode, bool needInit = false)
    {
        if (mode == null) return;
        if (modeDic == null) modeDic = new Dictionary<int, InputMode>();

        if (modeDic.ContainsKey((int)modeType) == false)
        {
            modeDic.Add((int)modeType, mode);

            if (needInit)
            {
                mode.DoInit(this);
                mode.MouseInputSwitch(true);
                mode.KeyboardInputSwitch(true);
            }
        }
    }

    public void RegisterInputMode(InputMode mode, bool needInit = false)
    {
        if (modeDic == null) modeDic = new Dictionary<int, InputMode>();

    }

    public void UnRegisterInputMode(InputMode mode, bool needUnInit = false)
    {
        if (modeDic == null) return;
    }

    #region use_scriptableObj
    public void RegisterInputMode(E_InputMode modeType, bool needInit = false)
    {
        if (modeDic == null) modeDic = new Dictionary<int, InputMode>();

        if (modeDic.ContainsKey((int)modeType))
        {
            Debug.LogErrorFormat("リストの中もう存在してる {0}",modeType);
            return;
        }

        //先尝试从实体列表中获取
        bool needAddToInstanceDic = false;
        InputMode mode = null;
        if (modeInstanceDic == null)
        {
            modeInstanceDic = new Dictionary<int, InputMode>();
        }
        if(modeInstanceDic.TryGetValue((int)modeType,out mode) == false)
        {
            needAddToInstanceDic = true;
            mode = InputModeFactory.Instance.GetInputModeInstance(modeType);
        }

        if (mode == null)
        {
            Debug.LogErrorFormat("InputMpdeインスタンス化エラー {0}", modeType);
            return;
        }

        if (needAddToInstanceDic)
        {
            modeInstanceDic.Add((int)modeType, mode);
        }
        modeDic.Add((int)modeType, mode);

        if (needInit)
        {
            mode.DoInit(this);
        }
    }

    public void UnRegisterInputMode(E_InputMode modeType, bool needUnInit = false)
    {
        if (modeDic == null) return;

    }

    #endregion use_scriptableObj

    public void SetInputModeMouseState(bool state)
    {
        if (modeDic == null)
        {
            Debug.LogError("InputMode切り替えエラー");
            return;
        }

        foreach (var mode in modeDic)
        {
            if (mode.Value == null) continue;
            mode.Value.MouseInputSwitch(state);
        }
    }

    public void SetInputModeMouseState(E_InputMode modeType, bool state)
    {
        if (modeDic == null)
        {
            Debug.LogError("InputMode切り替えエラー");
            return;
        }
        InputMode mode = null;
        if (modeDic.TryGetValue((int)modeType, out mode) == false)
        {
            Debug.LogErrorFormat("マウスのInputMode切り替えエラー{0}，InputMode存在しない {1}", state, modeType);
            return;
        }

        mode.MouseInputSwitch(state);
    }

    public void SetInputModeKeyboardState(bool state)
    {
        if (modeDic == null)
        {
            Debug.LogError("InputMode切り替えエラー");

            return;
        }
        foreach(var mode in modeDic)
        {
            if (mode.Value == null) continue;
            mode.Value.KeyboardInputSwitch(state);
        }
    }

    public void SetInputModeKeyboardState(E_InputMode modeType,bool state)
    {
        if (modeDic == null)
        {
            Debug.LogError("InputMode切り替えエラー");

            return;
        }
        InputMode mode = null;
        if (modeDic.TryGetValue((int)modeType, out mode) == false)
        {
            Debug.LogErrorFormat("キーボードのInputMode切り替えエラー{0}，InputMode存在しない {1}", state, modeType);
            return;
        }

        mode.KeyboardInputSwitch(state);
    }

    #region event_Invoke
    public void UpdateMosuePosition(Vector2 mousePos)
    {

    }

    public void InvokeEvent(E_InputEvent eventType,bool down = true)
    {
        switch (eventType)
        {
            //---------------- mouse
            case E_InputEvent.MouseLeft:

                if (down)
                    event_mouseLeftButtonDown?.Invoke();
                else
                    event_mouseLeftButtonUp?.Invoke();

                break;

            case E_InputEvent.MouseMid:
                if (down)
                    event_mouseMidButtonDown?.Invoke();
                else
                    event_mouseMidButtonUp?.Invoke();
                break;

            case E_InputEvent.MouseRight:

                if (down)
                    event_mouseRightButtonDown?.Invoke();
                else
                    event_mouseRightButtonUp?.Invoke();
                break;

            case E_InputEvent.MouseScrollWheel:
                if (down)
                    event_mouseScrollWheelDown?.Invoke();
                else
                    event_mouseScrollWheelUp?.Invoke();
                break;

            #region keyboard_event
            //---------------------- keyboard
            case E_InputEvent.Up:

                test = true;
                break;
            case E_InputEvent.Down:

                test = false;
                break;

            case E_InputEvent.Button1:
                event_Button1?.Invoke(down);
                break;

            case E_InputEvent.Button2:
                event_Button2?.Invoke(down);
                break;

            case E_InputEvent.Button3:
                
                event_Button3?.Invoke(down);

                break;

            case E_InputEvent.Button4:

                break;

            case E_InputEvent.Button5:

                break;

            case E_InputEvent.Button6:

                break;

            #endregion keyboard_event

            default:
                DebugTools.Log("Input other", DebugColor.Red);
                break;
        }
    }

    #endregion event_Invoke

    #region event_bind

    #endregion event_bind

    public void AddInputMode(E_InputMode modeType)
    {
        RegisterInputMode(modeType,true);
    }


    #endregion 外部方法




}

public class InputModeData
{
    public E_InputMode modeType;
    public InputModeData(E_InputMode mType)
    {
        modeType = mType;
    }

    public InputMode GetModeInstance()
    {
        return InputModeFactory.Instance.GetInputModeInstance(modeType);
    }
}
/// <summary>
/// Inputmodeファクトリー
/// </summary>
public class InputModeFactory:Singleton<InputModeFactory>
{

    public InputMode GetInputModeInstance(E_InputMode modeType)
    {
        switch (modeType)
        {
            case E_InputMode.Editor:

                return new InputMode_EditorMap();

            default:
                break;
        }
        return null;
    }

    public InputMode GetInputModeInstance(InputModeData data)
    {
        return null;
    }
}


public enum E_InputMode
{
    Editor=-1,
    Game=0
}

public enum E_InputEvent
{
    //---------------------- mouse
    MouseLeft,
    MouseRight,
    MouseMid,
    MouseScrollWheel,

    MouseUpdatePos,
    //------------------------keyboard
    Up,
    Down,
    Left,
    Right,

    Button1,
    Button2,
    Button3,
    Button4,
    Button5,
    Button6,
}