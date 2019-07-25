using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvailableActionWindowPanel : MonoBehaviour
{
    public TextMeshProUGUI nameLabel;
    public TextMeshProUGUI buttonLabel;
    public Image buttonImage;

    private string actionName;
	
    public void SetContent(string actionName, bool buttonState)
    {
        this.actionName = actionName;
        nameLabel.text = actionName;
        SetButtonState(buttonState);
    }

    public void Toggle()
    {
        SetButtonState(AvailableActionsWindow.instance.TogglePanel(actionName));
    }

    private void SetButtonState(bool state)
    {
        if(state)
        {
            Color c;
            ColorUtility.TryParseHtmlString("#52C03D", out c);
            buttonImage.color = c;
            buttonLabel.text = "true";
        }
        else
        {
            Color c;
            ColorUtility.TryParseHtmlString("#DE3936", out c);
            buttonImage.color = c;
            buttonLabel.text = "false";
        }
    }
}
