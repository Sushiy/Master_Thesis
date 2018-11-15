using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActionTarget : MonoBehaviour, IActionTarget
{
    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
