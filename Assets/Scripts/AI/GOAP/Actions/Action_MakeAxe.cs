using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_MakeAxe : GOAP_Action
{
    public Action_MakeAxe()
    {
        Init();
        actionID = "MakeAxe";


        VariationData ironAxe = new VariationData();
        ironAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Iron);
        ironAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
        ironAxe.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.IronAxe);
        ironAxe.benefitingSkill = Skills.Smithing;
        ironAxe.workCost = 1f;
        ironAxe.range = 1f;
        variations.Add(ironAxe);

        VariationData stoneAxe = new VariationData();
        stoneAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Stone);
        stoneAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
        stoneAxe.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.StoneAxe);
        stoneAxe.workCost = 4f;
        stoneAxe.range = 1f;
        variations.Add(stoneAxe);
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

        if(completed)
        {
            if(variationIndex == 0)
            {
                agent.Character.UpdateInventory(ItemType.Wood, false);
                agent.Character.UpdateInventory(ItemType.Iron, false);
                agent.Character.UpdateInventory(ItemType.IronAxe, true);
            }
            else
            {
                agent.Character.UpdateInventory(ItemType.Wood, false);
                agent.Character.UpdateInventory(ItemType.Stone, false);
                agent.Character.UpdateInventory(ItemType.StoneAxe, true);
            }
            CompletePerform(agent);
        }
        return completed;
    }

    public override GOAP_Action GetVariation(int i)
    {

        Action_MakeAxe action = new Action_MakeAxe();
        action.SetVariationData(i);
        return action;
    }
}
