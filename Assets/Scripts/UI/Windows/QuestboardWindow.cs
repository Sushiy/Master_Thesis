using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestboardWindow : BasicWindow
{
    public static QuestboardWindow instance;
    
    public RectTransform questsParent;
    public GameObject questPanelPrefab;

    [HideInInspector]
    public PlanInfo planInfo;

    Dictionary<int, GameObject> panels;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        panels = new Dictionary<int, GameObject>();

    }

    private void Start()
    {

        GOAP_QuestBoard.instance.addQuestEvent = AddQuest;
        GOAP_QuestBoard.instance.completeQuestEvent = RemoveQuest;

        gameObject.SetActive(false);
    }

    private void OnGUI()
    {

    }

    public override void ShowWindow()
    {
        ClearQuests();
        UpdateWindow();
        base.ShowWindow();
    }

    private void UpdateWindow()
    {
        if (questsParent != null)
        {
            foreach(KeyValuePair<int, GOAP_Quest> pair in GOAP_QuestBoard.instance.quests)
            {
                AddQuest(pair.Value);            
            }
        }
    }

    public override void HideWindow()
    {
        base.HideWindow();
    }

    public void ClearQuests()
    {
        List<int> list = new List<int>(panels.Keys);
        for (int i = list.Count-1; i >= 0; i--)
        {
            RemoveQuest(list[i]);
        }
    }

    public void AddQuest(GOAP_Quest quest)
    {
        GameObject g = Instantiate(questPanelPrefab, questsParent);
        g.GetComponent<QuestContentPanel>().SetContent("Quest " + quest.id, quest.ToLongString(), "Owner: " + quest.Owner.Character.characterData.characterName + "\nRequired: " + quest.RequiredStates.ToString() + "\n\nReward: " + quest.Reward);
        panels.Add(quest.id, g);
    }

    public void RemoveQuest(int id)
    {
        GameObject g = panels[id];
        panels.Remove(id);
        Destroy(g);
    }
}
