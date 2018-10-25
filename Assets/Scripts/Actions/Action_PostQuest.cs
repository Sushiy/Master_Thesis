using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_PostQuest : GOAP_Action
{
    HashSet<GOAP_Worldstate> questStates;

    public override void Awake()
    {
        base.Awake();
        workCost = 10f;
        actionID = "PostQuest";
        keepOpen = true;
    }

    public int GetQuestCost(float estimatedCost)
    {
        return 5;
    }

    public override bool CheckProceduralConditions()
    {
        return true;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public void SetQuestStates(HashSet<GOAP_Worldstate> states )
    {
        questStates = new HashSet<GOAP_Worldstate>(states);
    }

    public override void Run(GOAP_Agent agent)
    {
        if (questStates == null) return;
        string msg = "Posting Quest:\n";
        foreach (GOAP_Worldstate state in questStates)
        {
            msg += "Fulfill: (" + state.key.ToString() + "|" + state.value.ToString() +  ")\n";
        }

        Debug.Log(msg);
    }
}
