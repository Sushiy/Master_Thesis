using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricAgentView : MonoBehaviour, IGOAP_AgentView
{
    public TextSizer actionText;
    public float speed = 3.0f;
    private GameObjectActionTarget selfActionTarget;

    private Vector3? moveToTarget;

    private void Awake()
    {
        selfActionTarget = GetComponent<GameObjectActionTarget>();    
    }

    private void Update()
    {
        Move();
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
        moveToTarget = position;
    }

    public void StopMove()
    {
        moveToTarget = null;
    }

    private void Move()
    {
        if(moveToTarget != null)
        {
            Vector3 targetVector = ((Vector3)moveToTarget - transform.position);
            targetVector.y = 0;
            transform.position += targetVector.normalized * speed * Time.deltaTime;
            if (targetVector.sqrMagnitude > 0)
            {
                transform.rotation = Quaternion.LookRotation(-targetVector);
            }
        }
    }

    public void PrintMessage(string message)
    {
        actionText.SetText(message);
    }

    public IActionTarget GetActionTargetSelf()
    {
        return selfActionTarget;
    }

    public void VisualizeAction(GOAP_Action action)
    {
        if(action == null)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else if(action.ActionID == "Sleep")
        {
            transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, 0);
        }
        else
        {
        }
    }

    public void TurnTo(Vector3 position)
    {
        transform.LookAt(position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
