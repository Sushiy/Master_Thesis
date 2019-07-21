using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_HarvestWheat : GOAP_Action
{
    Field_GOAT field;
    public Action_HarvestWheat()
    {
        Init();
        actionID = "HarvestWheat";
        workCost = 30f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Hoe);
        AddRequiredWorldState(WorldStateKey.bIsWheatRipe, true);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Wheat);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.farmingLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
        if(isStartingWork)
        {
            field = (Field_GOAT)target;
            if(!field.IsAvailable())
            {
                agent.ChangeCurrentWorldState(new GOAP_Worldstate(WorldStateKey.bIsWheatRipe, false, field));
                agent.ChangeCurrentWorldState(new GOAP_Worldstate(WorldStateKey.bWasFieldTended, field.IsAlreadyTendedTo, field));
                Debug.Log("<color=#cc0000>" + agent.Character.characterName + "s Hoe broke.</color>");
                agent.Replan();
                return false;
            }
        }
        UpdateWorkTime(deltaTime);

        if (completed)
        {
            if (Random.value < 0.3f)
            {
                Debug.Log("<color=#cc0000>" + agent.Character.characterName + "s Hoe broke.</color>");
                agent.Character.UpdateInventory(ItemType.Hoe, false);
            }
            agent.Character.UpdateInventory(ItemType.Wheat, true, 4);
            CompletePerform(agent);
        }
        return completed;
    }
}
