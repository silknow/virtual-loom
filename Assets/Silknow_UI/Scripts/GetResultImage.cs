using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetResultImage : MonoBehaviour
{
    private void Start()
    {
        GetCorrectedImage();
    }
    private void OnEnable()
    {
        GetCorrectedImage();
    }

    private void GetCorrectedImage()
    {
        GetComponent<RawImage>().texture = WizardController.instance.homographyResult;
        GetComponent<ZoomImage>().UpdateRawImageAspect();
    }
}
