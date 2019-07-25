using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakeAxe : GOAP_Action
{
    public Action_MakeAxe()
    {
        Init();
        actionID = "MakeAxe";
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Axe);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Iron);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
        BenefitingSkill = Skills.Smithing;
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.smithingWorkshopLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            agent.Character.UpdateInventory(ItemType.Wood, false);
            agent.Character.UpdateInventory(ItemType.Iron, false);
            agent.Character.UpdateInventory(ItemType.Axe, true);
            CompletePerform(agent);
        }
        return completed;
    }
}
