using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsContentPanel : DropDownValuePanel
{
    public override void OnDropdownChanged(int i)
    {
        characterData.skills[transform.GetSiblingIndex()].id = (Skills)i;
    }

    public override void OnRemoveClicked()
    {
        characterData.skills.RemoveAt(transform.GetSiblingIndex());
        Destroy(gameObject);
    }

    public override void OnValueChanged(string s)
    {
        characterData.skills[transform.GetSiblingIndex()].level = int.Parse(s);
    }
}
