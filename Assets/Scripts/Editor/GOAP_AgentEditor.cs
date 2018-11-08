using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GOAP_Character))]
public class GOAP_AgentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Edit Character"))
        {
            CharacterWindow.ShowWindow((GOAP_Character)this.target);
        }
    }
}
