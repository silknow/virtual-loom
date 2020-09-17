using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class VirtualLoomUI : MonoBehaviour
{
    public Patch patch;
    public Image damaskPattern;
    public Image weaveTechnique;
    public ScriptablePattern []patterns;
    public ScriptablePattern []damasks;

    public Dropdown weaveTechniqueDropdown;
    public Dropdown damaskDropdown;
    // Start is called before the first frame update
    public void Weave()
    {
        patch.Weave();
        
    }

    // Update is called once per frame
    void Start()
    {
        weaveTechniqueDropdown.captionImage.preserveAspect = true;
        damaskDropdown.captionImage.preserveAspect = true;
        List<Dropdown.OptionData> options=new List<Dropdown.OptionData>(patterns.Length);
        foreach (ScriptablePattern p in patterns)
        {
            options.Add(new Dropdown.OptionData(p.objectName,p.pattern));
        }
        weaveTechniqueDropdown.AddOptions(options);
        options=new List<Dropdown.OptionData>(damasks.Length);
        foreach (ScriptablePattern p in damasks)
        {
            options.Add(new Dropdown.OptionData(p.objectName,p.pattern));
        }
        damaskDropdown.AddOptions(options);
    }

    public void DropDownSelected()
    {
        OpenCVForUnity.UnityUtils.Utils.texture2DToMat(patterns[weaveTechniqueDropdown.value].pattern.texture, patch.technique.pattern);
        OpenCVForUnity.UnityUtils.Utils.texture2DToMat(patterns[damaskDropdown.value].pattern.texture, patch.backgroundPattern.pattern);
        
        patch.Weave();
    }
}
