﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_BuyIron : GOAP_Action
{
    public override void Awake()
    {
        base.Awake();
        workCost = 10f;
        coinCost = 10f;
        actionID = "BuyIron";
        AddSatisfyWorldState(WorldStateKey.bHasIron, true);
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