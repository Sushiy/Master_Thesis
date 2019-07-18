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
        if(character.agent != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Current Worldstate", EditorStyles.boldLabel);
            foreach (GOAP_Worldstate state in character.agent.currentWorldstates)
            {
                GUILayout.Label(state.ToString());
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Current PostedQuestIDs", EditorStyles.boldLabel);
            foreach (int id in character.agent.postedQuestIDs)
            {
                GUILayout.Label(id.ToString());
            }
            EditorGUILayout.LabelField("Current CompletedQuestIDs", EditorStyles.boldLabel);
            foreach (int id in character.agent.completedQuestIDs)
            {
                GUILayout.Label(id.ToString());
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Edit Character"))
        {
            CharacterWindow.ShowWindow((GOAP_Character)this.target);
        }
    }
}
