using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_ChopWood : GOAP_Action
{
    public Action_ChopWood()
    {
        Init();
        actionID = "ChopWood";
        workCost = 2f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Axe);
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Log);
        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)ItemType.Wood);
        requiredSkill = new GOAP_Skill(Skills.WoodCutting, 1);
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

            if (Random.value < 0.1f)
            {
                Debug.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Axe broke.</color>");
                agent.Character.UpdateInventory(ItemType.Axe, false);
            }
            CompletePerform(agent);
        }
        return completed;
    }
}
