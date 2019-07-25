using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : DropDownValuePanel
{
    public override void OnDropdownChanged(int i)
    {
        characterData.startingInventory[transform.GetSiblingIndex()] = (ItemType)i;
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
