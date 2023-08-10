using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs_SceneTool : MonoBehaviour
{

    public bool hideAtPlaying = true;

    public float prefabsInterval;
    public float prefabsConstY;

    private void Awake()
    {
        this.gameObject.SetActive(!hideAtPlaying);
    }


    [ContextMenu("Set Prefabs Interval(exclude inactive obj)")]
    public void SetPrefabsInterval()
    {
        int index = 0;
        bool sign = false;
        this.MapAllComponent<Transform>(t =>
        {
            Vector3 newPos = t.position;
            newPos.y = prefabsConstY;

            newPos.x = (sign) ? prefabsInterval * index : prefabsInterval * -index ;

            t.position = newPos;

            if (sign)
            {
                index++;
            }

            sign = !sign;
        });
    }

}
