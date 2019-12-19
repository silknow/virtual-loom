using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenColorPicker : MonoBehaviour
{
    public YarnPanel parentPanel;

    public void OpenPicker()
    {
        if(parentPanel!= null)
            WizardController.instance.selectedYarnPanel = parentPanel;
        WizardController.instance.ToggleColorPickerWindow();
    }
}
