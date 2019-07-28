using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class GOAP_Character : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData
    {
        public string characterName;
        public int characterSkinIndex;

        public List<GOAP_Skill> skills;

        public List<GOAP_Worldstate> goals;

        [HideInInspector]
        public List<string> availableActions;

        public List<ItemType> startingInventory;

        public float laziness;


        public CharacterData(string characterName, List<GOAP_Skill> skills, List<GOAP_Worldstate> goals, List<string> availableActions, List<ItemType> startingInventory, float laziness)
        {
            this.characterName = characterName;
            this.skills = skills;
            this.goals = goals;
            this.availableActions = availableActions;
            this.startingInventory = startingInventory;
            this.laziness = laziness;
        }

        public CharacterData(CharacterData characterData) : this(characterData.characterName
            , new List<GOAP_Skill>(characterData.skills)
            , new List<GOAP_Worldstate>(characterData.goals)
            , new List<string>(characterData.availableActions)
            , new List<ItemType>(characterData.startingInventory)
            , characterData.laziness
            )
        {

        }

        public void InitBaseActions(string[] allActions)
        {
            for (int i = 0; i < allActions.Length; i++)
            {
                string action = allActions[i];
                if (!availableActions.Contains(action) && GOAP_Action.baseActions.Contains(action))
                {
                    availableActions.Add(action);
                }
            }
        }

        public void RemoveWrongActions(string[] allActions)
        {
            for (int i = availableActions.Count - 1; i >= 0; i--)
            {
                string action = availableActions[i];
                if (!allActions.Contains(action) || action == "Action_CompleteQuest" || action == "Action_WaitForQuest" || action == "Action_PostQuest")
                {
                    availableActions.RemoveAt(i);
                }
            }
        }
    }

    public CharacterData characterData;

    [Header("HealthData")]
    public float food = 100f;
    public float sleep = 100f;
    public float social = 100f;
    float hungerSpeed = 2.5f; //points of hunger per second => 40s
    float tiredSpeed = 0.8f; //points of tiredness per second => 120s
    float lonelySpeed = 1.2f; //points of tiredness per second => 83s
    float health;

    public GOAP_Agent agent;
    float agentDeltaTime;

    public string log;

    public GameObjectActionTarget home;

    Inventory inventory;
    public Inventory Inventory
    {
        get { return inventory; }
    }

    private void Awake()
    {
        agent = new GOAP_Agent(this, GetComponent<IGOAP_AgentView>());
        inventory = new Inventory();
        agent.ChangeCurrentWorldState(WorldStateKey.bHasSlept, true);
        agent.ChangeCurrentWorldState(WorldStateKey.bHasEaten, true);
    }

    public void SetCharacterData(CharacterData data)
    {
        characterData = data;
        AddStartingInventory();
    }

    public void UpdateHealthData(float deltaTime)
    {
        if (food < 0)
        {
            Debug.Log("<color=#0000cc>" + characterData.characterName + "</color> is hungry");
            agent.ChangeCurrentWorldState(WorldStateKey.bHasEaten, false);
            food = 0;
        }
        else if(food > 0)
        {
            food -= deltaTime * hungerSpeed;
        }

        if (sleep < 0)
        {
            Debug.Log("<color=#0000cc>" + characterData.characterName + "</color> is tired");
            agent.ChangeCurrentWorldState(WorldStateKey.bHasSlept, false);
            sleep = 0;
        }
        else if (sleep > 0)
        {
            sleep -= deltaTime * tiredSpeed;
        }

        if (social < 0)
        {
            Debug.Log("<color=#0000cc>" + characterData.characterName + "</color> is lonely");
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

    private void AddStartingInventory()
    {
        for(int i = 0; i < characterData.startingInventory.Count; i++)
        {
            UpdateInventory(characterData.startingInventory[i], true);
        }
    }

    private void Update()
    {
        agentDeltaTime += Time.deltaTime;
        if(agentDeltaTime >= 0.1f)
        {
            agent.Update(agentDeltaTime);
            UpdateHealthData(agentDeltaTime);
            agentDeltaTime = 0;
        }
    }

    public void AddSkill(Skills id, int level)
    {
        GOAP_Skill skill = new GOAP_Skill(id, level);
        if (characterData.skills.Contains(skill)) return;
        characterData.skills.Add(skill);
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

    public void Log(string msg)
    {
        Debug.Log(msg);
        log += "\n" + msg;
    }
}
