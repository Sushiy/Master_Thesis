using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakeHoe : GOAP_Action
{
    public Action_MakeHoe()
    {
        Init();
        actionID = "MakeHoe";
        workCost = 2f;
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.IronHoe);
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

        if (completed)
        {
            agent.Character.UpdateInventory(ItemType.Wood, false);
            agent.Character.UpdateInventory(ItemType.Iron, false);
            agent.Character.UpdateInventory(ItemType.IronHoe, true);
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}
