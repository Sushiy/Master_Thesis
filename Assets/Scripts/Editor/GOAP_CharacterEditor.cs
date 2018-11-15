using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GOAP_Character))]
public class GOAP_CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Edit Character"))
        {
            EditorGUILayout.PrefixLabel("Allowed Actions");
            CharacterWindow.ShowWindow((GOAP_Character)this.target);
        }
    }
}
