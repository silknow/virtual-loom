using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTabInteractivity : MonoBehaviour
{
    public int tab;
    public TabContentManager tcm;
        
    private Toggle toggle;
    
    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void OnDisable()
    {
        tcm.gameObject.SetActive(false);
    }

   
    // Update is called once per frame
    void Update()
    {
        toggle.interactable = WizardController.instance.selectedTab > tab;
    }
}
