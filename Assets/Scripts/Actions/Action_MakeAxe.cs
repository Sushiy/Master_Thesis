using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakeAxe : GOAP_Action
{
    public override void Awake()
    {
        base.Awake();
        actionID = "MakeAxe";
        AddSatisfyWorldState(WorldStateKey.bHasAxe, true);
        AddRequiredWorldState(WorldStateKey.bHasIron, true);
        requiredSkill = new GOAP_Skill(Skills.Smithing, 3);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override bool Run(GOAP_Agent agent)
    {
        Debug.Log("performing: " + actionID);
        return true;
    }
}
