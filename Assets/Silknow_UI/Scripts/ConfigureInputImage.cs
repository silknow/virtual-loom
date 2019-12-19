using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureInputImage : MonoBehaviour
{
    private Texture2D displayImage;

    public Slider aspectSlider;
    // Start is called before the first frame update
    void Start()
    {
        displayImage = (Texture2D)GetComponent<RawImage>().mainTexture;
    }


    public void RotateTexture(bool clockwise)
    {
        var tex  = WizardController.instance.RotateTexture(displayImage, clockwise);
        GetComponent<RawImage>().texture = tex;
        GetComponent<RectTransform>().anchorMin = Vector2.zero;
        GetComponent<RectTransform>().anchorMax = Vector2.one;
        displayImage = tex;
        GetComponent<ZoomImage>().UpdateRawImageAspect();
        HomographyImage.getInstance().GetTextureFromReference();
        HomographyImage.getInstance().GetComponent<ZoomImage>().UpdateRawImageAspect();
        HomographyImage.getInstance().aspect = HomographyImage.getInstance().GetComponent<ZoomImage>().aspect;
        aspectSlider.value = HomographyImage.getInstance().GetComponent<ZoomImage>().aspect;
    }

    private void OnEnable()
    {
        if (!WizardController.instance.inputTexture) return;
        displayImage = WizardController.instance.inputTexture;
        GetComponent<RawImage>().texture = displayImage;
        HomographyImage.getInstance().GetTextureFromReference();
        HomographyImage.getInstance().GetComponent<ZoomImage>().UpdateRawImageAspect();
        HomographyImage.getInstance().aspect = HomographyImage.getInstance().GetComponent<ZoomImage>().aspect;
        aspectSlider.value = HomographyImage.getInstance().GetComponent<ZoomImage>().aspect;
    }
}
