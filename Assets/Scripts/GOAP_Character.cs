using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Character : MonoBehaviour
{
    public string characterName;

    public List<GOAP_Skill> skills;

    public void AddSkill(Skills id, int level)
    {
        GOAP_Skill skill = new GOAP_Skill(id, level);
        if (skills.Contains(skill)) return;
        skills.Add(skill);
    }
}
