using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_BuyIron : GOAP_Action
{
    public Action_BuyIron()
    {
        Init();
        workCost = 10f;
        coinCost = 10f;
        actionID = "BuyIron";
        AddSatisfyWorldState(WorldStateKey.bHasIron, true);
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
