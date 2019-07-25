using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPanel : DropDownValuePanel
{
    List<ItemType> itemTypes;

    public void SetContent(List<ItemType> itemTypes, GOAP_Character.CharacterData characterData, List<TMP_Dropdown.OptionData> optionData, int dropDownValue = 0, string value = "")
    {
        this.itemTypes = itemTypes;
        base.SetContent(characterData, optionData, dropDownValue, value);
    }

    public override void OnDropdownChanged(int i)
    {
        characterData.startingInventory[transform.GetSiblingIndex()] = itemTypes[i];
    }

    public override void OnRemoveClicked()
    {
        characterData.startingInventory.RemoveAt(transform.GetSiblingIndex());
        Destroy(gameObject);
    }

    public override void OnValueChanged(string s)
    {
        throw new System.NotImplementedException();
    }
}
