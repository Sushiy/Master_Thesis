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
        workCost = 4f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.IronAxe);
        AddRequiredWorldState(WorldStateKey.bIsTreeAvailable, 1);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Log);
        BenefitingSkill = Skills.WoodCutting;
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
            if(Random.value < 0.2f)
            {
                agent.Character.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Axe broke.</color>");
                agent.Character.UpdateInventory(ItemType.IronAxe, false);
            }
            CompletePerform(agent);
        }
        return completed;
    }
}
