using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_TendToField : GOAP_Action
{
    Field_GOAT field;
    public Action_TendToField()
    {
        Init();
        actionID = "TendToField";
        workCost = 20f;
        AddRequiredWorldState(WorldStateKey.eHasItem, (int)ItemType.Hoe);
        AddRequiredWorldState(WorldStateKey.bWasFieldTended, false);
        AddRequiredWorldState(WorldStateKey.bIsWheatRipe, false);
        AddSatisfyWorldState(WorldStateKey.bWasFieldTended, true);
        AddSatisfyWorldState(WorldStateKey.bIsWheatRipe, true);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.farmingLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            field = (Field_GOAT)target;
            if (field.IsAlreadyTendedTo)
            {
                agent.ChangeCurrentWorldState(new GOAP_Worldstate(WorldStateKey.bWasFieldTended, true, field));
                Debug.Log("<color=#cc0000>" + agent.Character.characterData.characterName + " could not tend to the field.</color>");
                agent.Replan();
                return true;
            }
        }
        UpdateWorkTime(deltaTime);

        if (completed)
        {
            if (Random.value < 0.3f)
            {
                Debug.Log("<color=#cc0000>" + agent.Character.characterData.characterName + "s Hoe broke.</color>");
                agent.Character.UpdateInventory(ItemType.Hoe, false);
            }
            Debug.Log("<color=#cc00cc>" + agent.Character.characterData.characterName + "</color> hopes that the field is now ripe");
            CompletePerform(agent);
            field.TendToField();
        }
        return completed;
    }
}