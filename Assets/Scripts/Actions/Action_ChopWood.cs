using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChopWood : GOAP_Action
{
    public override void Awake()
    {
        base.Awake();
        actionID = "ChopWood";
        workCost = 2f;
        AddRequiredWorldState(WorldStateKey.bHasAxe, true);
        AddRequiredWorldState(WorldStateKey.bHasLog, true);
        AddSatisfyWorldState(WorldStateKey.bHasWood, true);
        requiredSkill = new GOAP_Skill(Skills.WoodCutting, 1);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.LOCATIONS.WOODWORKSHOP, agent.transform.position).gameObject; //TODO: This isnt technically correct
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
