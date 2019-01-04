using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_GatherFirewood : GOAP_Action
{
    public Action_GatherFirewood()
    {
        Init();
        actionID = "GatherWood";
        workCost = 40f;
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemIds.Wood);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool Perform(GOAP_Agent agent)
    {
        BasePerform(agent);
        agent.Character.UpdateInventory(ItemIds.Wood, true, 3);
        return true;
    }
}