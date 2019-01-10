using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GOAP_QuestBoardWindow : EditorWindow
{
    [MenuItem("Window/GOAP/QuestBoard")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GOAP_QuestBoardWindow));
    }

    private void OnGUI()
    {
        GUILayout.Label("Open Quests", EditorStyles.boldLabel);
        if (GOAP_QuestBoard.instance == null) return;
        for (int i = 0; i < GOAP_QuestBoard.instance.quests.Count; i++)
        {
            if (GUILayout.Button(GOAP_QuestBoard.instance.quests[i].ToString()))
            {
                GOAP_QuestBoard.instance.CompleteQuest(i);
            }
        }
    }

    public void OnInspectorUpdate()
    {
        // This will only get called 10 times per second.
        Repaint();
    }
}
