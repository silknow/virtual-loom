using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(RectTransform))]
public class SlidePanel : MonoBehaviour
{
    public bool visible = true;
    private RectTransform _rectTransform;
    private bool animate = false;
    public enum Direction
    {
        Ltr = 1,
        Rtl = -1
    }
    public Direction SlideDirection = Direction.Rtl;
    public LeanTweenType easeType = LeanTweenType.easeInCubic;
    public float animationTime = 0.5f;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
       
    }
    public void TogglePanelAnimation()
    {
        if(animate)
            return;
        animate = true;
        var finalPos = visible ? 0f : (int)SlideDirection *  _rectTransform.sizeDelta.x;
        LeanTween.moveX(_rectTransform, finalPos, animationTime).setEase(easeType).setOnComplete(ToggleVisible);

    }
    private void ToggleVisible()
    {
        visible = !visible;
        animate = false;
    }
}
