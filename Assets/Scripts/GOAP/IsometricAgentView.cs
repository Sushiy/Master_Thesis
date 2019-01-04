using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricAgentView : MonoBehaviour, IGOAP_AgentView
{
    public TMPro.TextMeshProUGUI actionText;
    private bool readyToAct = true;
    public float timeWarpFactor = 0.1f;
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
        transform.position += (position - transform.position).normalized * 3.0f * Time.deltaTime;
    }

    public void PrintMessage(string message, float time)
    {
        actionText.text = message;
        if (time > 0)
            StartCoroutine(DelayAct(time));
    }

    IEnumerator DelayAct(float time)
    {
        readyToAct = false;
        yield return new WaitForSeconds(time * timeWarpFactor);
        readyToAct = true;
    }

    public bool IsReadyToAct()
    {
        return readyToAct;
    }

    public IActionTarget GetActionTargetSelf()
    {
        return selfActionTarget;
    }
}
