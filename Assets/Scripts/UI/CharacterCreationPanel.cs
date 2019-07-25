using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class CharacterCreationPanel : MonoBehaviour
{
    public TMP_InputField nameField;
    [Space]
    public Transform skillsParent;
    public GameObject skillsPrefab;
    [Space]
    public Transform goalsParent;
    public GameObject goalsPrefab;
    [Space]
    public Transform inventoryParent;
    public GameObject inventoryPrefab;
    [Space]
    public Transform actionsParent;
    public GameObject actionsPrefab;

    private GOAP_Character.CharacterData characterData;
    private CharacterSpawner spawner;

    private List<TMP_Dropdown.OptionData> skillOptions;
    private List<TMP_Dropdown.OptionData> worldStateOptions;
    private List<TMP_Dropdown.OptionData> ItemTypeOptions;
    private List<TMP_Dropdown.OptionData> boolOptions;
    private List<ItemType> itemTypes;

    private void Awake()
    {
        skillOptions = Enum.GetValues(typeof(Skills)).Cast<Skills>().Select(v => new TMP_Dropdown.OptionData(v.ToString())).ToList();

        worldStateOptions = Enum.GetValues(typeof(WorldStateKey)).Cast<WorldStateKey>().Select(v => new TMP_Dropdown.OptionData(v.ToString())).ToList();

        itemTypes = Enum.GetValues(typeof(ItemType)).Cast<ItemType>().ToList();
        ItemTypeOptions = itemTypes.Select(v => new TMP_Dropdown.OptionData(v.ToString())).ToList();
        boolOptions = new List<TMP_Dropdown.OptionData>();
        boolOptions.Add(new TMP_Dropdown.OptionData("false"));
        boolOptions.Add(new TMP_Dropdown.OptionData("true"));

    }

    public void SetContent(GOAP_Character.CharacterData characterData, CharacterSpawner spawner)
    {
        Debug.Log("Setting content for character " + characterData.characterName);
        this.characterData = characterData;
        this.spawner = spawner;

        nameField.text = characterData.characterName;

        for (int i = 0; i < characterData.skills.Count; i++)
        {
            AddSkill(characterData.skills[i]);
        }
        for (int i = 0; i < characterData.goals.Count; i++)
        {
            AddGoal(characterData.goals[i]);
        }
        for (int i = 0; i < characterData.startingInventory.Count; i++)
        {
            AddInventory(characterData.startingInventory[i]);
        }

        UpdateActions();
    }

    private void Update()
    {
        if(characterData != null)
            UpdateActions();
    }

    public void OnRemoveClicked()
    {
        spawner.characterDatas.Remove(characterData);
        Destroy(gameObject);
    }

    private void AddSkill(GOAP_Skill skill)
    {
        SkillsContentPanel p = Instantiate(skillsPrefab, skillsParent).GetComponent<SkillsContentPanel>();
        p.SetContent(characterData, skillOptions, (int)skill.id, skill.level.ToString());
    }

    private void AddGoal(GOAP_Worldstate goal)
    {
        GoalListPanel p = Instantiate(goalsPrefab, goalsParent).GetComponent<GoalListPanel>();
        p.SetContent(characterData, worldStateOptions, boolOptions, ItemTypeOptions, itemTypes, (int)goal.key, goal.value);
        Debug.Log("Add Goal " + goal.ToString());
    }

    private void AddInventory(ItemType item)
    {
        InventoryPanel p = Instantiate(inventoryPrefab, inventoryParent).GetComponent<InventoryPanel>();
        p.SetContent(characterData, ItemTypeOptions, itemTypes.IndexOf(item));
    }

    private void UpdateActions()
    {
        int countDifference = actionsParent.childCount - characterData.availableActions.Count;
        if(countDifference > 0)
        {
            for(int i = actionsParent.childCount - 1; i >= actionsParent.childCount - countDifference; i--)
            {
                Destroy(actionsParent.GetChild(i).gameObject);
            }
            countDifference = 0;
        }
        if(countDifference < 0)
        {
            countDifference *= -1;
        }
        
        for(int i = 0; i < countDifference; i++)
        {
            Instantiate(actionsPrefab, actionsParent);
        }

        for (int i = 0; i < actionsParent.childCount; i++)
        {
            SimpleContentPanel p = actionsParent.GetChild(i).GetComponent<SimpleContentPanel>();
            p.SetContent(characterData.availableActions[i], "");
        }
    }

    public void NewSkill()
    {
        SkillsContentPanel p = Instantiate(skillsPrefab, skillsParent).GetComponent<SkillsContentPanel>();
        p.SetContent(characterData, skillOptions);
        characterData.skills.Add(new GOAP_Skill(0, 0));
    }

    public void NewGoal()
    {
        GoalListPanel p = Instantiate(goalsPrefab, goalsParent).GetComponent<GoalListPanel>();
        p.SetContent(characterData, worldStateOptions, boolOptions, ItemTypeOptions, itemTypes);
        characterData.goals.Add(new GOAP_Worldstate(0, false));
    }

    public void NewInventoryItem()
    {
        InventoryPanel p = Instantiate(inventoryPrefab, inventoryParent).GetComponent<InventoryPanel>();
        p.SetContent(characterData, ItemTypeOptions);
        characterData.startingInventory.Add(0);
    }

    public void EditActions()
    {
        AvailableActionsWindow.instance.ShowWindow(characterData);
    }
}
