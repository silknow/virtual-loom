using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeavePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color selectedWeaveColor;
    public Color defaultColor = Color.white;
    public string name
    {
        get => weaveNameText.text;
        set => weaveNameText.text = value;
    }


   

    public Sprite image
    {
        get
        {
          return weaveImage.sprite;
        }
        set
        {
             weaveImage.sprite = value;
        }
    }

    [HideInInspector] public Weave weavingTechnique;
    
    [SerializeField]
    private Text weaveNameText;
    [SerializeField]
    private Image weaveImage;
    

    public void OnSelectWeave()
    {
        WizardController.instance.selectedWeave = weavingTechnique;
        GetComponent<Image>().color = selectedWeaveColor;
        WizardController.instance.ToggleWeaveWindow();
    }
    public void SetSelectedColor()
    {
        GetComponent<Image>().color = selectedWeaveColor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        WizardController.instance.weaveDetailWindow.SetActive(true);
        WizardController.instance.weaveDetailWindow.GetComponent<WeaveDetail>().set(weaveImage.GetComponent<Image>().sprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WizardController.instance.weaveDetailWindow.SetActive(false);
    }

    public void OnDisable()
    {
        WizardController.instance.weaveDetailWindow.SetActive(false);
    }
}
