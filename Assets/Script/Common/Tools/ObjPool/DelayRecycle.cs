using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayRecycle : MonoBehaviour,ICustomAction
{
    public float recycleTime;

    public void Execute()
    {
        CustomAction action = this.gameObject.AddComponent<CustomAction>();
        if (action != null)
        {
            action.TakeAction(() =>
            {
                AssetManager.Instance.Destory(this.gameObject);
            }, recycleTime);
        }
    }

}
