using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZoomImage : MonoBehaviour, IScrollHandler
{

    public float zoomSpeed = 0.1f;
    public float maxZoom = 10f;
    public float minZoom = 1f;
    public float aspect = 1f;
    [SerializeField]
    private Vector3 initialScale;
    [SerializeField]
    private Vector3 unscaledInitialScale;

    public bool fitSizeOnStart = true;

    void Awake()
    {
        initialScale = Vector3.one;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (fitSizeOnStart)
        {

           
            if (TryGetComponent(out Image im))
            {
                aspect = im.sprite.rect.width / im.sprite.rect.height;
                Debug.Log(aspect);
                if (GetComponent<AspectRatioFitter>())
                {
                    GetComponent<AspectRatioFitter>().aspectRatio = aspect;
                    GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                }
            }
            else
            {
                UpdateRawImageAspect();
            }


        }
        /*
        // UnityEngine.UI.RawImage imr = GetComponent<UnityEngine.UI.RawImage>();
            // if (imr != null)
            //     aspect = imr.texture.width / imr.texture.height;

            Canvas c = AUtils.FindInParents<Canvas>(gameObject);
            RectTransform rt = GetComponent<RectTransform>();
            Rect rectImage = RectTransformUtility.PixelAdjustRect(rt, c);

            RectTransform rtp = rt.parent as RectTransform;
            Rect rectViewport = RectTransformUtility.PixelAdjustRect(rtp, c);

          
            float scalex = rectViewport.width / (rectImage.width * aspect);
            Debug.Log(scalex);
            float scaley = rectViewport.height/rectImage.height;
            Debug.Log(scaley);

            if (scalex < scaley)
            {
                transform.localScale = new Vector3(scalex * aspect, scaley, 1);
            }
            else
            {
                transform.localScale = new Vector3(scaley*aspect, scaley, 1);
            }
          
            
        }
       */
        initialScale = transform.localScale;
        unscaledInitialScale = new Vector3(initialScale.x / aspect, initialScale.y, initialScale.z);
     
    }

    public void OnScroll(PointerEventData eventData) {
        var delta = Vector3.one * (eventData.scrollDelta.y * zoomSpeed);
        var desScale = transform.localScale + delta;
        desScale = ClampDesiredScale(desScale);
        transform.localScale = desScale;
    }

    private Vector3 ClampDesiredScale(Vector3 desScale) {
        desScale = Vector3.Max(initialScale * minZoom, desScale);
        desScale = Vector3.Min(initialScale * maxZoom, desScale);
        return desScale;
    }

    public void OnAspectChangeValue(float value)
    {
        aspect = value;

    
        if (GetComponent<AspectRatioFitter>())
        {
            GetComponent<AspectRatioFitter>().enabled = true;
            GetComponent<AspectRatioFitter>().aspectRatio = aspect;
            StartCoroutine(DisableAspectRatio());
           
        }
    }


    public void UpdateRawImageAspect()
    {
        if(TryGetComponent(out RawImage rim))
        {
            if (rim.texture != null)
            {
                aspect = (float) rim.texture.width / (float) rim.texture.height;
                if (GetComponent<AspectRatioFitter>())
                {
                    GetComponent<AspectRatioFitter>().enabled = true;
                    GetComponent<AspectRatioFitter>().aspectRatio = aspect;
                    GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.FitInParent;
                    StartCoroutine(DisableAspectRatio());
                }
            }
        }
    }

    private IEnumerator DisableAspectRatio()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<AspectRatioFitter>().enabled = false;
    }
    private void OnRectTransformDimensionsChange()
    {
        // The RectTransform has changed!
        if (gameObject.activeInHierarchy && GetComponent<AspectRatioFitter>())
        {
            GetComponent<AspectRatioFitter>().enabled = true;
            GetComponent<AspectRatioFitter>().aspectRatio = aspect;
            StartCoroutine(DisableAspectRatio());
           
        }
    }

}
