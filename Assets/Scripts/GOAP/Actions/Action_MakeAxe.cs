using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakeAxe : GOAP_Action
{
    public Action_MakeAxe()
    {
        Init();
        actionID = "MakeAxe";
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemIds.Axe);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemIds.Iron);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemIds.Wood);
        requiredSkill = new GOAP_Skill(Skills.Smithing, 3);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.LOCATIONS.SMITHWORKSHOP, agent.View.GetPosition()); //TODO: This isnt technically correct
        if (target != null)
            return true;
        else
            return false;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool Perform(GOAP_Agent agent)
    {
        BasePerform(agent);
        agent.Character.UpdateInventory(ItemIds.Wood, false);
        agent.Character.UpdateInventory(ItemIds.Iron, false);
        agent.Character.UpdateInventory(ItemIds.Axe, true);
        return true;
    }
}
