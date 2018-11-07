using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_WhiteBoard : MonoBehaviour
{
    public static GOAP_WhiteBoard instance;
    public List<GOAP_Quest> quests;

	// Use this for initialization
	void Awake ()
    {
        instance = this;
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
}
