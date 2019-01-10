using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterWindow : EditorWindow
{
    GOAP_Character character;

    Skills newSkill = Skills.WoodCutting;
    int newSkillLevel = 0;

    public static void ShowWindow(GOAP_Character character)
    {
        CharacterWindow window  = (CharacterWindow)EditorWindow.GetWindow(typeof(CharacterWindow));
        window.character = character;
    }
    private void OnGUI()
    {
        GUILayout.Label("Character Name", EditorStyles.boldLabel);
        character.characterName = EditorGUILayout.TextField("Text Field", character.characterName);

        GUILayout.Label("Add New Skill", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        newSkill = (Skills)EditorGUILayout.EnumPopup("SkillID", newSkill);
        newSkillLevel = (int)EditorGUILayout.Slider("Slider", newSkillLevel, 0, 5);
        EditorGUILayout.EndHorizontal();
        if(GUILayout.Button("Add Skill"))
        {
            character.AddSkill(newSkill,newSkillLevel);
            newSkill = Skills.WoodCutting;
            newSkillLevel = 0;
        }

        GUILayout.Label("Skills", EditorStyles.boldLabel);

        if (character.skills == null) character.skills = new List<GOAP_Skill>();
        foreach (GOAP_Skill skill in character.skills)
        {
            GUILayout.Label(skill.id + "|" + skill.level);
        }

    }
}
