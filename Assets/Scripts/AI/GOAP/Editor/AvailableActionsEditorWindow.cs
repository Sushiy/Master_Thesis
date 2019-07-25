using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class AvailableActionsEditorWindow : EditorWindow
{
    GOAP_Character.CharacterData characterData;

    string[] allActions;

    public static void ShowWindow(ref GOAP_Character.CharacterData characterData)
    {
        AvailableActionsEditorWindow window  = (AvailableActionsEditorWindow)EditorWindow.GetWindow(typeof(AvailableActionsEditorWindow));
        window.characterData = characterData;

        //Add all actions to the list
        window.allActions = GOAP_Action.GetAllActionNames();
        characterData.InitBaseActions(window.allActions);
        characterData.RemoveWrongActions(window.allActions);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label(characterData.characterName);
        for(int i = 0; i < allActions.Length; i++)
        {
            string action = allActions[i];
            if (GOAP_Action.baseActions.Contains(action))
            {
                continue;
            }
            bool contained = characterData.availableActions.Contains(action);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(action);
            if (GUILayout.Button(contained.ToString(), GUILayout.MaxWidth(100)))
            {
                if (!contained)
                {
                    characterData.availableActions.Add(action);
                }
                else
                {
                    characterData.availableActions.Remove(action);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

    }
}
