using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;

public class AspectInputField : MonoBehaviour
{
    public Slider slider;
    private InputField _inputField;
    public AspectRatioFitter aspectRatioFitter;

    public ZoomImage originalImage;

    private void Awake()
    {
        _inputField = GetComponent<InputField>();
        
    }

    private void OnEnable()
    {
        Invoke("UpdateAspect",0.5f);
    }

    void UpdateAspect()
    {
        _inputField.text = aspectRatioFitter.aspectRatio.ToString(CultureInfo.InvariantCulture);
    }
    public void OnSliderChange(float value)
    {
        _inputField.text = value.ToString(CultureInfo.InvariantCulture);
    }
    public void OnEndEdit(string value)
    {
        slider.value = float.Parse(value.Replace('.', ','));
    }

    public void ResetAspectToOriginal()
    {
        var originalAspect = originalImage.aspect;
        slider.value = originalAspect;
    }
}
