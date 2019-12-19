using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Patch))]
public class PatchEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var p = (Patch)target;
        if (GUILayout.Button("Update"))
        {            
            p.Weave();
        }
    }
}