using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MineIron : GOAP_Action
{
    public Action_MineIron()
    {
        Init();
        actionID = "MineIron";

        VariationData ironPick = new VariationData();
        ironPick.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.IronPickaxe);
        ironPick.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Iron);
        ironPick.workCost = 5f;
        ironPick.range = 1f;
        variations.Add(ironPick);

        VariationData stonePick = new VariationData();
        stonePick.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.StoneAxe);
        stonePick.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Iron);
        stonePick.workCost = 15f;
        stonePick.range = 1f;
        variations.Add(stonePick);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.mineIronLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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

        if(completed)
        {
            agent.Character.UpdateInventory(ItemType.Iron, true, 2);
            if(Random.value < (variationIndex == 0 ? 0.1f : 0.4f))
            {
                agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Pickaxe broke.</color>");
                if(variationIndex == 0)
                {
                    agent.Character.UpdateInventory(ItemType.IronPickaxe, false);
                }
                else
                {
                    agent.Character.UpdateInventory(ItemType.StoneAxe, false);
                }
            }
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {
        Action_MineIron action = new Action_MineIron();
        action.SetVariationData(i);
        return action;
    }
}
