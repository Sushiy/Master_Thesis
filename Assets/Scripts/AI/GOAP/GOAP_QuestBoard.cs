using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_QuestBoard : MonoBehaviour
{
    public static GOAP_QuestBoard instance;
    public SortedDictionary<int, GOAP_Quest> quests;
    public SortedDictionary<int, GOAP_Quest> questArchive;

    public Action<GOAP_Quest> addQuestEvent;
    public Action<int> completeQuestEvent;

    // Use this for initialization
    void Awake ()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        quests = new SortedDictionary<int, GOAP_Quest>();
        questArchive = new SortedDictionary<int, GOAP_Quest>();
    }
	
	public void AddQuest(GOAP_Quest quest)
    {
        quests.Add(quest.id, quest);
        addQuestEvent.Invoke(quest);
    }

    public void CompleteQuest(int index)
    {
        quests[index].Complete();
        questArchive.Add(index, quests[index]);
        quests.Remove(index);
        completeQuestEvent.Invoke(index);
    }
}
