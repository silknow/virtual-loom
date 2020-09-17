using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

public enum GeneralTechnique
{
    Damask = 0,
    Spolined= 1,
    SpolinedDamask = 2,
    Lampas = 3,
    Brocade = 4,
    Freestyle = 5,
    Other = 6
}


public class WizardController : Singleton<WizardController>
{
    public int selectedTab = 0;
    public Texture2D homographyResult;
    public Texture2D posterizeResult;
    public Texture2D inputTexture;

    public YarnPanel selectedYarnPanel;
    
    public YarnPanel warpPanel;
    public List<YarnPanel> yarnPanels;
    
    public List<Mat> imageProcessingResults;
    public Mat backgroundWhiteTexture;

    public int clusterCount = 2;

    [SerializeField]
    private Dropdown generalTechniqueDropdown;

    public ColorPicker colorPicker;

    [SerializeField] private ToggleTabInteractivity []tabs;
    [SerializeField] private GameObject wizardWindow;
    [SerializeField] private GameObject colorPickerWindow;
    [SerializeField] private GameObject weaveTechniqueWindow;
    [SerializeField] private GameObject visualizationWindow;
    [SerializeField] private GameObject processingWindow;
    [SerializeField] private GameObject helpWindow;
    [SerializeField] private GameObject settingsWindow;

    [SerializeField] private ToggleGroup qualityToggleGroup;
    [SerializeField]
    private Dropdown clustersDropdown;
    [HideInInspector]
    public GeneralTechnique _generalTechnique;

    public Weave selectedWeave;
    

    public List<Color> clusterList;
    
    public ScriptableYarn []yarnTypes;

    public GameObject patchPrefab;


    public Patch instantiatedPatch;

    public List<WeavingTechniqueRestrictions> techniqueRestrictions;

    public WeavingTechniqueRestrictions selectedTechniqueRestrictions;

    public Mat labelsMatrix;

    public bool bindingWarp = false;

    public YarnTabManager yarnTabManager;
    
    public GameObject freestyleOptions;

    [SerializeField]
    private bool brocadedFreestyle;

    private bool _visualizationState = false;

    private void Awake()
    {
        clusterList = new List<Color>();
        imageProcessingResults = new List<Mat>();
        yarnPanels = new List<YarnPanel>();
        
      
        techniqueRestrictions = Resources.LoadAll<WeavingTechniqueRestrictions>("WeavingTechniques").ToList();
    
        selectedTechniqueRestrictions= techniqueRestrictions.First(t => t.technique == GeneralTechnique.Damask).Clone();
        selectedWeave = selectedTechniqueRestrictions.defaultWeave;
        labelsMatrix = new Mat();

    }

    

    public void OnGeneralTechniqueChange(Dropdown dropDown)
    {
        _generalTechnique  = (GeneralTechnique)dropDown.value;
        Debug.Log(_generalTechnique);
        
        clustersDropdown.ClearOptions();
        var opt = new List<string>();
        switch (_generalTechnique)
        {
            case GeneralTechnique.Damask:
                opt.Clear();
                opt.Add("2");
                break;
            case GeneralTechnique.Spolined:
                opt.Clear();
                opt = Enumerable.Range(1, 25).Select(n => n.ToString()).ToList();
                break;
            case GeneralTechnique.SpolinedDamask:
                opt.Clear();
                opt = Enumerable.Range(3, 25).Select(n => n.ToString()).ToList();
                break;
            case GeneralTechnique.Brocade:
                opt.Clear();
                opt.Add("2");
                break;
            case GeneralTechnique.Lampas:
                opt.Clear();
                opt.Add("2");
                break;
            case GeneralTechnique.Freestyle:
                opt.Clear();
                opt = Enumerable.Range(1, 16).Select(n => n.ToString()).ToList();
                break;
            
        }
        clustersDropdown.AddOptions(opt);
        
        clusterCount = Convert.ToInt32(clustersDropdown.options[clustersDropdown.value].text);
        
        selectedTechniqueRestrictions= techniqueRestrictions.First(t => t.technique == _generalTechnique).Clone();
        selectedWeave = selectedTechniqueRestrictions.defaultWeave;
        
        freestyleOptions.SetActive(_generalTechnique == GeneralTechnique.Freestyle);
        if (_generalTechnique == GeneralTechnique.Freestyle)
        {
            foreach (var toggle in freestyleOptions.GetComponentsInChildren<Toggle>())
            {
                toggle.isOn = false;
                brocadedFreestyle = false;
            }
            selectedTechniqueRestrictions.backgroundWeftCount = 1;
        }
        
        
        
    }
    
    public void OnClustersChange(Dropdown dropDown)
    {
        clusterCount = Convert.ToInt32(dropDown.options[dropDown.value].text);
       
    }

    public void ChangeColorSelectedYarn(Color newColor)
    {
        selectedYarnPanel.outputColor = newColor;
        
        if(selectedYarnPanel.yarnZone != YarnPanel.YarnZone.Warp)
            selectedYarnPanel.parentManager.GenerateOutputImage();
        
    }

    public void ToggleColorPickerWindow()
    {
        if (!colorPickerWindow.activeInHierarchy)
        {
            colorPicker.AssignColor(selectedYarnPanel.outputColor);
        }
        else
        {
            selectedYarnPanel.UpdateColor();
        }
        colorPickerWindow.SetActive(!colorPickerWindow.activeInHierarchy);
    }
    
    public void ToggleWeaveWindow()
    {
        weaveTechniqueWindow.SetActive(!weaveTechniqueWindow.activeInHierarchy);
    }
    public void ToggleHelpWindow()
        {
            helpWindow.SetActive(!helpWindow.activeInHierarchy);
        }
    
    public void ToggleSetttingsWindow()
    {
        settingsWindow.SetActive(!settingsWindow.activeInHierarchy);
        if (settingsWindow.activeInHierarchy)
        {
            if (!PlayerPrefs.HasKey("Quality"))
                PlayerPrefs.SetInt("Quality",1);
            int quality = PlayerPrefs.GetInt("Quality");
            qualityToggleGroup.GetComponentsInChildren<Toggle>()[quality].SetIsOnWithoutNotify(true);
        }
    }

    public void ToggleVisualizationWindow()
    {
        visualizationWindow.SetActive(!visualizationWindow.activeInHierarchy);
    }
    
    public void ToggleWizardWindow()
    {
        wizardWindow.SetActive(!wizardWindow.activeInHierarchy);
    }
    
    public void TogglePatchActive(bool active)
    {
        if(instantiatedPatch)
            instantiatedPatch.gameObject.SetActive(active);
    }

    public void SaveAllPosterizedImages()
    {
        //MANOLO: Comentado al pasar todas las Textures2D a Mat en el procesado de imágenes.
        /*byte[] bytesPosterized = posterizeResult.EncodeToPNG ();
        File.WriteAllBytes (Application.persistentDataPath + "/"+_generalTechnique +"_posterized.png", bytesPosterized);
        
        var cont = 0;
        foreach (var texture in imageProcessingResults)
        {
            var fileNamePersistentData = Application.persistentDataPath + "/"+_generalTechnique +"_"+ cont+".png";
            byte[] bytes = texture.EncodeToPNG ();
            File.WriteAllBytes (fileNamePersistentData, bytes);
            cont++;
        }*/
    }

    public void GenerateSTLPatch(string path)
    {
        //Cambiar Ruta - File Dialog
        if (instantiatedPatch)
        {
            instantiatedPatch.export_STL(path);
        }

    }


    IEnumerator WeavePatch(float delayTime)
    {
        
        yield return new WaitForSeconds(delayTime);
        if (instantiatedPatch == null)
        {
            var go  = Instantiate(patchPrefab, Vector3.zero, Quaternion.identity);
            instantiatedPatch = go.GetComponent<Patch>();
        }

        var patch = instantiatedPatch;
        //Reset Patch Parent Rotation
        instantiatedPatch.transform.parent.rotation = Quaternion.identity;
        instantiatedPatch.transform.parent.GetComponent<RotatePatch>().targetRotation = 0;

        //Background Pattern


        var backgroundWeft = yarnTabManager.backgroundYarns.First(by => by.yarnZone == YarnPanel.YarnZone.Weft);
        
        var indexOfBackground = yarnPanels.IndexOf(yarnPanels.First(y => y.GetComponent<YarnPanel>() == backgroundWeft));


        if (selectedTechniqueRestrictions.uniformBackground)
        {
            patch.backgroundPattern.pattern = backgroundWhiteTexture;
        }
        else
        {
            patch.backgroundPattern.pattern = imageProcessingResults[indexOfBackground];
        }
        
        
        patch.warpYarn = warpPanel.GetScriptableYarn();
      
        if(_generalTechnique == GeneralTechnique.Damask) 
            patch.weftYarn = yarnPanels[yarnPanels.IndexOf(yarnPanels.First(y => y.GetComponent<YarnPanel>().yarnZone == YarnPanel.YarnZone.Pictorial ))].GetComponent<YarnPanel>().GetScriptableYarn();
        else if (!selectedTechniqueRestrictions.uniformBackground)
        {
            var pictorialBg = yarnTabManager.backgroundYarns.FindLast(by => by.yarnZone == YarnPanel.YarnZone.Weft);
        
            var indexOfPictorialBg = yarnPanels.IndexOf(yarnPanels.First(y => y.GetComponent<YarnPanel>() == pictorialBg));
            
            patch.weftYarn = yarnPanels[indexOfPictorialBg].GetComponent<YarnPanel>().GetScriptableYarn();
        }
        else 
            patch.weftYarn = yarnPanels[indexOfBackground].GetComponent<YarnPanel>().GetScriptableYarn();
        patch.technique.pattern = new Mat(selectedWeave.weavePattern.texture.width,selectedWeave.weavePattern.texture.height,CvType.CV_8U);
        Utils.texture2DToMat(selectedWeave.weavePattern.texture, patch.technique.pattern);
        
        patch.pictorials.Clear();
        for (var i=0;i<imageProcessingResults.Count;i++)
        {
            if(_generalTechnique == GeneralTechnique.Damask || yarnPanels[i].GetComponent<YarnPanel>().yarnZone !=YarnPanel.YarnZone.Pictorial)
                continue;
            var tex = imageProcessingResults[i];

            var picto = new Pictorial();
            picto.drawing = new Pattern();
            picto.drawing.pattern = tex;
            

            picto.yarn = yarnPanels[i].GetComponent<YarnPanel>().GetScriptableYarn();
            if (bindingWarp)
            {
                picto.healedStep = 6;
                picto.healedStepGap = 2;
                
            }
            else if(_generalTechnique == GeneralTechnique.Lampas || _generalTechnique == GeneralTechnique.Brocade)
            {
                picto.healedStep = 6;
                picto.healedStepGap = 2;
                picto.doubleHealed = true;
            }
            else
            {
                picto.healedStep = -1;
            }

            //Revisar Freestyle Brocaded
            picto.adjusted = _generalTechnique == GeneralTechnique.Spolined ||
                             _generalTechnique == GeneralTechnique.SpolinedDamask ||
                             (_generalTechnique == GeneralTechnique.Freestyle && brocadedFreestyle);
            
            
            patch.pictorials.Add(picto);
        }
        
        //patch.divider = -1;
        patch.divider = -1f;

        
        patch.Weave();
        
        TogglePatchActive(true);
        ToggleProcessingWindow();
        SetVisualizationState(true);
        //visualizationWindow.SetActive(true);
    }
    
    public void Generate3DPatch()
    {
        ToggleProcessingWindow();
        //wizardWindow.SetActive(false);
        StartCoroutine(WeavePatch(0.0f));
    }

    public void Update3DVisualization()
    {
        if (instantiatedPatch)
        {
            ToggleProcessingWindow();
            StartCoroutine(UpdatePatch(0.5f));
        }
    }
    
    IEnumerator UpdatePatch(float delayTime)
    {
        instantiatedPatch.CleanAll();
        yield return new WaitForSeconds(delayTime);
        instantiatedPatch.Weave();
        ToggleProcessingWindow();
    }
    public Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;
        
        Debug.Log("w: " + w + "; h: "+h);
 
        int iRotated = 0;
        int iOriginal = 0;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }
 
        Texture2D rotatedTexture = new Texture2D(h, w);
        Debug.Log("Rotated Texture w: " + rotatedTexture.width + "; h: "+rotatedTexture.height);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        Debug.Log("Rotated Texture After Apply w: " + rotatedTexture.width + "; h: "+rotatedTexture.height);
        return rotatedTexture;
    }


    public void ToggleBrocadedFreestyle(bool value)
    {
        brocadedFreestyle = value;
    }
    public void ToggleDamasseGroundFreestyle(bool value)
    {
        if (selectedTechniqueRestrictions.technique == GeneralTechnique.Freestyle)
        {
            selectedTechniqueRestrictions.uniformBackground = !value;

            if (value)
                selectedTechniqueRestrictions.backgroundWeftCount = 2;
            else
                selectedTechniqueRestrictions.backgroundWeftCount = 1;
        }
         
    }

    public void ToggleProcessingWindow()
    {
        processingWindow.SetActive(!processingWindow.activeInHierarchy);
    }

    public  void LoadFromJson(VLConfig cfg)
    {
        StartCoroutine(jsonReader.GetText(cfg.imgUri,TextureLoaded));
        tabs[0].gameObject.SetActive(false);
    }

    private void TextureLoaded(Texture2D texture)
    {
        //Pasamos la textura
        inputTexture = texture;
        //Activamos el tab de homografia
        tabs[1].GetComponent<Toggle>().isOn = true;
       
    }

    public void setQuality(int qualityLevel)
    {
        
        if(qualityLevel != PlayerPrefs.GetInt("Quality"))
        {
            print("Change Quality Level to "+qualityLevel);
            PlayerPrefs.SetInt("Quality", qualityLevel);
            if(_visualizationState)
                Update3DVisualization();
        }
        qualityToggleGroup.GetComponentsInChildren<Toggle>()[qualityLevel].SetIsOnWithoutNotify(true);
        print("Set Quality Level to "+qualityLevel);
        /*
        if (toggle.isOn)
        {
            string name = toggle.name;
            print("hola, soy "+name+ " y estaba encendido");
            var qualityLevel = 1;
            if (name.Contains("HIGH"))
                qualityLevel = 2;
            else if (name.Contains("MED"))
                qualityLevel = 1;
            else if (name.Contains("LOW"))
                qualityLevel = 0;
            if (qualityLevel != PlayerPrefs.GetInt("Quality"))
            {
                PlayerPrefs.SetInt("Quality", qualityLevel);
                if(_visualizationState)
                    Update3DVisualization();
            }
        }
        else{
            toggle.SetIsOnWithoutNotify(true);
            print("hola, soy "+toggle.name+ " y estaba apagado");
        }
        */
       

    }

    public bool GetVisualizationState()
    {
        return _visualizationState;
    }

    public void SetVisualizationState(bool active)
    {
        _visualizationState = active;
        wizardWindow.GetComponent<Image>().enabled = !_visualizationState;
    }
}
