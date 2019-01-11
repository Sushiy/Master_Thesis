using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakeBread : GOAP_Action
{
    public Action_MakeBread()
    {
        Init();
        actionID = "MakeBread";
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Flour);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Water);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Bread);
        requiredSkill = new GOAP_Skill(Skills.Baking, 3);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool Perform(GOAP_Agent agent, float deltaTime)
    {
        StartPerform(agent);
        UpdateWorkTime(deltaTime);

        if(completed)
        {
            agent.Character.UpdateInventory(ItemType.Flour, false);
            agent.Character.UpdateInventory(ItemType.Water, false);
            agent.Character.UpdateInventory(ItemType.Bread, true);
            CompletePerform(agent);
        }
        return completed;
    }
}
