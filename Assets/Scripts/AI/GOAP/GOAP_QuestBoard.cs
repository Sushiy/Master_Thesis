using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_QuestBoard : MonoBehaviour
{
    public static GOAP_QuestBoard instance;
    public SortedDictionary<int, GOAP_Quest> quests;

	// Use this for initialization
	void Awake ()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        quests = new SortedDictionary<int, GOAP_Quest>();
	}
	
	public void AddQuest(GOAP_Quest quest)
    {
        quests.Add(quest.id, quest);
    }

    public void CompleteQuest(int index)
    {
        quests[index].Complete();
        quests.Remove(index);
    }
}
