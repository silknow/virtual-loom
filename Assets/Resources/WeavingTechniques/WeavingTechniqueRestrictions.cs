using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeavingTechniqueRestriction", menuName = "Virtual Loom/Weaving Technique Restrictions", order = 3)]
public class WeavingTechniqueRestrictions : ScriptableObject
{
    public GeneralTechnique technique;

    public Weave defaultWeave;

    public List<ScriptableYarn> warpYarns;
    
    public List<ScriptableYarn> weftYarns;
    
    public List<ScriptableYarn> pictorialYarns;

    public bool pictorialZone;

    public int backgroundWeftCount = 1;

    public bool allowedBindingWarp = false;

    public bool uniformBackground = false;
    
    
    
    
    public WeavingTechniqueRestrictions Clone()
    {
        var result=ScriptableObject.CreateInstance<WeavingTechniqueRestrictions>();
        result.technique = technique;
        result.defaultWeave = defaultWeave;
        result.warpYarns = warpYarns;
        result.weftYarns = weftYarns;
        result.pictorialYarns = pictorialYarns;
        result.pictorialZone = pictorialZone;
        result.backgroundWeftCount = backgroundWeftCount;
        result.allowedBindingWarp = allowedBindingWarp;
        result.uniformBackground = uniformBackground;
        return result;
    }
}