using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_GatherFirewood : GOAP_Action
{
    public override void Awake()
    {
        base.Awake();
        actionID = "GatherWood";
        cost = 13f;
        AddSatisfyWorldState(WorldStateKey.bHasWood, true);
    }

    public override bool CheckProceduralConditions()
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Run(GOAP_Agent agent)
    {
        Debug.Log("performing: " + actionID);
    }
}