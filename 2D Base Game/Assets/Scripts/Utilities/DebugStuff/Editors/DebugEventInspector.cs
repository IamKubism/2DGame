using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebugEvent))]
public class DebugEventInspector : Editor
{ 
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Raise"))
        {
            ((DebugEvent)target).Raise();
        }
        //base.OnInspectorGUI();
    }
}
