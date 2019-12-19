using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeYarnResolution : MonoBehaviour
{
    public Text numberOfYarnsText;
    
    
    private void OnEnable()
    {
        
        GetComponent<Slider>().minValue = 1;

        GetComponent<Slider>().maxValue = Mathf.Max(1,
            WizardController.instance.instantiatedPatch.backgroundPattern.getOriginalResolution().x / 50);
        
        if (WizardController.instance.instantiatedPatch != null)
            GetComponent<Slider>().value = WizardController.instance.instantiatedPatch.divider;
        numberOfYarnsText.text = WizardController.instance.instantiatedPatch.resolution.x.ToString() + " yarns";

    }

    public void OnYarnSliderChange(float value)
    {
        WizardController.instance.instantiatedPatch.divider = value;
        numberOfYarnsText.text = ((int)(WizardController.instance.instantiatedPatch.backgroundPattern.getOriginalResolution().x / value)).ToString() + " yarns";
    }
}
