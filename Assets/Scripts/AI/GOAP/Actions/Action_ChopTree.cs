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
        requiredSkill = new GOAP_Skill(Skills.WoodCutting, 2);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        //When should he really check for the conditions?
        return true;
        /*
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.chopTreeLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
        if (target != null)
            return true;
        else
            return false;
            */
    }

    public override bool RequiresInRange()
    {
        return false;
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
                Debug.Log("<color=#cc0000>" + agent.Character.characterName + "s Axe broke.</color>");
                agent.Character.UpdateInventory(ItemType.Axe, false);
            }
            CompletePerform(agent);
        }
        return completed;
    }
}
