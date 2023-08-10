using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementConfig : MonoBehaviour
{
    [SerializeField] E_ElementType elementType;
    [SerializeField] bool canSetElement;
    [SerializeField] Vector3 elementSize = Vector3.one;
    public E_ElementType ElementType
    {
        get { return elementType; }
    }

    public bool CanSetElement
    {
        get { return canSetElement; }
    }

    public Vector3 ElementSize
    {
        get { return elementSize; }
    }

    #region tool_method

    [ContextMenu("Auto Set Type")]
    public void SetType()
    {
        string name = this.gameObject.name;
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("set type failed");
            return;
        }


        if (name.Contains("#"))
        {
            int sharpStrIndex = name.IndexOf("#");
            if (name.Length > sharpStrIndex + 1)
            {

                string idStr = name.Substring(sharpStrIndex + 1);

                try
                {
                    int id = int.Parse(idStr);

                    elementType = (E_ElementType)id;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    Debug.LogError("set type failed");
                }
            }
        }
        

    }

    [Header("EditorMethodParam")]
    public E_ColliderType colliderType = E_ColliderType.Box;

    [ContextMenu("Set Prefabs Collider")]
    public void SetCollider()
    {
        if (this.gameObject.GetComponent<Collider>() != null)
        {
            return;
        }

        int childCount = transform.childCount;
        if (childCount > 1 || childCount == 0)
        {
            return;
        }

        Transform childT = transform.GetChild(0);
        if (childT != null)
        {
            Renderer r = childT.GetComponent<Renderer>();
            Collider c = childT.GetComponent<Collider>();
            if (r == null)
            {
                return;
            }

            if (c == null)
            {
                switch (colliderType)
                {
                    case E_ColliderType.Box:
                        c = childT.gameObject.AddComponent<BoxCollider>();
                        break;
                    case E_ColliderType.Capsule:
                        c = childT.gameObject.AddComponent<CapsuleCollider>();
                        break;

                    case E_ColliderType.Mesh:
                        c = childT.gameObject.AddComponent<MeshCollider>();
                        break;

                }


            }

            UnityEditorInternal.ComponentUtility.CopyComponent(c);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(this.gameObject);

            DestroyImmediate(c);
        }

    }

    [ContextMenu("SetPrefabsCollider2")]
    public void SetPrefabCollider2()
    {
        if (transform.childCount == 0)
        {
            return;
        }

        this.MapAllComponent<Collider>(cur =>
         {
             UnityEngine.Object.DestroyImmediate(cur);
         });

        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        Vector3 scale = transform.localScale;
        Vector3 center = Vector3.zero;
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;

        Renderer[] rArray = this.GetComponentsInChildren<Renderer>();
        foreach (Renderer cur in rArray)
        {
            center += cur.bounds.center;
        }

        center /= rArray.Length;

        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach(Renderer cur in rArray)
        {
            bounds.Encapsulate(cur.bounds);
        }

        Collider collider;
        switch (colliderType)
        {
            case E_ColliderType.Box:
                collider = this.gameObject.AddComponent<BoxCollider>();
                ((BoxCollider)collider).center = bounds.center - this.transform.position;
                ((BoxCollider)collider).size = bounds.size;
                break;
            case E_ColliderType.Capsule:
                collider = this.gameObject.AddComponent<CapsuleCollider>();
                ((CapsuleCollider)collider).center = bounds.center - this.transform.position;
                ((CapsuleCollider)collider).radius = bounds.size.x / 2;
                ((CapsuleCollider)collider).height = bounds.size.y;
                
                break;
            default:
                break;
        }

        this.transform.position = position;
        this.transform.rotation = rotation;
        this.transform.localScale = scale;

    }

    [ContextMenu("DeleteChildCollider")]
    public void DestroyChildCollider()
    {
        if (transform.childCount == 0) return;

        Transform child = transform.GetChild(0);
        BoxCollider c = child.GetComponent<BoxCollider>();
        if (c == null) return;
        DestroyImmediate(c);

        
    }

    public enum E_ColliderType
    {
        Box,
        Capsule,
        Mesh,
    }

    #endregion tool_method
}