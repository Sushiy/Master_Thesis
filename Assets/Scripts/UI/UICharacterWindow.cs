using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICharacterWindow : BasicWindow
{
    public static UICharacterWindow instance;

    GOAP_Character character;

    [Header("General")]
    public TextMeshProUGUI title;

    [Header("GOAPBasics")]
    public TextMeshProUGUI currentGoal; //Can be a quest
    public TextMeshProUGUI currentAction;
    public TextMeshProUGUI currentAgentState;

    [Header("CharacterBasics")]
    public Slider hungrySlider;
    public Slider tiredSlider;
    public Slider lonelySlider;

    [Space]

    public RectTransform contentParent;
    public RectTransform[] contentTabs;
    private int activeTab = 0;

    [Header("GOAPTab")]
    public GameObject planMemoryPanelPrefab;
    public TextMeshProUGUI currentPlan;
    public RectTransform planMemoryParent;
    public TextMeshProUGUI currentWorldstate;

    [Header("QuestTab")]
    public TextMeshProUGUI postedQuests;
    public TextMeshProUGUI completedQuests;

    [Header("CharacterTab")]
    public TextMeshProUGUI[] skills;
    public RectTransform inventoryContentPanel;
    public GameObject inventoryContentPrefab;
    private List<GameObject> inventoryItems;

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
        if (this.character != character)
        {
            ClearPlanMemory();
        }
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
        title.text = character.characterName;

        //GOAP basic
        currentGoal.text = "Goal: " + character.agent.PrintGoal();
        currentAction.text = "Action: " + ((character.agent.activeAction != null) ? character.agent.activeAction.ActionID : "None");
        currentAgentState.text = "State: " + character.agent.PrintCurrentState();

        //Character basic
        hungrySlider.value = character.food / 100f;
        tiredSlider.value = character.sleep / 100f;
        lonelySlider.value = character.social / 100f;

        //Character Tab
        ClearInventoryPanel();

        if (skills != null && skills.Length > 0)
        {
            for (int i = 0; i < character.skills.Count; i++)
            {
                skills[i].text = character.skills[i].id.ToString() + ": " + character.skills[i].level;
            }
        }
        if (inventoryContentPanel != null)
        {
            int index = 0;
            foreach (KeyValuePair<ItemType, int> item in character.Inventory.Items)
            {
                inventoryItems[index].SetActive(true);
                inventoryItems[index].GetComponent<TextMeshProUGUI>().text = item.Key.ToString() + ":" + item.Value;
                index++;
            }
        }

        //QuestTab
        if (postedQuests != null)
        {
            postedQuests.text = "";
            for(int i = 0; i < character.agent.postedQuestIDs.Count; i++)
            {
                postedQuests.text += "- " + GOAP_QuestBoard.instance.quests[character.agent.postedQuestIDs[i]].ToString() + "\n";
            }
        }

        if (completedQuests != null)
        {
            completedQuests.text = "";
            for (int i = 0; i < character.agent.completedQuestIDs.Count; i++)
            {
                completedQuests.text += "- " + GOAP_QuestBoard.instance.quests[character.agent.completedQuestIDs[i]].ToString() + "\n";
            }
        }

        //GOAP Tab

        if(character.agent.activePlanInfo != null)
        {
            PlanInfo plan = character.agent.planMemory[(int)character.agent.activePlanInfo];
            currentPlan.text = "Plan " + plan.PlanID + "; Goal: " + plan.goalInfo + "\n" + plan.actionQueueInfo;
            
        }
        else
        {
            currentPlan.text = "None";
        }
        if (planMemoryParent != null)
        {
            if (planMemoryParent.childCount < character.agent.planMemory.Count)
            {
                for (int i = planMemoryParent.childCount; i < character.agent.planMemory.Count; i++)
                {
                    AddPlan(character.agent.planMemory[i]);
                }
            }
        }

        if (currentWorldstate != null)
        {
            currentWorldstate.text = "Worldstate:\n";
            for (int i = 0; i < character.agent.currentWorldstates.Count; i++)
            {
                currentWorldstate.text += "- " + character.agent.currentWorldstates[i].ToString() + "\n";
            }
        }

    }

    public void SetActiveTab(int newActiveTab)
    {
        activeTab = newActiveTab;

        contentTabs[activeTab].SetAsLastSibling();

    }

    public void ShowPlan()
    {
        if (character.agent.activePlanInfo != null)
        {
            UIPlandetailWindow.instance.ShowWindow(character.agent.planMemory[(int)character.agent.activePlanInfo]);
        }
    }

    private void AddPlan(PlanInfo planinfo)
    {
        PlanMemoryContentPanel plan = Instantiate(planMemoryPanelPrefab, planMemoryParent).GetComponent<PlanMemoryContentPanel>();
        plan.SetContent(planinfo);
    }

    private void ClearPlanMemory()
    {
        PlanMemoryContentPanel[] plans = planMemoryParent.GetComponentsInChildren<PlanMemoryContentPanel>();
        for (int i = 0; i < plans.Length; i++)
        {
            Destroy(plans[i].gameObject);
        }
    }
}
