using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(STLGenerator))]
    public class STLGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var p = (STLGenerator)target;
            if (GUILayout.Button("Generate and Save"))
            {            
                p.GenerateGeometry();
            }
        }
    }
}