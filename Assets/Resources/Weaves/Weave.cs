using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaveTec", menuName = "Virtual Loom/Weave", order = 2)]
public class Weave : ScriptableObject
{
    public List<GeneralTechnique> techniques;

    public enum WeavingTechniqueType
    {
        Satin,
        Tabby,
        Twill
    }

    public WeavingTechniqueType type;
    [FormerlySerializedAs("techniqueImage")] public Sprite weavePattern;

    [FormerlySerializedAs("techniqueName")] public string weaveName;
}