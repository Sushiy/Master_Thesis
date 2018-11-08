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
        requiredSkill = new GOAP_Skill("woodCutting", 1);
    }

    public override bool CheckProceduralConditions()
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Run(GOAP_Agent agent)
    {
        Debug.Log("performing: " + actionID);
    }
}
