using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_GetWater : GOAP_Action 
{

	public Action_GetWater()
    {
        Init();
        actionID = "GetWater";
        workCost = 2f;
        range = 2f;
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Water);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.getWaterLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}
