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

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        instance = this;
        inventoryItems = new List<GameObject>();

        for (int i = 0; i < 16; i++)
        {
            GameObject itemPanel = GameObject.Instantiate(inventoryContentPrefab, inventoryContentPanel);
            itemPanel.SetActive(false);
            inventoryItems.Add(itemPanel);
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
        for (int i = 0; i < character.skills.Count; i++)
        {
            skills[i].text = character.skills[i].id.ToString() + ": " + character.skills[i].level;
        }

        int index = 0;
        foreach (KeyValuePair<Item, int> item in character.Inventory.Items)
        {
            inventoryItems[index].SetActive(true);
            inventoryItems[index].GetComponent<TextMeshProUGUI>().text = item.Key.Name + ":" + item.Value;
            index++;
        }

        currentGoal.text = character.agent.PrintGoal();
        currentAction.text = (character.agent.activeAction != null) ? character.agent.activeAction.ToString() : "None";
    }
}
