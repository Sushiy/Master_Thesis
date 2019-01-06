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
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Iron);
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
        UpdateWorkTime(deltaTime);

        if(completed)
        {
            agent.Character.UpdateInventory(ItemType.Iron, true, 2);
        }
        return completed;
    }
}
