using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGOAP_AgentView
{
    Vector3 GetPosition();
    bool IsInRange(Vector3 position, float range);
    void MoveTo(Vector3 position);
    void TurnTo(Vector3 position);
    void PrintMessage(string message);
    void VisualizeAction(GOAP_Action action);
    IActionTarget GetActionTargetSelf();
}
