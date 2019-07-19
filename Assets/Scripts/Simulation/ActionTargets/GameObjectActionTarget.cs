using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActionTarget : MonoBehaviour, IActionTarget
{
    public virtual bool IsAvailable()
    {
        return true;
    }

    public virtual Vector3 GetPosition()
    {
        return transform.position;
    }

    public virtual void Call(Vector3 callerPosition)
    {

    }
}
