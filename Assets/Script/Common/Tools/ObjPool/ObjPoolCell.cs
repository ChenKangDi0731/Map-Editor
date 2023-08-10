using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPoolCell : MonoBehaviour,IObjPoolCell
{
    public bool recycleState = false;
    public int prefabsId;

    public void Recycle()
    {
        //if (recycleState) return;
        ObjPoolManager.Instance.Recycle(this);
    }

    public int GetAssetID()
    {
        return prefabsId;
    }

}

public interface IObjPoolCell
{
    void Recycle();
    int GetAssetID();
}