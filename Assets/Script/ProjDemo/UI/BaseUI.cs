using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{

    public GameObject uiRoot;

    public bool uiState = false;


    public virtual void DoInit()
    {

    }

    public virtual void ShowUI(bool show)
    {
        if (uiRoot == null) return;

        if (uiRoot.activeSelf != show)
        {
            uiRoot.SetActive(show);
        }

    }

    public virtual bool IsUIShow()
    {
        return uiRoot.activeSelf;
    }

    public virtual void UIStateSwitch(bool state)
    {
        uiState = state;
    }

    public virtual void UIStateSwitch()
    {
        uiState = !uiState;
    }
}
