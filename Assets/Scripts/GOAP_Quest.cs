using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Quest
{
    static int id = 0;
    GOAP_Agent owner;
    HashSet<GOAP_Worldstate> requiredStates;
    HashSet<GOAP_Worldstate> providedStates;

    public GOAP_Quest()
    {
        id++;
        requiredStates = new HashSet<GOAP_Worldstate>();
        providedStates = new HashSet<GOAP_Worldstate>();
    }

    public GOAP_Quest(GOAP_Agent owner, IEnumerable<GOAP_Worldstate> required, IEnumerable<GOAP_Worldstate> provided)
    {
        id++;
        this.owner = owner;
        requiredStates = new HashSet<GOAP_Worldstate>(required);
        providedStates = new HashSet<GOAP_Worldstate>(provided);
    }

    public void AddRequired(GOAP_Worldstate state)
    {
        requiredStates.Add(state);
    }

    public void AddProvided(GOAP_Worldstate state)
    {
        providedStates.Add(state);
    }

    public void Complete()
    {
        owner.postedQuest = null;
    }

    public override string ToString()
    {
        string quest = "QUEST " + id + ": I need someone to complete:";
        foreach (GOAP_Worldstate state in requiredStates)
        {
            quest += " " + state.ToString() + ";";
        }
        quest += " \nIf necessary, I can provide:";
        foreach (GOAP_Worldstate state in providedStates)
        {
            quest += " " + state.ToString() + ";";
        }
        quest += "\n and will pay you handsomly.";
        return quest;
    }
}
