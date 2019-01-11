using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakeFlour : GOAP_Action 
{

	public Action_MakeFlour()
    {
        Init();
        actionID = "MakeFlour";
        workCost = 10f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Wheat);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Flour);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.millingLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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

        if (completed)
        {
            agent.Character.UpdateInventory(ItemType.Wheat, false);
            agent.Character.UpdateInventory(ItemType.Flour, true, 4);
            CompletePerform(agent);
        }
        return completed;
    }
}
