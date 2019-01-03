using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Quest
{
    private static int count = 0;
    public int id;
    GOAP_Agent owner;
    public GOAP_Agent Owner
    {
        get { return owner; }
    }
    HashSet<GOAP_Worldstate> requiredStates;
    public HashSet<GOAP_Worldstate> RequiredStates
    {
        get { return requiredStates; }
    }
    HashSet<GOAP_Worldstate> providedStates;
    public HashSet<GOAP_Worldstate> ProvidedStates
    {
        get { return providedStates; }
    }
    public GOAP_Quest(GOAP_Agent agent)
    {
        id = count++;
        owner = agent;
        requiredStates = new HashSet<GOAP_Worldstate>();
        providedStates = new HashSet<GOAP_Worldstate>();
    }

    public GOAP_Quest(GOAP_Agent owner, IEnumerable<GOAP_Worldstate> required, IEnumerable<GOAP_Worldstate> provided)
    {
        id = count++;
        this.owner = owner;
        requiredStates = new HashSet<GOAP_Worldstate>(required);
        providedStates = new HashSet<GOAP_Worldstate>(provided);
    }
    public void ClearRequired()
    {
        requiredStates.Clear();
    }
    public void AddRequired(GOAP_Worldstate state)
    {
        requiredStates.Add(state);
    }

    public void ClearProvided()
    {
        providedStates.Clear();
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
        string quest = "QUEST " + id + ": " + owner.Character.characterName + " needs someone to complete:";
        foreach (GOAP_Worldstate state in requiredStates)
        {
            quest += " " + state.ToString() + ";";
        }
        if(providedStates != null && providedStates.Count > 0)
        {
            quest += " \nIf necessary, he can provide:";
            foreach (GOAP_Worldstate state in providedStates)
            {
                quest += " " + state.ToString() + ";";
            }
            quest += "\n and will pay you handsomly.";
        }
        quest += "\n I will pay you handsomely.";
        return quest;
    }
}
