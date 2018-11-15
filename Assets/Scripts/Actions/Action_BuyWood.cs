using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_BuyWood : GOAP_Action
{
    public Action_BuyWood()
    {
        Init();
        workCost = 10f;
        coinCost = 10f;
        actionID = "BuyWood";
        AddSatisfyWorldState(WorldStateKey.bHasWood, true);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool Run(GOAP_Agent agent)
    {
        Debug.Log("performing: " + actionID);
        return true;
    }
}