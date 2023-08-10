using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking_CameraLook : MonoBehaviour
{

    public Transform lookTransform;


    // Update is called once per frame
    void Update()
    {
        if (lookTransform != null)
        {
            this.transform.LookAt(lookTransform);
        }
    }
}
