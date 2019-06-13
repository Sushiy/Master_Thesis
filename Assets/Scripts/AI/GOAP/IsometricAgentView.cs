﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricAgentView : MonoBehaviour, IGOAP_AgentView
{
    public TMPro.TextMeshProUGUI actionText;
    public float speed = 3.0f;
    private GameObjectActionTarget selfActionTarget;

    private void Awake()
    {
        selfActionTarget = GetComponent<GameObjectActionTarget>();    
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool IsInRange(Vector3 position, float range)
    {
        return Vector3.Distance(GetPosition(), position) <= range;
    }

    public void MoveTo(Vector3 position)
    {
        Vector3 targetVector = (position - transform.position);
        targetVector.y = 0;
        transform.position += targetVector.normalized * speed * Time.deltaTime;
        if(targetVector.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(-targetVector);
        }
    }

    public void PrintMessage(string message)
    {
        actionText.text = message;
    }

    public IActionTarget GetActionTargetSelf()
    {
        return selfActionTarget;
    }

    public void VisualizeAction(GOAP_Action action)
    {
        if(action == null)
        {
            //Idle
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        if(action.ActionID == "Sleep")
        {
            transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, 0);
        }
    }
}