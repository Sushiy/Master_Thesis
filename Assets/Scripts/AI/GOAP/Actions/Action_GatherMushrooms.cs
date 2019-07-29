using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_GatherMushrooms : GOAP_Action
{
    public Action_GatherMushrooms()
    {
        Init();
        actionID = "GatherMushrooms";
        workCost = 16f;
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Food);
        AddRequiredWorldState(WorldStateKey.bIsMushroomAvailable, true);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.gatherMushroomsLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
        if (isStartingWork)
        {
            target.IsAvailable();
            agent.ChangeCurrentWorldState(new GOAP_Worldstate(WorldStateKey.bIsMushroomAvailable, false));
            agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + " could not gather mushrooms, as there aren't enough.</color>");
            agent.CancelPlan();
        }
        UpdateWorkTime(deltaTime);

        if (completed)
        {
            agent.Character.UpdateInventory(ItemType.Food, true, 1);
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}
