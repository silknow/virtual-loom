using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AnalyticsMonitor : MonoBehaviour
{
    public int mouseMovement;

    public int leftClicks;

    public int rightClicks;
    
    public int middleClicks;

    public GeneralTechnique technique
    {
        get => WizardController.instance._generalTechnique;
    }
    private Vector3 _lastMousePosition;
    void Start()
    {
        
        _lastMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) leftClicks++;
        if (Input.GetMouseButtonUp(1)) rightClicks++;
        if (Input.GetMouseButtonUp(2)) middleClicks++;

        Vector3 incremento = Input.mousePosition - _lastMousePosition;
        mouseMovement += (int)(Mathf.Abs(incremento[0])+Mathf.Abs(incremento[1]));
        _lastMousePosition = Input.mousePosition;
    }

    public void clear()
    {
        leftClicks = rightClicks = mouseMovement = middleClicks = 0;
    }
}
