using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MineIron : GOAP_Action
{
    public Action_MineIron()
    {
        Init();
        actionID = "MineIron";
        workCost = 10f;
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemIds.Iron);
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
        agent.Character.UpdateInventory(ItemIds.Iron, true, 2);
        return true;
    }
}
