using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_BakeBread : GOAP_Action
{
    public Action_BakeBread()
    {
        Init();
        actionID = "BakeBread";
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemIds.Flour);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemIds.Water);
        requiredSkill = new GOAP_Skill(Skills.Baking, 4);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool Run(GOAP_Agent agent)
    {
        Debug.Log("performing: " + actionID);
        agent.RemoveCurrentWorldState(new GOAP_Worldstate(WorldStateKey.eHasItem, (int)ItemIds.Flour));
        agent.RemoveCurrentWorldState(new GOAP_Worldstate(WorldStateKey.eHasItem, (int)ItemIds.Water));
        return true;
    }
}
