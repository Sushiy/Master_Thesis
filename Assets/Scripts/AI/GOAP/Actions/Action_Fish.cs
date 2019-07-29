using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Fish : GOAP_Action
{
    public Action_Fish()
    {
        Init();
        actionID = "Fish";
        workCost = 10f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.FishingRod);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Food);
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
            agent.Character.UpdateInventory(ItemType.Food, true, FishingSucess());
            if (Random.value < 0.05f)
            {
                agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Fishing rod broke.</color>");
                agent.Character.UpdateInventory(ItemType.FishingRod, false);
            }
            CompletePerform(agent);
        }
        return completed;
    }


    public int FishingSucess()
    {
        float f = Random.Range(0f, 1f);
        if (f < 0.1f) return 3;
        if (f < 0.3f) return 2;
        if (f < 0.7) return 1;
        return 0;
    }

    public override GOAP_Action GetVariation(int i)
    {
        throw new System.NotImplementedException();
    }
}
