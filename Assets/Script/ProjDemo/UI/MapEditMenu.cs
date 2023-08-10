using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MapEditMenu : MonoBehaviour
{
    public  EventSystem es;
    public GraphicRaycaster raycastInCanvas;
    private void Awake()
    {
        es = EventSystem.current;
        if (es == null)
        {
            Debug.LogError("event system is null");
        }
    }

    // Update is called once per frame
    void Update()
    {

        MapEditorManager.Instance.ChangeOperateState(CheckUIBlock());
        
    }

    /// <summary>
    /// ui遮挡
    /// </summary>
    /// <returns></returns>
    public bool CheckUIBlock()
    {
        if (es == null) return true;

        PointerEventData eventData = new PointerEventData(es);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        raycastInCanvas.Raycast(eventData, list);

        return list.Count == 0;
    }
}
