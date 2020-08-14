using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutomaticVerticalScript))]
public class AutomaticVerticalScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Recalc Size"))
        {
            ((AutomaticVerticalScript)target).AdjustSize();
        }
        //base.OnInspectorGUI();
    }
}
