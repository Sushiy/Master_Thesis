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
        GOAP_Character character = (GOAP_Character)target;
        EditorGUILayout.PrefixLabel("Allowed Actions");
        character.availableActions = (PlannableActions)EditorGUILayout.EnumFlagsField(((GOAP_Character)target).availableActions);
        EditorGUILayout.PrefixLabel("Current Worldstate", EditorStyles.boldLabel);
        if(character.agent != null)
        {
            GUILayout.BeginVertical();
            foreach (GOAP_Worldstate state in character.agent.currentWorldstates)
            {
                GUILayout.Label(state.ToString());
            }
            GUILayout.EndVertical();
        }
        if (GUILayout.Button("Edit Character"))
        {
            CharacterWindow.ShowWindow((GOAP_Character)this.target);
        }
    }
}
