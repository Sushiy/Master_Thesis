using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Fish : GOAP_Action
{
    public Action_Fish()
    {
        Init();
        actionID = "Fish";
        workCost = 20f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.FishingRod);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Food);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {

        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.getWaterLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            agent.Character.UpdateInventory(ItemType.Food, true, 3);
            if (Random.value < 0.05f)
            {
                agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Fishing rod broke.</color>");
                agent.Character.UpdateInventory(ItemType.FishingRod, false);
            }
            CompletePerform(agent);
        }
        return completed;
    }
}
