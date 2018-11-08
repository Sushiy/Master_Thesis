using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WhiteBoardWindow : EditorWindow
{

    string newSkill = "";
    int newSkillLevel = 0;

    [MenuItem("Window/GOAP/WhiteBoardWindow")]
    public static void ShowWindow()
    {
        WhiteBoardWindow window = (WhiteBoardWindow)EditorWindow.GetWindow(typeof(WhiteBoardWindow));
    }
    private void OnGUI()
    {
        GUILayout.Label("Open Quests", EditorStyles.boldLabel);
        if (GOAP_WhiteBoard.instance == null) return;
        for (int i = 0; i < GOAP_WhiteBoard.instance.quests.Count; i++)
        {
            if (GUILayout.Button(GOAP_WhiteBoard.instance.quests[i].ToString()))
            {
                GOAP_WhiteBoard.instance.CompleteQuest(i);
            }
        }
    }
}
