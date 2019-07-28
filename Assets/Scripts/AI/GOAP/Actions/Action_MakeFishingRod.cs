using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakeFishingRod : GOAP_Action
{
    public Action_MakeFishingRod()
    {
        Init();
        actionID = "MakeFishingRod";
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.FishingRod);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.woodWorkshopLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            agent.Character.UpdateInventory(ItemType.Wood, false);
            agent.Character.UpdateInventory(ItemType.FishingRod, true);
            CompletePerform(agent);
        }
        return completed;
    }
}