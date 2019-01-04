using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_BakeBread : GOAP_Action
{
    public Action_BakeBread()
    {
        Init();
        actionID = "BakeBread";
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemIds.Flour);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemIds.Water);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemIds.Bread);
        AddSatisfyWorldState(WorldStateKey.bHasWood, true);
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

    public override bool Perform(GOAP_Agent agent)
    {
        BasePerform(agent);

        agent.Character.UpdateInventory(ItemIds.Flour, false);
        agent.Character.UpdateInventory(ItemIds.Water, false);
        agent.Character.UpdateInventory(ItemIds.Bread, true);
        return true;
    }
}
