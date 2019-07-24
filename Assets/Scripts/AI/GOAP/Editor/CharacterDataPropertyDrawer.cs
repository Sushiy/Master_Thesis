using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GOAP_Character.CharacterData))]
public class CharacterDataPropertyDrawer : PropertyDrawer
{
    GOAP_Character.CharacterData characterData;
    int index = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);
        
        object obj = fieldInfo.GetValue(property.serializedObject.targetObject);
        characterData = obj as GOAP_Character.CharacterData;
        if (obj.GetType().IsArray || (obj is IList && obj.GetType().IsGenericType))
        {
            index = Convert.ToInt32(new string(property.propertyPath.Where(c => char.IsDigit(c)).ToArray()));
            if(index >= 0)
            {
                if (obj.GetType().IsArray)
                {
                    characterData = ((GOAP_Character.CharacterData[])obj)[index];
                }
                else
                {
                    characterData = ((List<GOAP_Character.CharacterData>)obj)[index];
                }

            }
        }


        if (property.isExpanded && characterData != null)
        {
            GUI.Label(new Rect(position.xMin, position.yMax - 20f * (characterData.availableActions.Count + 2), position.width - 30f, 20f), "Available Actions:");
            for (int i = 0; i < characterData.availableActions.Count; i++)
            {
                GUI.Label(new Rect(position.xMin + 30f, position.yMax - 20f * (characterData.availableActions.Count-i + 1), position.width - 30f, 20f), characterData.availableActions[i]);
            }
            if (GUI.Button(new Rect(position.xMin + 30f, position.yMax - 20, position.width - 30f, 20f), "Edit Available Actions"))
            {
                AvailableActionsEditorWindow.ShowWindow(ref characterData, index);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded && characterData != null && characterData.availableActions != null)
            return EditorGUI.GetPropertyHeight(property) + 20f * (characterData.availableActions.Count + 2);
        return EditorGUI.GetPropertyHeight(property);
    }
}
