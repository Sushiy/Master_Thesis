using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_BuyItem : GOAP_Action
{
    //this can only buy resources no crafted items or such!
    ItemType wantedItem;

    public Action_BuyItem()
    {
        Init();
        workCost = 10f;
        coinCost = 10f;
        actionID = "BuyItem";
    }

    public Action_BuyItem(ItemType wantedItem) : this()
    {
        SetWantedItem(wantedItem);
    }

    public override bool CheckProceduralConditions(GOAP_Agent agent)
    {
        target = InfoBlackBoard.instance.FindClosest(InfoBlackBoard.instance.buyResourceLocations, agent.View.GetPosition()); //TODO: This isnt technically correct
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
            agent.Character.UpdateInventory(wantedItem, true);
            CompletePerform(agent);
        }
        return completed;
    }

    public void SetWantedItem(ItemType item)
    {
        wantedItem = item;
        actionID = "BuyItem: " + wantedItem.ToString();

        AddSatisfyWorldState(WorldStateKey.eHasItem, (int)wantedItem);
    }
}

