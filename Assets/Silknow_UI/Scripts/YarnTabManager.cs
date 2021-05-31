using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Honeti;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class YarnTabManager : MonoBehaviour
{


    public GameObject prefabYarnPanel;
    public RectTransform parentBackground;
    public RectTransform parentPictorical;

    public List<YarnPanel> backgroundYarns;
    public List<YarnPanel> pictorialYarns;

    public Button generate3DButton;
    public Button generateSVG;

    public RawImage imageGenerated;

    public bool yarnsReady = false;

    public GameObject bindingWarpToggle;
    
    private void OnEnable()
    {
        WizardController.instance.yarnTabManager = this;
        generateSVG.interactable = generate3DButton.interactable = false;
        
        backgroundYarns.Clear();
        pictorialYarns.Clear();
        yarnsReady = false;
        foreach (RectTransform child in parentBackground) {
            if(child.GetComponent<YarnPanel>().yarnZone != YarnPanel.YarnZone.Warp)
                GameObject.Destroy(child.gameObject);
            else
            {
                
                child.GetComponent<YarnPanel>().parentManager = this;
                backgroundYarns.Add(child.GetComponent<YarnPanel>());
                if (WizardController.instance._generalTechnique == GeneralTechnique.Freestyle)
                {
                    child.GetComponent<YarnPanel>().yarnTypeDropdown.interactable = true;
                    child.GetComponent<YarnPanel>().outputColorImage.GetComponent<Button>().interactable = true;
                }

                child.GetComponent<YarnPanel>().UpdateYarnTypes();
            }
        }
        foreach (RectTransform child in parentPictorical) {
            GameObject.Destroy(child.gameObject);
        }

        // Jesús: set the labels matrix texture
        setLabelsMatrix();

        WizardController.instance.yarnPanels.Clear();
        WizardController.instance.yarnEntities.Clear();
        
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
            
            panel.GetComponent<YarnPanel>().yarnZone = YarnPanel.YarnZone.Pictorial;
            pictorialYarns.Add(panel.GetComponent<YarnPanel>());
            WizardController.instance.yarnPanels.Add(panel);
            panel.GetComponent<YarnPanel>().UpdateYarnTypes();
            
            var yarnEntity = new YarnEntity(WizardController.instance.clusterList[i],i,panel);
            WizardController.instance.yarnEntities.Add(yarnEntity);
        }
        
        
        /*for (int i = 0; i < WizardController.instance.clusterList.Count; i++)
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
            } #1#
            
            WizardController.instance.yarnPanels.Add(panel);
            panel.GetComponent<YarnPanel>().UpdateYarnTypes();
        }*/

        yarnsReady = true;
        Activate3DButton();

        bindingWarpToggle.GetComponent<Toggle>().isOn = false;
        bindingWarpToggle.SetActive(WizardController.instance.selectedTechniqueRestrictions.allowedBindingWarp);

        
        //Pablo: Prueba pa siempre
        updateYarnColors();
        
    }

    public void Activate3DButton()
    {
        if(WizardController.instance._generalTechnique !=GeneralTechnique.SpolinedDamask)
            generateSVG.interactable = generate3DButton.interactable = backgroundYarns.Count >= 2;
        else
            generateSVG.interactable = generate3DButton.interactable = backgroundYarns.Count >= 3;
    }

    public void GenerateOutputImage()
    {
        if(!yarnsReady)
            return;
        // Jesús: set new colors
        updateYarnColors();
    }

    public void ToggleBindingWarp(bool value)
    {
        WizardController.instance.bindingWarp = value;
    }

    // Jesús: method to apply the labelsMatrix to the image
    private void setLabelsMatrix() {

        int w = WizardController.instance.posterizeResult.width;
        int h = WizardController.instance.posterizeResult.height;
        // Create a texture with only 1 channel (8-bit) to store the label of each pixel 
        var tex = new Texture2D(w,h,TextureFormat.RGB24,false,false);
        tex.filterMode = FilterMode.Point;
        
        int rows = 0;
        Color[] pixels = new Color[w*h]; 
        for(int x = 0; x < h; x++) {
            for(int y = 0; y < w; y++) {
                int label = (int)WizardController.instance.labelsMatrix.get(rows, 0)[0];
                pixels[rows] = new Color(label/255.0f,0,0,1);;
                rows++;
            }
        }
        tex.SetPixels(0,0,w,h,pixels);
        tex.Apply();
        imageGenerated.texture = tex;
        Resources.UnloadUnusedAssets();
    }

    private void updateYarnColors() {
        var colors = new Color[30];
        for (int i=0;i<WizardController.instance.yarnPanels.Count;i++)
        {
            if (WizardController.instance.yarnPanels[i].yarnZone == YarnPanel.YarnZone.Weft &&
                WizardController.instance.selectedTechniqueRestrictions.uniformBackground)
                colors[i] = WizardController.instance.warpPanel.outputColor;
            else
                colors[i] = WizardController.instance.yarnPanels[i].GetComponent<YarnPanel>().outputColor;
        }
        imageGenerated.material.SetColorArray("_Colors", colors);
    }
    public void HighlightSelectedYarn(YarnPanel selectedPanel, bool selected)
    {
        if(!selected)
            imageGenerated.material.SetInt("_SelectedYarn", -1);
        else
        {
            int label = WizardController.instance.yarnPanels.IndexOf(selectedPanel);
            imageGenerated.material.SetInt("_SelectedYarn", label);
            imageGenerated.material.SetFloat("_SelectedTime", Time.time);

        }
        
    }
}
