using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineRotControl : MonoBehaviour
{
    bool initOnce = false;//只需要初始化一次

    public float rotSpeed;

    bool startRot = false;

    Quaternion _defaultRot;
    Vector3 _defaultEuler;

    Vector3 rotTargetDir;

    // Start is called before the first frame update
    void Awake()
    {
        //record default rot

    }

    private void OnDisable()
    {
        ResetParam();
    }

    // Update is called once per frame
    public bool RotSpine()
    {
        if (startRot)
        {

            return true;
        }
        else
        {
            return false;
        }
    }
    

    public void SetRotTargetDir(Vector3 targetDir)
    {

        rotTargetDir = targetDir;
        startRot = true;
    }

    public void SetRotDefault()
    {

    }

    public void ResetParam()
    {
        startRot = false;
        rotTargetDir = Vector3.zero;
    }
}
