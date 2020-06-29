using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ARTEC.Curves;
[CustomEditor(typeof(Yarn))]
public class YarnEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Yarn yarn = (Yarn)target;
        if (GUILayout.Button("Update"))
        {
            yarn.UpdateMesh();
        }
    }
}
