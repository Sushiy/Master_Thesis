using System.Collections;
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
        transform.position += (position - transform.position).normalized * speed * Time.deltaTime;
    }

    public void PrintMessage(string message)
    {
        actionText.text = message;
    }

    public IActionTarget GetActionTargetSelf()
    {
        return selfActionTarget;
    }
}
