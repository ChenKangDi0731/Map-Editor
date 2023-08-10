using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirGizmos : MonoBehaviour
{

    Vector3 centerPos = Vector3.zero;
    float yOffset = 1f;

    Vector3 xPoint = Vector3.zero;
    Vector3 yPoint = Vector3.zero;
    Vector3 zPoint = Vector3.zero;

    public float gizmosLength = 20f;

    void Awake()
    {
        centerPos = transform.position;
        centerPos.y += yOffset;

        xPoint = centerPos + Vector3.right * gizmosLength;
        yPoint = centerPos + Vector3.up * gizmosLength;
        zPoint = centerPos + Vector3.forward * gizmosLength;
    }

    public void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawLine(centerPos, xPoint);
        Gizmos.DrawWireSphere(xPoint, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(centerPos, yPoint);
        Gizmos.DrawWireSphere(yPoint, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(centerPos, zPoint);
        Gizmos.DrawWireSphere(zPoint, 0.1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centerPos, 0.2f);

    }

}
