using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestContentPanel : SimpleContentPanel
{
    public TextMeshProUGUI infoLabel;

    public void SetContent(string title, string description, string info)
    {
        base.SetContent(title, description);

        infoLabel.text = info;
    }
}
