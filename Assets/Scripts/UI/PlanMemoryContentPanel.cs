using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanMemoryContentPanel : MonoBehaviour
{
    public TextMeshProUGUI label;
    public Button detailButton;

    PlanInfo plan;
    
    public void SetContent(PlanInfo plan)
    {
        label.text = "Plan " + plan.PlanID + "\nGoal: " + plan.goalInfo;
        this.plan = plan;
        detailButton.onClick.AddListener(() => { UIPlandetailWindow.instance.ShowWindow(this.plan); });
    }
}
