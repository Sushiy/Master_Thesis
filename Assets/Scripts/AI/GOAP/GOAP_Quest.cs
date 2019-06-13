﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QuestData
{
    public GOAP_Agent owner;
    public HashSet<GOAP_Worldstate> requiredStates;

    public HashSet<GOAP_Worldstate> providedStates;
    public void Init()
    {
        requiredStates = new HashSet<GOAP_Worldstate>();
        providedStates = new HashSet<GOAP_Worldstate>();
    }
    public QuestData(GOAP_Agent owner)
    {
        this.owner = owner;
        requiredStates = new HashSet<GOAP_Worldstate>();
        providedStates = new HashSet<GOAP_Worldstate>();
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

    public string RequiredToString()
    {
        string msg = "";
        foreach(GOAP_Worldstate state in requiredStates)
        {
            msg += state.ToString() + ", ";
        }
        return msg;
    }
}

public class GOAP_Quest
{
    private static int count = 0;
    public int id;

    private QuestData questData;

    public GOAP_Agent Owner
    {
        get { return questData.owner; }
    }
    
    public HashSet<GOAP_Worldstate> RequiredStates
    {
        get { return questData.requiredStates; }
    }
    
    public HashSet<GOAP_Worldstate> ProvidedStates
    {
        get { return questData.providedStates; }
    }
    public GOAP_Quest(QuestData data)
    {
        questData = data;
        id = count++;
    }

    public GOAP_Quest(GOAP_Agent agent, IEnumerable<GOAP_Worldstate> required, IEnumerable<GOAP_Worldstate> provided)
    {
        id = count++;
        questData = new QuestData
        {
            owner = agent,
            requiredStates = new HashSet<GOAP_Worldstate>(required),
            providedStates = new HashSet<GOAP_Worldstate>(provided)
        };
    }

    public void Complete()
    {
        Owner.postedQuest = null;
    }

    public override string ToString()
    {
        string quest = "QUEST " + id + ": " + Owner.Character.characterName + " needs someone to complete:";
        foreach (GOAP_Worldstate state in RequiredStates)
        {
            quest += " " + state.ToString() + ";";
        }
        if(ProvidedStates != null && ProvidedStates.Count > 0)
        {
            quest += " \nIf necessary, he can provide:";
            foreach (GOAP_Worldstate state in ProvidedStates)
            {
                quest += " " + state.ToString() + ";";
            }
            quest += "\n and will pay you handsomly.";
        }
        quest += "\n I will pay you handsomely.";
        return quest;
    }
}