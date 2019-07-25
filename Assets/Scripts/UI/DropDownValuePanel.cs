using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class DropDownValuePanel : MonoBehaviour
{
    public TMP_Dropdown dropDown;
    public TMP_InputField valueField;

    protected GOAP_Character.CharacterData characterData;
    private void Awake()
    {
        if(dropDown != null)
        dropDown.onValueChanged.AddListener(OnDropdownChanged);
        if(valueField != null)
            valueField.onValueChanged.AddListener(OnValueChanged);
    }

    public void SetContent(GOAP_Character.CharacterData characterData, List<TMP_Dropdown.OptionData> optionData, int dropDownValue = 0, string value = "")
    {
        this.characterData = characterData;
        if (dropDown != null)
        {
            dropDown.options = optionData;
            dropDown.value = dropDownValue;

        }
        if (valueField != null)
            valueField.text = value;
    }

    public abstract void OnDropdownChanged(int i);
    public abstract void OnValueChanged(string s);
    public abstract void OnRemoveClicked();
}
