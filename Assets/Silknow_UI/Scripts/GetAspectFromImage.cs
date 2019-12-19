using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAspectFromImage : MonoBehaviour
{
    public AspectRatioFitter aspectRatioFitter;
    // Start is called before the first frame update
    void Start()
    {
       Invoke("UpdateAspect",0.5f);
    }

    void UpdateAspect()
    {
        GetComponent<Slider>().value = aspectRatioFitter.aspectRatio;
    }


}
