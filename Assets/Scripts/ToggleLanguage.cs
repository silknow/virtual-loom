using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLanguage : MonoBehaviour
{
    public string[] langs;
    private Dropdown _dropdown;
    private Toggle[] languageTabs;
    public void Awake()
    {
        languageTabs = GetComponentsInChildren<Toggle>();
        languageTabs[(int)I18N.instance.gameLang].SetIsOnWithoutNotify(true);
        #if UNITY_WEBGL
        GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        #endif
    }
    
    public void setLanguage(int i)
    {
        I18N.instance.setLanguage((LanguageCode)i);
        languageTabs[(int)I18N.instance.gameLang].SetIsOnWithoutNotify(true);
    }
    
    public void setLanguage(string langCode)
    {
        setLanguage((int)(LanguageCode)System.Enum.Parse(typeof(LanguageCode), langCode));
    }


}
