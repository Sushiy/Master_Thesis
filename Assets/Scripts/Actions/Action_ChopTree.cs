﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChopTree : GOAP_Action
{
    public override void Awake()
    {
        base.Awake();
        actionID = "ChopTree";
        workCost = 4f;
        AddRequiredWorldState(WorldStateKey.bHasAxe, true);
        AddSatisfyWorldState(WorldStateKey.bHasLog, true);
        requiredSkill = new GOAP_Skill(Skills.WoodCutting, 2);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.LOCATIONS.CHOPTREE, agent.transform.position).gameObject; //TODO: This isnt technically correct
        if (target)
            return true;
        else
            return false;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool Run(GOAP_Agent agent)
    {
        Debug.Log("performing: " + actionID);
        return true;
    }
}
