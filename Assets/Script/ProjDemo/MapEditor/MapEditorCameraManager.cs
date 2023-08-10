using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorCameraManager : Singleton<MapEditorCameraManager>
{

    public Camera controlCamera;
    public Transform controlCameraRootPoint;
    #region state

    public Vector2 cameraZoomRange;
    public int cameraZoomInterval;

    public float cameraHorizontalMoveFactor;
    public float cameraMoveSpeed = 2f;
    public float cameraZoomSpeed = 5f;
    public float cameraRotateSpeed = 10f;

    public bool zoomStart = false;
    public bool cameraMoveState = true;
    public bool canHorizontalMoveCamera = false;
    public bool rotateStart = false;

    #endregion

    #region default_state

    public float defaultZoomValue;

    public Vector3 defaultForward;
    public Vector3 defaultRotateEuler = Vector3.zero;

    #endregion default_state

    #region param_store

    float cameraZoomValue = -1;

    Vector3 startMousePosition = Vector3.zero;
    Vector3 startCameraPosition = Vector3.zero;

    //Vector3 updateMouseVector = Vector3.zero;
    //Vector3 updateCameraVector = Vector3.zero;

    float cameraZoomTargetValue;
    Vector3 cameraTargetPos = Vector3.zero;


    Vector3 cameraTargetRotateEuler = Vector3.zero;
    Vector3 cameraStartForward = Vector3.zero;
    Vector3 cameraTargetForward = Vector3.zero;
    bool isRightRotate;
    float curRotateIncreaseValue = 0;
    float targetRotateAngleValue = 0;

    #endregion param_store

    #region operate_interval_param

    //bool canSetZoomValue = true;
    //public float operateZoomInterval =0.5f;
    //float curZoomOperateTimePass;

    #endregion operate_interval_param

    #region lifeCycle

    public void DoUpdate(float deltatime)
    {
        if (cameraMoveState == false) return;
        MoveCameraHorizontal();
        UpdateCameraPosition(deltatime);
        UpdateCameraZoomValue(deltatime);
        UpdateCameraRotate(deltatime);

    }

    public void DoFixedUpdate(float fixedUpdateTime)
    {

    }

    #endregion lifeCycle

    void InitInputEvent()
    {

    }

    void UpdateCameraPosition(float deltatime)
    {
        if (controlCamera == null ||controlCameraRootPoint==null)
        {
            return;
        }

        controlCameraRootPoint.position = Vector3.Lerp(controlCameraRootPoint.position, cameraTargetPos, deltatime * cameraMoveSpeed);
    }

    void UpdateCameraZoomValue(float deltatime)
    {

        if (controlCamera == null)
        {
            zoomStart = false;
            return;
        }

        if (zoomStart)
        {
            cameraZoomValue = Mathf.Lerp(cameraZoomValue, cameraZoomTargetValue, deltatime * cameraZoomSpeed);
            controlCamera.transform.localPosition = new Vector3(controlCamera.transform.localPosition.x, cameraZoomValue, controlCamera.transform.localPosition.z);

            if (Mathf.Abs(cameraZoomValue - cameraZoomTargetValue) < 0.001f)
            {
                zoomStart = false;
            }
        }

    }

    void UpdateCameraRotate(float deltaTime)
    {
        if (rotateStart)
        {
            //curRotateIncreaseValue += deltaTime * cameraRotateSpeed;
            controlCameraRootPoint.forward = Vector3.Lerp(controlCameraRootPoint.forward, cameraTargetForward, deltaTime * cameraRotateSpeed);

            if (Vector3.Dot(controlCameraRootPoint.forward, cameraTargetForward) >= 1f)
            {
                controlCameraRootPoint.forward = cameraTargetForward;
                rotateStart = false;
            }
        }
    }

    #region input_event

    public void MoveCameraHorizontal()
    {
        if (canHorizontalMoveCamera == false) return;
        if (controlCamera == null)
        {
            Debug.LogError("[MapEditorCameraManager]カメラ獲得エラー");
            canHorizontalMoveCamera = false;
            return;
        }

        Vector3 mousePosOffset = Input.mousePosition - startMousePosition;

        mousePosOffset.z = mousePosOffset.y;
        mousePosOffset = controlCamera.transform.TransformPoint(mousePosOffset);
        mousePosOffset.y = 0;


        cameraTargetPos = startCameraPosition - mousePosOffset * cameraHorizontalMoveFactor;

    }

    public void CameraZoom(bool isZoomIn, int zoomValue = 1)
    {
        cameraZoomTargetValue += isZoomIn ? -zoomValue : zoomValue;
        cameraZoomTargetValue = Mathf.Clamp(cameraZoomTargetValue, cameraZoomRange.x, cameraZoomRange.y);

        zoomStart = true;
    }

    public void CameraRotate(bool isRight)
    {

        cameraTargetRotateEuler.y += isRight ? 90f : -90f;

        cameraTargetRotateEuler.y = (cameraTargetRotateEuler.y + 360f) % 360;

        cameraTargetForward = Quaternion.Euler(cameraTargetRotateEuler) *defaultForward;

        //Debug.LogErrorFormat("camera rotate invoke {0} , targetForward = {1}", isRight, cameraTargetForward);


        rotateStart = true;
    }

    #endregion input_event

    #region 外部方法

    public void DoInit()
    {
        //set camera
        controlCamera = CameraManager.Instance.MainCamera;
        GameConfig.Instance.SetCamera2DefaultPoint(controlCamera);
        GameConfig.Instance.SetCameraFollowRootPoint(controlCamera, out controlCameraRootPoint);
        InitInputEvent();

        if (controlCamera != null &&controlCameraRootPoint!=null )
        {
            cameraTargetPos = controlCameraRootPoint.position;
            cameraZoomValue = controlCamera.transform.localPosition.z;
            defaultZoomValue = cameraZoomValue;
            cameraZoomTargetValue = defaultZoomValue;

            defaultForward = controlCameraRootPoint.forward;
            defaultRotateEuler = controlCameraRootPoint.eulerAngles;

        }
        SetCameraMoveState(true);
        SetHorizontalMoveCamera(false);

        if (MapEditorManager.Instance.operateData != null)
        {
            cameraHorizontalMoveFactor = MapEditorManager.Instance.operateData.cameraHorizontalMoveFactor;
            cameraMoveSpeed = MapEditorManager.Instance.operateData.cameraMoveSpeed;
            cameraZoomSpeed = MapEditorManager.Instance.operateData.cameraZoomSpeed;
            cameraZoomRange = MapEditorManager.Instance.operateData.cameraZoomRange;
            cameraZoomInterval = MapEditorManager.Instance.operateData.zoomValueInterval;
        }

        cameraZoomTargetValue = Mathf.Clamp(cameraZoomTargetValue, cameraZoomRange.x, cameraZoomRange.y);
        //canSetZoomValue = true;
        //curZoomOperateTimePass = 0;
        zoomStart = true;

    }


    /// <summary>
    /// カメラ平行移動
    /// </summary>
    /// <param name="state"></param>
    public void SetHorizontalMoveCamera(bool state)
    {
        if (state == canHorizontalMoveCamera) return;

        canHorizontalMoveCamera = state;
        if (state)
        {
            startMousePosition = Input.mousePosition;
            startCameraPosition = controlCameraRootPoint.position;
        }
    }

    public void SetCameraMoveState(bool state)
    {
        if (controlCamera == null)
        {
            Debug.LogError("カメラ獲得エラー");
            cameraMoveState = false;
            return;
        }
        cameraMoveState = state;
    }

    #endregion 外部方法


}

