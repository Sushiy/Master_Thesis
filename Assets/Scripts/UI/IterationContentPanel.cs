using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IterationContentPanel : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI current;
    public TextMeshProUGUI openSet;
    public TextMeshProUGUI chosen;

    public void SetContent(PlanInfo.IterationInfo iterationInfo)
    {
        title.text = "Iteration " + iterationInfo.iterationDepth;
        current.text = iterationInfo.currentNode;
        openSet.text = iterationInfo.openSet;
        chosen.text = "Chosen Node:" + iterationInfo.chosenNode;
    }

}
