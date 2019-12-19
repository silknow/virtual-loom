using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstTabManager : Singleton<FirstTabManager>
{
    public Button nextStepButton;

    public bool imgIsLoaded = false;

    private void Update()
    {
        nextStepButton.interactable = imgIsLoaded;
    }
}
