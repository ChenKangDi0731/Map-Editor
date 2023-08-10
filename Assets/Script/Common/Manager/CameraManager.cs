using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラ管理クラス
/// </summary>
public class CameraManager:Singleton<CameraManager>
{
    #region component_param

    private Camera mainCamera;
    public Camera MainCamera
    {
        get { return mainCamera; }
    }

    #endregion component_param


    public void DoInit()
    {
        //mainCamera = Camera.main;
        if (GameConfig.Instance != null)
        {
            mainCamera = GameConfig.Instance.MainCamera;
        }
        else
        {
            mainCamera = Camera.main;
            Debug.LogError("GameConfig エラー");
        }

        if (mainCamera == null)
        {
            Debug.LogError("Get main camera failed");
        }
    }

    public void SetMainCamera(Camera c)
    {
        if (c == null) return;
        mainCamera = c;
    }

}

