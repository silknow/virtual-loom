using System;
using System.Collections;
using System.Collections.Generic;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class ChangeYarnResolution : MonoBehaviour
{
    public I18NText numberOfYarnsText;
    
    
    private void OnEnable()
    {
        
        GetComponent<Slider>().minValue = 1;

        GetComponent<Slider>().maxValue = Mathf.Max(1,
            WizardController.instance.instantiatedPatch.backgroundPattern.getOriginalResolution().x / 50);
        
        if (WizardController.instance.instantiatedPatch != null)
            GetComponent<Slider>().value = WizardController.instance.instantiatedPatch.divider;
        numberOfYarnsText.updateParam(WizardController.instance.instantiatedPatch.resolution.x.ToString(),0);

    }

    public void OnYarnSliderChange(float value)
    {
        WizardController.instance.instantiatedPatch.divider = value;
        numberOfYarnsText.updateParam(""+(int)(WizardController.instance.instantiatedPatch.backgroundPattern.getOriginalResolution().x/value),0);
    }
}
