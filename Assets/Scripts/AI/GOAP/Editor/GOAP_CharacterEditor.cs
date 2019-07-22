using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;

[CustomEditor(typeof(GOAP_Character))]
public class GOAP_CharacterEditor : Editor
{
    bool showBaseActions = true;
    bool showAvailableActions = true;

    Vector2 actionScrollPos;

    GOAP_Character character;

    List<string> allActions;

    public void OnEnable()
    {
        character = (GOAP_Character)target;
        if (allActions == null)
        {
            allActions = new List<string>();

            System.Type[] types = typeof(GOAP_Action).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GOAP_Action))).ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                allActions.Add(types[i].ToString());
            }

            for(int i = character.availableActions.Count-1; i >= 0 ; i--)
            {
                string action = character.availableActions[i];
                if (!allActions.Contains(action) || action == "Action_CompleteQuest" || action == "Action_WaitForQuest" || action == "Action_PostQuest")
                {
                    character.availableActions.RemoveAt(i);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        showBaseActions = EditorGUILayout.Foldout(showBaseActions, "Base Actions", true);
        if (showBaseActions)
        {
            EditorGUILayout.BeginVertical();
            foreach (string action in GOAP_Action.baseActions)
            {
                GUILayout.Label(action);
                if(!character.availableActions.Contains(action) && action != "Action_CompleteQuest" && action != "Action_WaitForQuest" && action != "Action_PostQuest")
                    character.availableActions.Add(action);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();
        showAvailableActions = EditorGUILayout.Foldout(showAvailableActions, "Allowed Actions", true);
        if(showAvailableActions)
        {
            EditorGUILayout.BeginVertical();
            foreach(string Key in allActions)
            {
                if (GOAP_Action.baseActions.Contains(Key)) continue;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(Key);

                bool contained = character.availableActions.Contains(Key);
                if (GUILayout.Button(contained.ToString(), GUILayout.MaxWidth(100)))
                {
                    if (!contained)
                    {
                        character.availableActions.Add(Key);
                    }
                    else
                    {
                        character.availableActions.Remove(Key);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

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
        if (GUILayout.Button("Edit Character"))
        {
            CharacterWindow.ShowWindow((GOAP_Character)this.target);
        }
    }
}
