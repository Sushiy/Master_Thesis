using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakePickaxe : GOAP_Action
{
    public Action_MakePickaxe()
    {
        Init();
        actionID = "MakePickaxe";

        VariationData ironPickaxe = new VariationData();
        ironPickaxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Iron);
        ironPickaxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
        ironPickaxe.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.IronPickaxe);
        ironPickaxe.benefitingSkill = Skills.Smithing;
        ironPickaxe.workCost = 2f;
        ironPickaxe.range = 1f;
        variations.Add(ironPickaxe);

        VariationData stonePickaxe = new VariationData();
        stonePickaxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Stone);
        stonePickaxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
        stonePickaxe.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.StonePickaxe);
        stonePickaxe.workCost = 8f;
        stonePickaxe.range = 1f;
        variations.Add(stonePickaxe);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.smithingWorkshopLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            if (variationIndex == 0)
            {
                agent.Character.UpdateInventory(ItemType.Wood, false);
                agent.Character.UpdateInventory(ItemType.Iron, false);
                agent.Character.UpdateInventory(ItemType.IronPickaxe, true);
            }
            else
            {
                agent.Character.UpdateInventory(ItemType.Wood, false);
                agent.Character.UpdateInventory(ItemType.Stone, false);
                agent.Character.UpdateInventory(ItemType.StonePickaxe, true);
            }
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {

        Action_MakePickaxe action = new Action_MakePickaxe();
        action.SetVariationData(i);
        return action;
    }
}