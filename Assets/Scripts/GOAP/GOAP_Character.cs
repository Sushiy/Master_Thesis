using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Character : MonoBehaviour
{
    public string characterName;

    public List<GOAP_Skill> skills;

    Inventory inventory;
    public Inventory Inventory
    {
        get { return inventory; }
    }

    [HideInInspector]
    public PlannableActions availableActions;

    public GOAP_Agent agent;

    private void Awake()
    {
        agent = new GOAP_Agent(this, GetComponent<IGOAP_AgentView>());
        inventory = new Inventory();
    }

    private void Update()
    {
        agent.Update(Time.deltaTime);
    }

    public void AddSkill(Skills id, int level)
    {
        GOAP_Skill skill = new GOAP_Skill(id, level);
        if (skills.Contains(skill)) return;
        skills.Add(skill);
    }

    public void UpdateInventory(ItemIds id, bool adding, int count = 1)
    {
        if(adding)
        {   
            if(inventory.AddItem(new Item(id), count))
            {
                agent.ChangeCurrentWorldState(new GOAP_Worldstate(WorldStateKey.eHasItem, (int)id));
            }
        }
        else
        {
            if(inventory.RemoveItem(new Item(id), count))
            {
                agent.ConsumeWorldState(id);
            }
        }
    }
}
