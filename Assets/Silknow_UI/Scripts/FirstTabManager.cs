using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstTabManager : Singleton<FirstTabManager>
{
    public Button nextStepButton;

    public bool imgIsLoaded = false;

    public Toggle tab2Toggle;
    private void Update()
    {
        nextStepButton.interactable = imgIsLoaded;
    }

    public void ChangeToSecondTab()
    {
        tab2Toggle.isOn = true;
    }
}
