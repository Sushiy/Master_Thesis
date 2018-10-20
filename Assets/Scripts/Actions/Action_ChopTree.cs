﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChopTree : GOAP_Action
{
    public override void Awake()
    {
        base.Awake();
        actionID = "ChopTree";
        cost = 2f;
        AddRequiredWorldState(WorldStateKey.bHasAxe, true);
        AddSatisfyWorldState(WorldStateKey.bHasLog, true);
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
