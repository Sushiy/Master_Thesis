using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_EatFood : GOAP_Action
{
    public Action_EatFood()
    {
        Init();
        actionID = "EatFood";
        workCost = 5f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Food);
        AddSatisfyWorldState(WorldStateKey.bHasEaten, true);
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

        if (completed)
        {
            agent.Character.UpdateInventory(ItemType.Food, false);
            agent.Character.Eat();
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}