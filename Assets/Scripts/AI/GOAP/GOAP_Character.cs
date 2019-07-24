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
    
    public List<string> availableActions;

    public List<ItemType> startingInventory;

    public GOAP_Agent agent;

    public GameObjectActionTarget home;

    [Header("HealthData")]
    
    public float food = 100f;
    float hungerSpeed = 2.5f; //points of hunger per second => 40s
    public float sleep = 100f;
    float tiredSpeed = 0.8f; //points of tiredness per second => 120s
    public float social = 100f;
    float lonelySpeed = 1.2f; //points of tiredness per second => 83s

    float health;

    public void UpdateHealthData(float deltaTime)
    {
        if (food < 0)
        {
            Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is hungry");
            agent.ChangeCurrentWorldState(WorldStateKey.bHasEaten, false);
            food = 0;
        }
        else if(food > 0)
        {
            food -= deltaTime * hungerSpeed;
        }

        if (sleep < 0)
        {
            Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is tired");
            agent.ChangeCurrentWorldState(WorldStateKey.bHasSlept, false);
            sleep = 0;
        }
        else if (sleep > 0)
        {
            sleep -= deltaTime * tiredSpeed;
        }

        if (social < 0)
        {
            Debug.Log("<color=#0000cc>" + agent.Character.characterName + "</color> is lonely");
            agent.ChangeCurrentWorldState(WorldStateKey.bHasSocialised, false);
            social = 0;
        }
        else if (social > 0)
        {
            social -= deltaTime * lonelySpeed;
        }
    }

    public void Eat()
    {
        food = 100;
    }
    public void Sleep()
    {
        sleep = 100;
    }
    public void Socialise()
    {
        social = 100;
    }

    private void Awake()
    {
        agent = new GOAP_Agent(this, GetComponent<IGOAP_AgentView>());
        inventory = new Inventory();
        AddStartingInventory();
        agent.ChangeCurrentWorldState(WorldStateKey.bHasSlept, true);
        agent.ChangeCurrentWorldState(WorldStateKey.bHasEaten, true);
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
