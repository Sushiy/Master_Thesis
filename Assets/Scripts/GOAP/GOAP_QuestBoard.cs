using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_QuestBoard : MonoBehaviour
{
    public static GOAP_QuestBoard instance;
    public List<GOAP_Quest> quests;

	// Use this for initialization
	void Awake ()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        quests = new List<GOAP_Quest>();
	}
	
	public void AddQuest(GOAP_Quest quest)
    {
        quests.Add(quest);
    }

    public void CompleteQuest(int index)
    {
        quests[index].Complete();
        quests.RemoveAt(index);
    }
    public bool ChooseQuest(GOAP_Quest quest)
    {
        return quests.Remove(quest); 
    }
}
