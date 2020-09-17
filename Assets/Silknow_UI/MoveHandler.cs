using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;


public class MoveHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public UnityEvent moveCallback = null;

    private RectTransform m_DraggingPlane;

    private Image _image;

    public Sprite pressedSprite;

    private Sprite _defaultSprite;

    // Start is called before the first frame update
    void Awake()
    {
        _image = GetComponent<Image>();
        _defaultSprite = _image.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData) {
        // var canvas = FindInParents<Canvas>(gameObject);
        // if (canvas == null)
        //     return;
        if(pressedSprite!= null)
            _image.sprite = pressedSprite;
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

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.sprite = _defaultSprite;
    }

}
