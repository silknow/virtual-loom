using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class RenderHandlerLine : MonoBehaviour
{
    
    public RectTransform[] handlers = new RectTransform[4];

    private UILineRenderer _uiLineRenderer;

    private void Awake()
    {
        _uiLineRenderer = GetComponent<UILineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _uiLineRenderer.Points = new Vector2[5];
        for(var i = 0; i <handlers.Length;i++)
        {
            var handler = handlers[i];

            _uiLineRenderer.Points[i] = handler.switchToRectTransform(this.GetComponent<RectTransform>());
        }
        _uiLineRenderer.Points[4] = handlers[0].switchToRectTransform(this.GetComponent<RectTransform>());
    }
}
