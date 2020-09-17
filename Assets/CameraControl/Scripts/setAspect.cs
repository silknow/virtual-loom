using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setAspect : MonoBehaviour
{
    public float aspect;
    public void set(float aspect)
    {
        this.aspect = aspect;
        
            ((RectTransform) transform).sizeDelta = new Vector2(((RectTransform) transform).sizeDelta.x,
                ((RectTransform) transform).sizeDelta.x / (1.0f * aspect));
            if (aspect<1.0f)
            {
                Vector3 ap = ((RectTransform) transform).anchoredPosition;
                ((RectTransform) transform).anchoredPosition=new Vector3(20+200*aspect,ap.y,ap.z);
                transform.localScale=Vector3.one*aspect;
            }
    }

}
