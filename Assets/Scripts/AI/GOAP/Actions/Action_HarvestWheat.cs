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
        workCost = 10f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.IronHoe);
        AddRequiredWorldState(WorldStateKey.bIsWheatRipe, true);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Wheat);
        BenefitingSkill = Skills.Farming;
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
                agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + " could not harvest wheat, as it is not ripe.</color>");
                agent.Replan();
                return true;
            }
        }
        UpdateWorkTime(deltaTime);

        if (completed)
        {
            if (Random.value < 0.3f)
            {
                agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Hoe broke.</color>");
                agent.Character.UpdateInventory(ItemType.IronHoe, false);
            }
            agent.Character.UpdateInventory(ItemType.Wheat, true, 4);
            CompletePerform(agent);
            field.Harvest();
        }
        return completed;
    }
}
