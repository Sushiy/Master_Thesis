using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionTarget
{
    //TODO: Add other needed stuff here: Animations?
    bool IsAvailable();
    Vector3 GetPosition();
}
