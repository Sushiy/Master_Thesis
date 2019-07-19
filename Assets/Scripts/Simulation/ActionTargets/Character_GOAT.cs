using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_GOAT : GameObjectActionTarget
{
    GOAP_Agent agent;

    private void Start()
    {
        agent = GetComponent<GOAP_Character>().agent;
    }

    public override void Call(Vector3 callerPosition)
    {
        agent.Called(callerPosition);
    }
}
