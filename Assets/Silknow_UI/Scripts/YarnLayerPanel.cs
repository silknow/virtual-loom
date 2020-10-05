using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Honeti;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YarnLayerPanel : MonoBehaviour
{
   
    public string yarnNumber
    {
        get => yarnText.GetComponent<I18NText>().getParam(0);
        set => yarnText.GetComponent<I18NText>().updateParam( value,0);
    }
    public Color outputColor
    {
        get => outputColorImage.color;
        set => outputColorImage.color = value;
    }

    public bool isVisible
    {
        get => visibilityToggle.isOn;
        set => visibilityToggle.isOn = value;
    }
    
    [SerializeField]
    private Text yarnText;
    [SerializeField]
    private Image outputColorImage;
    [SerializeField]
    private Toggle visibilityToggle;

    public YarnEntity yarnEntity;


    public void OnEnable()
    {
        I18N.OnLanguageChanged += _onLanguageChanged;
    }
    public void OnDisable()
    {
        I18N.OnLanguageChanged -= _onLanguageChanged;
    }
    
    private void _onLanguageChanged(LanguageCode newLang)
    {
    }
    public void ActiveToggle(bool active)
    {
        visibilityToggle.interactable = active;
    }

    public void OnToggleVisibility(bool isOn)
    {
        //HIDE/Show Layer in patch
        if(yarnEntity.geometryIndex!=-1)
            WizardController.instance.instantiatedPatch.setYarnActive(yarnEntity.geometryIndex, isOn);
        else
        {
            print("geometry Index -1");
        }
    }

    public void OnPointerClick(BaseEventData eventData)
    {
        var pointerData = eventData as PointerEventData;
        if ( pointerData != null && pointerData.clickCount== 2)
        {
            print("doble click on " +yarnNumber);
        }
    }
    
}
