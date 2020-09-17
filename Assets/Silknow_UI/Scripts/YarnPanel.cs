using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Honeti;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YarnPanel : MonoBehaviour, IPointerEnterHandler
{
    public enum YarnZone
    {
        Warp,
        Weft,
        Pictorial
    }

    public YarnZone yarnZone = YarnZone.Pictorial;
    public ScriptableYarn defaultWarpYarn;
    
    public string yarnNumber
    {
        get => yarnText.GetComponent<I18NText>().getParam(0);
        set => yarnText.GetComponent<I18NText>().updateParam( value,0);
    }
    
    public List<Dropdown.OptionData> yarnOptions
    {
        get => yarnTypeDropdown.options;
        set => yarnTypeDropdown.options = value;
    }

    public string yarnType
    {
        get { return yarnTypeDropdown.options[yarnTypeDropdown.value].text; }
        set
        {
            var newSelected = yarnTypeDropdown.options.Single(s => s.text == value);
            yarnTypeDropdown.value = yarnTypeDropdown.options.IndexOf(newSelected);
        }
    }
    
    public Color inputColor
    {
        get => inputColorImage.color;
        set => inputColorImage.color = value;
    }
    public Color outputColor
    {
        get => outputColorImage.color;
        set => outputColorImage.color = value;
    }

    public bool isBackground
    {
        get => backgroundToggle.isOn;
        set => backgroundToggle.isOn = value;
    }



    [HideInInspector] public YarnTabManager parentManager;

    public Dropdown yarnTypeDropdown;
    [SerializeField]
    private Text yarnText;
    [SerializeField]
    private Image inputColorImage;
    [SerializeField]
    private Image outputColorImage;
    
   
    public Toggle backgroundToggle;


    public void OnEnable()
    {
        I18N.OnLanguageChanged += _onLanguageChanged;
    }
    public void OnDisable()
    {
        I18N.OnLanguageChanged -= _onLanguageChanged;
    }

    public ScriptableYarn GetScriptableYarn()
    {
        ScriptableYarn instance;
        if (yarnZone == YarnZone.Warp)
        {
            if(WizardController.instance._generalTechnique == GeneralTechnique.Freestyle)
                instance = WizardController.instance.selectedTechniqueRestrictions.warpYarns[yarnTypeDropdown.value].Clone() as ScriptableYarn;
            else
                instance = defaultWarpYarn.Clone() as ScriptableYarn;
            
        }
        else
        {
            if(yarnZone == YarnZone.Weft)
                instance = WizardController.instance.selectedTechniqueRestrictions.weftYarns[yarnTypeDropdown.value].Clone() as ScriptableYarn;
            else
                instance = WizardController.instance.selectedTechniqueRestrictions.pictorialYarns[yarnTypeDropdown.value].Clone() as ScriptableYarn;
                
            
        }
            
        instance.color = outputColor;
        return instance;
    }
    
    public RectTransform backgroundZone;
    public RectTransform pictoricalZone;
    
    
    private void _onLanguageChanged(LanguageCode newLang)
    {
        UpdateYarnTypes();
    }

    private void _updateTranslation()
    {
        /*if (_text)
        {
            if (!_isValidKey)
            {
                _key = _text.text;

                if (_key.StartsWith("^"))
                {
                    _isValidKey = true;
                }
            }

            _text.text = I18N.instance.getValue(_key, _params);
        }*/
    }
    public void OnChangeParent(bool value)
    {
        if (value && parentManager.backgroundYarns.Count > WizardController.instance.selectedTechniqueRestrictions.backgroundWeftCount)
        {
            isBackground = false;
            return;
        }

        if (value)
        {
            parentManager.pictorialYarns.Remove(this);
            parentManager.backgroundYarns.Add(this);
            yarnZone = YarnZone.Weft;
            if (!WizardController.instance.selectedTechniqueRestrictions.uniformBackground)
            {
                if (parentManager.backgroundYarns.Count == 2)
                {
                    yarnTypeDropdown.value = 0;
                    yarnTypeDropdown.RefreshShownValue();
                    yarnTypeDropdown.interactable = false;
                    WizardController.instance.warpPanel.outputColor = outputColor;
                    WizardController.instance.warpPanel.inputColor = inputColor;

                    if(WizardController.instance.selectedTechniqueRestrictions.backgroundWeftCount>1)
                        foreach (var yarnPanel in parentManager.pictorialYarns)
                        {
                            yarnPanel.backgroundToggle.GetComponentInChildren<Text>().text = "Pictorial Bg";
                        }
                }
            }
            else
            {
                if (parentManager.backgroundYarns.Count == 2)
                {
                    WizardController.instance.warpPanel.outputColor = outputColor;
                    WizardController.instance.warpPanel.inputColor = inputColor;
                }
            }
        }
        else
        {
            parentManager.pictorialYarns.Add(this);
            parentManager.backgroundYarns.Remove(this);
            yarnZone = YarnZone.Pictorial;
            yarnTypeDropdown.interactable = true;
            if (!WizardController.instance.selectedTechniqueRestrictions.uniformBackground)
            {
                if (parentManager.backgroundYarns.Count <= 2 && WizardController.instance.selectedTechniqueRestrictions.backgroundWeftCount>1)
                {
                    foreach (var yarnPanel in parentManager.pictorialYarns)
                    {
                        yarnPanel.backgroundToggle.GetComponentInChildren<Text>().text =
                            parentManager.backgroundYarns.Count == 2 ? "Pictorial Bg" : "Background";
                    }

                    foreach (var yarnPanel in parentManager.backgroundYarns)
                    {
                        if (yarnPanel.yarnZone == YarnZone.Weft)
                            yarnPanel.backgroundToggle.GetComponentInChildren<Text>().text = "Background";
                    }
                }
            }
        }
        GetComponent<RectTransform>().parent = value ? backgroundZone: pictoricalZone;
        UpdateYarnTypes();
        parentManager.Activate3DButton();
    }

    public void ActiveToggle(bool active)
    {
        backgroundToggle.interactable = active;
    }

    public void UpdateYarnTypes()
    {
        yarnTypeDropdown.ClearOptions();

        List<ScriptableYarn> yarnList = new List<ScriptableYarn>();

        switch (yarnZone)
        {
            case YarnZone.Warp:
                yarnList = WizardController.instance.selectedTechniqueRestrictions.warpYarns;
                break;
            case YarnZone.Weft:
                yarnList = WizardController.instance.selectedTechniqueRestrictions.weftYarns;
                break;
            case YarnZone.Pictorial:
                yarnList = WizardController.instance.selectedTechniqueRestrictions.pictorialYarns;
                break;
        }
        foreach (var yarnType in yarnList)
        {
            yarnType.translatedName = I18N.instance.getValue("^" + yarnType.name);
            yarnTypeDropdown.options.Add(new Dropdown.OptionData() {text=yarnType.translatedName});
        }
        yarnTypeDropdown.value = 0;
        OnChangeYarnType(0);
        yarnTypeDropdown.RefreshShownValue();
        
    }

    public void OnChangeYarnType(int value)
    {
        var sy = GetScriptableYarn();
        if (sy.isMetallic)
        {
            outputColor = sy.fixedColor;
            outputColorImage.GetComponent<Button>().interactable = false;
        }
        else if (WizardController.instance._generalTechnique == GeneralTechnique.Damask && yarnZone == YarnZone.Warp)
        {
            outputColorImage.GetComponent<Button>().interactable = false;
        }
        else
        {
            outputColorImage.GetComponent<Button>().interactable = true;
        }
        
        
        // Image Simulation
        if(yarnZone != YarnZone.Warp) 
            parentManager.GenerateOutputImage();
        
    }

    public void UpdateColor()
    {
        if ((WizardController.instance.selectedTechniqueRestrictions.technique == GeneralTechnique.Damask || WizardController.instance.selectedTechniqueRestrictions.technique == GeneralTechnique.SpolinedDamask) && yarnZone == YarnZone.Weft)
        {
            if (parentManager.backgroundYarns[1] == this)
            {
                WizardController.instance.warpPanel.outputColor = outputColor;
                WizardController.instance.warpPanel.inputColor = inputColor;
            }
        }

        if (yarnZone == YarnZone.Warp) return;
            parentManager.GenerateOutputImage();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (yarnZone == YarnZone.Warp) return;
        {
            parentManager.HighlightSelectedYarn(this, true);
        }
    }
}
