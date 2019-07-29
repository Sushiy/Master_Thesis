using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_GatherDeadwood : GOAP_Action
{
    public Action_GatherDeadwood()
    {
        Init();
        actionID = "GatherDeadwood";
        workCost = 12f;
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.gatherWoodLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            agent.Character.UpdateInventory(ItemType.Wood, true, 3);
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}