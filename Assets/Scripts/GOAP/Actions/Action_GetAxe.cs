using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_GetAxe : GOAP_Action
{
    public Action_GetAxe()
    {
        Init();
        actionID = "GetAxe";
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Axe);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool Perform(GOAP_Agent agent, float deltaTime)
    {
        StartPerform(agent);
        return true;
    }
}
