using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Honeti;
using UnityEngine;
using UnityEngine.UI;

public class YarnTabManager : MonoBehaviour
{


    public GameObject prefabYarnPanel;
    public RectTransform parentBackground;
    public RectTransform parentPictorical;

    public List<YarnPanel> backgroundYarns;
    public List<YarnPanel> pictorialYarns;

    public Button generate3DButton;

    public RawImage imageGenerated;

    public bool yarnsReady = false;

    public GameObject bindingWarpToggle;
    
    private void OnEnable()
    {
        WizardController.instance.yarnTabManager = this;
        generate3DButton.interactable = false;
        backgroundYarns.Clear();
        pictorialYarns.Clear();
        yarnsReady = false;
        foreach (RectTransform child in parentBackground) {
            if(child.GetComponent<YarnPanel>().yarnZone != YarnPanel.YarnZone.Warp)
                GameObject.Destroy(child.gameObject);
            else
            {
                backgroundYarns.Add(child.GetComponent<YarnPanel>());
                if(WizardController.instance._generalTechnique == GeneralTechnique.Freestyle)
                    child.GetComponent<YarnPanel>().yarnTypeDropdown.interactable = true;
                child.GetComponent<YarnPanel>().UpdateYarnTypes();
            }
        }
        foreach (RectTransform child in parentPictorical) {
            GameObject.Destroy(child.gameObject);
        }

        WizardController.instance.yarnPanels.Clear();
        for (int i = 0; i < WizardController.instance.clusterList.Count; i++)
        {
            var panel = GameObject.Instantiate(prefabYarnPanel, parentPictorical).GetComponent<YarnPanel>();
            
            panel.parentManager = this;
            panel.inputColor = WizardController.instance.clusterList[i];
            panel.outputColor = WizardController.instance.clusterList[i];
            panel.yarnNumber =""+ (i + 1);
            panel.isBackground = false;
            panel.backgroundZone = parentBackground;
            panel.pictoricalZone = parentPictorical;
            //if (WizardController.instance.selectedTechniqueRestrictions.pictorialZone)
            //{
                panel.GetComponent<YarnPanel>().yarnZone = YarnPanel.YarnZone.Pictorial;
                pictorialYarns.Add(panel.GetComponent<YarnPanel>());
            /*}
            else
            {
                
                panel.GetComponent<YarnPanel>().isBackground = true;
                panel.GetComponent<YarnPanel>().yarnZone = YarnPanel.YarnZone.Weft;
                panel.GetComponent<YarnPanel>().ActiveToggle(false);
                backgroundYarns.Add(panel.GetComponent<YarnPanel>());
            } */
            
            WizardController.instance.yarnPanels.Add(panel);
            panel.GetComponent<YarnPanel>().UpdateYarnTypes();
        }

        yarnsReady = true;
        Activate3DButton();

        bindingWarpToggle.GetComponent<Toggle>().isOn = false;
        bindingWarpToggle.SetActive(WizardController.instance.selectedTechniqueRestrictions.allowedBindingWarp);
    }

    public void Activate3DButton()
    {
        if(WizardController.instance._generalTechnique !=GeneralTechnique.SpolinedDamask)
            generate3DButton.interactable = backgroundYarns.Count >= 2;
        else
            generate3DButton.interactable = backgroundYarns.Count >= 3;
    }

    public void GenerateOutputImage()
    {
        if(!yarnsReady)
            return;
        var tex = new Texture2D(WizardController.instance.posterizeResult.width,WizardController.instance.posterizeResult.height,TextureFormat.RGB24,false,false);
        
        int rows = 0;
        for(int x = 0; x < tex.height; x++) {
            for(int y = 0; y < tex.width; y++) {
                int label = (int)WizardController.instance.labelsMatrix.get(rows, 0)[0];
                Color c = WizardController.instance.yarnPanels[label].GetComponent<YarnPanel>().outputColor;
                //dst.put(y, x, b,g,r);
                tex.SetPixel(y,x,new Color(c.r/1.0f,c.g/1.0f,c.b/1.0f));
                rows++;
            }
        }

        tex.Apply();
        imageGenerated.texture = tex;
    }

    public void ToggleBindingWarp(bool value)
    {
        WizardController.instance.bindingWarp = value;
    }
}
