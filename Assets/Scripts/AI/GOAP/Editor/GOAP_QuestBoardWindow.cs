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
        foreach(KeyValuePair<int, GOAP_Quest> quest in GOAP_QuestBoard.instance.quests)
        {
            if (GUILayout.Button(quest.ToString()))
            {
                GOAP_QuestBoard.instance.CompleteQuest(quest.Key);
            }
        }
    }

    public void OnInspectorUpdate()
    {
        // This will only get called 10 times per second.
        Repaint();
    }
}
