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
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Pickaxe);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.mineIronLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
        if (target != null)
            return true;
        else
            return false;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool Perform(GOAP_Agent agent, float deltaTime)
    {
        StartPerform(agent);
        UpdateWorkTime(deltaTime);

        if(completed)
        {
            agent.Character.UpdateInventory(ItemType.Iron, true, 2);
            if(Random.value < 0.1f)
            {
                agent.Character.UpdateInventory(ItemType.Pickaxe, false);
            }
        }
        return completed;
    }
}
