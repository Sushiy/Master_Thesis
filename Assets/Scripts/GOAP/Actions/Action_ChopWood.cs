﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChopWood : GOAP_Action
{
    public Action_ChopWood()
    {
        Init();
        actionID = "ChopWood";
        workCost = 2f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemIds.Axe);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemIds.Log);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemIds.Wood);
        requiredSkill = new GOAP_Skill(Skills.WoodCutting, 1);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.LOCATIONS.WOODWORKSHOP, agent.View.GetPosition()); //TODO: This isnt technically correct
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

        agent.Character.UpdateInventory(ItemIds.Log, false);
        agent.Character.UpdateInventory(ItemIds.Wood, true, 4);

        if (agent.ConsumeWorldState(ItemIds.Axe, 0.1f))
        {
            Debug.Log("<color=#cc0000>" +agent.Character.characterName + "s Axe broke.</color>");
            agent.Character.UpdateInventory(ItemIds.Axe, false);
        }
        return true;
    }
}
