using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(GOAP_Character))]
public class GOAP_CharacterEditor : Editor
{

    Vector2 actionScrollPos;

    GOAP_Character character;

    public void OnEnable()
    {
        character = (GOAP_Character)target;
        character.characterData.InitBaseActions(GOAP_Action.GetAllActionNames());
        character.characterData.RemoveWrongActions(GOAP_Action.GetAllActionNames());
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (character.agent != null)
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
    }
}
