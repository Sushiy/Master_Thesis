using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UICharacterWindow : BasicWindow
{
    public static UICharacterWindow instance;

    GOAP_Character character;

    [Header("General")]
    public TextMeshProUGUI title;
    public TextMeshProUGUI[] skills;

    [Header("Inventory")]
    public Transform inventoryContentPanel;
    public GameObject inventoryContentPrefab;
    private List<GameObject> inventoryItems;

    [Header("GOAP")]
    public TextMeshProUGUI currentGoal;
    public TextMeshProUGUI currentAction;
    public TextMeshProUGUI currentWorldstate;

    [Header("Quests")]
    public TextMeshProUGUI postedQuests;
    public TextMeshProUGUI completedQuests;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        instance = this;
        inventoryItems = new List<GameObject>();

        if(inventoryContentPanel != null)
        {
            for (int i = 0; i < 16; i++)
            {
                GameObject itemPanel = GameObject.Instantiate(inventoryContentPrefab, inventoryContentPanel);
                itemPanel.SetActive(false);
                inventoryItems.Add(itemPanel);
            }
        }
        gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        UpdateWindow();
    }

    public void ShowWindow(GOAP_Character character)
    {
        this.character = character;
        UpdateWindow();
        ShowWindow();
    }

    public override void HideWindow()
    {
        base.HideWindow();
    }

    private void ClearInventoryPanel()
    {
        for(int i = 0; i < inventoryItems.Count; i++)
        {
            inventoryItems[i].SetActive(false);
        }
    }

    private void UpdateWindow()
    {
        ClearInventoryPanel();
        title.text = character.characterName;

        if(skills != null && skills.Length > 0)
        {
            for (int i = 0; i < character.skills.Count; i++)
            {
                skills[i].text = character.skills[i].id.ToString() + ": " + character.skills[i].level;
            }
        }
        if(inventoryContentPanel != null)
        {
            int index = 0;
            foreach (KeyValuePair<ItemType, int> item in character.Inventory.Items)
            {
                inventoryItems[index].SetActive(true);
                inventoryItems[index].GetComponent<TextMeshProUGUI>().text = item.Key.ToString() + ":" + item.Value;
                index++;
            }
        }

        currentGoal.text = "Goal: " + character.agent.PrintGoal();
        currentAction.text = "Action:" + ((character.agent.activeAction != null) ? character.agent.activeAction.ActionID : "None");

        if(currentWorldstate != null)
        {
            currentWorldstate.text = "Worldstate:\n";
            for(int i = 0; i < character.agent.currentWorldstates.Count; i++)
            {
                currentWorldstate.text += "- " + character.agent.currentWorldstates[i].ToString() + "\n";
            }
        }

        if(postedQuests != null)
        {
            postedQuests.text = "Posted Quests:\n";
            for(int i = 0; i < character.agent.postedQuestIDs.Count; i++)
            {
                postedQuests.text += "- " + GOAP_QuestBoard.instance.quests[character.agent.postedQuestIDs[i]].ToString() + "\n";
            }
        }

        if (completedQuests != null)
        {
            completedQuests.text = "Completed Quests:\n";
            for (int i = 0; i < character.agent.completedQuestIDs.Count; i++)
            {
                completedQuests.text += "- " + GOAP_QuestBoard.instance.quests[character.agent.completedQuestIDs[i]].ToString() + "\n";
            }
        }
    }
}
