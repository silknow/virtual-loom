using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SplitMeshRenderer))]
public class SplitMeshRendererEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SplitMeshRenderer smr = target as SplitMeshRenderer;
        if (GUILayout.Button("Split"))
        {
            smr.Split();
        }
    }
}
