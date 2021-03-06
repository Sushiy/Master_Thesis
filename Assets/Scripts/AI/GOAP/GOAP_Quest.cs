﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QuestData
{
    public GOAP_Agent owner;
    public List_GOAP_Worldstate requiredStates;

    public List_GOAP_Worldstate providedStates;

    public float reward;
    public void Init()
    {
        requiredStates = new List_GOAP_Worldstate();
        providedStates = new List_GOAP_Worldstate();
    }
    public QuestData(GOAP_Agent owner)
    {
        this.owner = owner;
        requiredStates = new List_GOAP_Worldstate();
        providedStates = new List_GOAP_Worldstate();
        reward = 0;
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
        return requiredStates.ToString();
    }
}

public class GOAP_Quest
{
    private static int count = 0;
    public int id;

    public float Reward
    {
        get { return questData.reward; }
    }

    private QuestData questData;

    public GOAP_Agent Owner
    {
        get { return questData.owner; }
    }
    
    public List_GOAP_Worldstate RequiredStates
    {
        get { return questData.requiredStates; }
    }
    
    public List_GOAP_Worldstate ProvidedStates
    {
        get { return questData.providedStates; }
    }
    public GOAP_Quest(QuestData data)
    {
        questData = data;
        id = count++;
    }

    public GOAP_Quest(GOAP_Agent agent, List_GOAP_Worldstate required, List_GOAP_Worldstate provided)
    {
        id = count++;
        questData = new QuestData
        {
            owner = agent,
            requiredStates = new List_GOAP_Worldstate(required),
            providedStates = new List_GOAP_Worldstate(provided)
        };
    }

    public void Complete()
    {
        if(Owner.postedQuestIDs.Contains(id))
        {
            Owner.completedQuestIDs.Add(id);
            Owner.postedQuestIDs.Remove(id);
        }
        else
        {
            Debug.LogError("Quest is no longer posted");
        }
    }

    public string ToLongString()
    {
        string quest = "QUEST " + id + ": " + Owner.Character.characterData.characterName + " needs someone to complete:";
        quest += RequiredStates.ToString();
        if(ProvidedStates != null && ProvidedStates.Count > 0)
        {
            quest += " \nIf necessary, he can provide:";
            quest += ProvidedStates.ToString();
            quest += "\n and will pay you handsomly.";
        }
        quest += "\nand will pay you handsomely.";
        return quest;
    }

    public override string ToString()
    {
        return "ID " + id + ": " + RequiredStates.ToString();
    }
}
