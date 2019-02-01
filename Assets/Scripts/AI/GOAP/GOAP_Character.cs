using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GOAP_Character : MonoBehaviour
{
    public string characterName;

    public List<GOAP_Skill> skills;

    public List<GOAP_Worldstate> goals;

    public List<ItemType> itemStockPilePriorities;

    Inventory inventory;
    public Inventory Inventory
    {
        get { return inventory; }
    }

    [HideInInspector]
    public PlannableActions availableActions;

    public List<ItemType> startingInventory;

    public GOAP_Agent agent;

    [Header("HealthData")]

    //All three values are measured from 1-12
    public float food = 40;
    float hungerSpeed = 1f;
    public float sleep = 120;
    float tiredSpeed = 1f;
    float health;

    public void UpdateHealthData(float deltaTime)
    {
        if (food < 0)
        {
            Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is hungry");
            agent.ChangeCurrentWorldState(WorldStateKey.bIsHungry, true);
            food = 0;
        }
        else if(food > 0)
        {
            food -= deltaTime * hungerSpeed;
        }

        if (sleep < 0)
        {
            Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is tired");
            agent.ChangeCurrentWorldState(WorldStateKey.bIsTired, true);
            sleep = 0;
        }
        else if (sleep > 0)
        {
            sleep -= deltaTime * tiredSpeed;
        }
    }

    public void Eat()
    {
        food = 40;
    }
    public void Sleep()
    {
        sleep = 120;
    }

    private void Awake()
    {
        agent = new GOAP_Agent(this, GetComponent<IGOAP_AgentView>());
        inventory = new Inventory();
        AddStartingInventory();
    }

    private void AddStartingInventory()
    {
        for(int i = 0; i < startingInventory.Count; i++)
        {
            UpdateInventory(startingInventory[i], true);
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        agent.Update(deltaTime);
        UpdateHealthData(deltaTime);
    }

    public void AddSkill(Skills id, int level)
    {
        GOAP_Skill skill = new GOAP_Skill(id, level);
        if (skills.Contains(skill)) return;
        skills.Add(skill);
    }

    public void UpdateInventory(ItemType id, bool adding, int count = 1)
    {
        if(adding)
        {   
            if(inventory.AddItem(id, count))
            {
                agent.ChangeCurrentWorldState(new GOAP_Worldstate(WorldStateKey.eHasItem, (int)id));
                //Debug.Log("Added " + id.ToString() + "(" + (int)id + ")" + " to " + characterName);
            }
        }
        else
        {
            if(inventory.RemoveItem(id, count))
            {
                agent.RemoveCurrentWorldState(id);
            }
        }
    }
}
