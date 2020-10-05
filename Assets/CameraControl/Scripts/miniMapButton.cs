using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;

public class miniMapButton : MonoBehaviour,IBeginDragHandler,IEndDragHandler, IDragHandler, IPointerClickHandler
{
    public CameraControl cc;

    private Vector3 _lastMousePos;

    public bool dragging;
    // Start is called before the first frame update
    void Start()
    {
        _lastMousePos = Input.mousePosition;
        dragging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging) return;
        Vector2 rectParent = transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta;
        transform.localPosition=new Vector3(cc.normalizedPosition.x*100-rectParent.x/2.0f,cc.normalizedPosition.y*100-rectParent.y/2.0f,0);
        transform.localRotation=Quaternion.AngleAxis(-cc.rotation, Vector3.forward);

        RectTransform t = (RectTransform) transform;
        
        t.anchorMax = cc.normalizedSize/2;
        t.anchorMin = -cc.normalizedSize/2;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _lastMousePos = Input.mousePosition;
            dragging = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 rectParent = transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta;
        Vector3 p=(Input.mousePosition - _lastMousePos)/GetComponentInParent<Canvas>().transform.localScale.x;
        transform.localPosition += p;
        _lastMousePos = Input.mousePosition;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        Vector2 rectParent = transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta;
        cc.setNormalizedPosition(new Vector2((transform.localPosition.x + (rectParent.x / 2.0f)) / 100,
            (transform.localPosition.y + (rectParent.y / 2.0f)) / 100));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2)
        {
            cc.resetRotation();
        }
            
        
        
    }
}
