using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChopTree : GOAP_Action
{
    Tree_GOAT tree;
    public Action_ChopTree()
    {
        Init();
        actionID = "ChopTree";

        VariationData ironAxe = new VariationData();
        ironAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.IronAxe);
        ironAxe.AddRequiredWorldState(WorldStateKey.bIsTreeAvailable, 1);
        ironAxe.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Log);
        ironAxe.benefitingSkill = Skills.WoodCutting;
        ironAxe.workCost = 4f;
        ironAxe.range = 1f;
        variations.Add(ironAxe);

        VariationData stoneAxe = new VariationData();
        stoneAxe.AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.StoneAxe);
        stoneAxe.AddRequiredWorldState(WorldStateKey.bIsTreeAvailable, 1);
        stoneAxe.AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Log);
        stoneAxe.benefitingSkill = Skills.WoodCutting;
        stoneAxe.workCost = 12f;
        stoneAxe.range = 1f;
        variations.Add(stoneAxe);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.chopTreeLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
        if (isStartingWork)
        {
            tree = (Tree_GOAT)InfoBlackBoard.instance.FindClosestAvailable(InfoBlackBoard.instance.chopTreeLocations, agent.View.GetPosition());
            if(tree == null)
            {
                agent.ChangeCurrentWorldState(new GOAP_Worldstate(WorldStateKey.bIsTreeAvailable, false));
                agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + " could not chop a tree, as none has grown enough.</color>");
                agent.CancelPlan();
            }
        }
        UpdateWorkTime(deltaTime);

        if(completed)
        {
            agent.Character.UpdateInventory(ItemType.Log, true);
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
        Action_ChopTree action = new Action_ChopTree();
        action.SetVariationData(i);
        return action;
    }
}
