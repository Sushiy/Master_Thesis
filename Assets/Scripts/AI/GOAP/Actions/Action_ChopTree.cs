using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChopTree : GOAP_Action
{
    public Action_ChopTree()
    {
        Init();
        actionID = "ChopTree";
        workCost = 4f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Axe);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Log);
        BenefitingSkill = Skills.WoodCutting;
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosestAvailable(InfoBlackBoard.instance.chopTreeLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            agent.Character.UpdateInventory(ItemType.Log, true);
            if(Random.value < 0.2f)
            {
                Debug.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Axe broke.</color>");
                agent.Character.UpdateInventory(ItemType.Axe, false);
            }
            CompletePerform(agent);
        }
        return completed;
    }
}
