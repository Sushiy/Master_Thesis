using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChopWood : GOAP_Action
{
    public Action_ChopWood()
    {
        Init();
        actionID = "ChopWood";

        VariationData ironAxe = new VariationData();
        ironAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.IronAxe);
        ironAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Log);
        ironAxe.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
        ironAxe.benefitingSkill = Skills.WoodCutting;
        ironAxe.workCost = 1f;
        ironAxe.range = 1f;
        variations.Add(ironAxe);

        VariationData stoneAxe = new VariationData();
        stoneAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.StoneAxe);
        stoneAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Log);
        stoneAxe.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
        stoneAxe.benefitingSkill = Skills.WoodCutting;
        stoneAxe.workCost = 3f;
        stoneAxe.range = 1f;
        variations.Add(stoneAxe);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.woodWorkshopLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            agent.Character.UpdateInventory(ItemType.Log, false);
            agent.Character.UpdateInventory(ItemType.Wood, true, 4);

            if (Random.value < (variationIndex == 0 ? 0.2f : 0.5f))
            {
                agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Axe broke.</color>");
                if (variationIndex == 0)
                {
                    agent.Character.UpdateInventory(ItemType.IronAxe, false);
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
        Action_ChopWood action = new Action_ChopWood();
        action.SetVariationData(i);
        return action;
    }
}
