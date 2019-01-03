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
        EditorGUILayout.PrefixLabel("Allowed Actions");
        ((GOAP_Character)target).availableActions = (PlannableActions)EditorGUILayout.EnumFlagsField(((GOAP_Character)target).availableActions);
        if (GUILayout.Button("Edit Character"))
        {
            CharacterWindow.ShowWindow((GOAP_Character)this.target);
        }
    }
}
