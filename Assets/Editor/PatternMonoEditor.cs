using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;

[CustomEditor(typeof(PatternMono))]
public class PatternMonoEditor : UnityEditor.Editor
{
    public Pattern p;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        /*var p = (PatternMono)target;
        if (GUILayout.Button("Reduce Pattern"))
        {            
            p.p.reducePattern(p.divider,p.gapAspect,true);
            OpenCVForUnity.UnityUtils.Utils.matToTexture2D(p.p.pattern,p.orig.texture);
            OpenCVForUnity.UnityUtils.Utils.matToTexture2D(p.p.reduced_pattern,p.target.texture);
        }*/
    }
    
}
