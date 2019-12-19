using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class MoveHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public UnityEvent moveCallback = null;

    private RectTransform m_DraggingPlane;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData) {
        // var canvas = FindInParents<Canvas>(gameObject);
        // if (canvas == null)
        //     return;
        
    }

    public void OnDrag(PointerEventData data) {
        if (data == null)
            return;

        if (data.pointerEnter == null)
            return;

        var rt = GetComponent<RectTransform>();
        Vector3 globalMousePos;
        m_DraggingPlane = data.pointerEnter.transform as RectTransform;
        
        if (m_DraggingPlane == null)
            return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            //rt.rotation = m_DraggingPlane.rotation;
        }

        // Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
        // Canvas c = FindInParents<Canvas>(gameObject);

        RectTransform p = rt.parent as RectTransform;

        Vector3 pos = new Vector3(
            rt.localPosition.x / p.rect.width,
            rt.localPosition.y / p.rect.height,
            0);

        if (moveCallback != null)
            moveCallback.Invoke();

        // Rect rect = RectTransformUtility.PixelAdjustRect(rt, c);
        // Debug.Log(rect.width);
    }

    public void OnEndDrag(PointerEventData eventData) {
    }

}
