using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GOAP_Planner))]
public class GOAP_PlannerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ((GOAP_Planner)target).plannableActions = (PlannableActions)EditorGUILayout.EnumFlagsField(((GOAP_Planner)target).plannableActions);
    }
}
