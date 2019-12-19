using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTabInteractivity : MonoBehaviour
{
    public int tab;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update()
    {
        toggle.interactable = WizardController.instance.selectedTab > tab;
    }
}
