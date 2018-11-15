using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricAgentView : MonoBehaviour, IGOAP_AgentView
{
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
        transform.position += (position - transform.position).normalized * 3.0f * Time.deltaTime;
    }
}
