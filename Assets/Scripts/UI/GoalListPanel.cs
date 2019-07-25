using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class GoalListPanel : MonoBehaviour
{
    GOAP_Character.CharacterData characterData;

    public TMP_Dropdown stateKey;
    public TMP_Dropdown value;

    List<TMP_Dropdown.OptionData> itemOptions;
    List<TMP_Dropdown.OptionData> boolOptions;

    List<ItemType> itemTypes;

    // Use this for initialization
    void Start ()
    {
        stateKey.onValueChanged.AddListener(OnStateKeyChanged);
        value.onValueChanged.AddListener(OnValueChanged);
    }

    public void SetContent(GOAP_Character.CharacterData characterData, List<TMP_Dropdown.OptionData> worldStateOptions, List<TMP_Dropdown.OptionData> boolOptions, List<TMP_Dropdown.OptionData> itemOptions, List<ItemType> itemTypes, int stateKeyValue = 0, int valueValue = 0)
    {
        stateKey.options = worldStateOptions;
        this.characterData = characterData;
        this.boolOptions = boolOptions;
        this.itemOptions = itemOptions;
        this.itemTypes = itemTypes;
        stateKey.value = stateKeyValue;
        value.value = valueValue;
    }

    void OnStateKeyChanged(int i)
    {
        if(stateKey.options[i].text == "eHasItem")
        {
            SetItemDropdown();
        }
        else
        {
            SetBoolDropdown();
        }
        characterData.goals[transform.GetSiblingIndex()].key = (WorldStateKey)i;
    }

    void OnValueChanged(int i)
    {
        if (stateKey.options[i].text == "eHasItem")
        {
            characterData.goals[transform.GetSiblingIndex()].value = (int)itemTypes[i];
        }
        else
        {
            characterData.goals[transform.GetSiblingIndex()].value = i;
        }
    }

    public void OnRemoveClicked()
    {
        characterData.goals.RemoveAt(transform.GetSiblingIndex());
        Destroy(gameObject);
    }

    void SetItemDropdown()
    {
        value.options = itemOptions;
        value.value = 0;
    }

    void SetBoolDropdown()
    {
        value.options = boolOptions;
        value.value = 0;
    }
}
