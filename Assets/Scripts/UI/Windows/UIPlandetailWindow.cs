using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlandetailWindow : BasicWindow
{
    public static UIPlandetailWindow instance;

    public GameObject iterationPanelPrefab;
    public GameObject goalPanelPrefab;
    public GameObject finalPanelPrefab;
    public RectTransform scrollContentParent;

    public TextMeshProUGUI title;
    string characterName;

    public TextMeshProUGUI goal;
    public TextMeshProUGUI actionQueue;
    public TextMeshProUGUI totalWorkCost;
    public TextMeshProUGUI iterationCount;
    public TextMeshProUGUI nodeCount;


    [HideInInspector]
    public PlanInfo planInfo;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        gameObject.SetActive(false);
    }

    private void OnGUI()
    {

    }

    public void ShowWindow(PlanInfo planInfo)
    {
        this.planInfo = planInfo;
        this.characterName = planInfo.characterName;
        UpdateWindow();
        ShowWindow();
    }

    private void UpdateWindow()
    {
        title.text = characterName + " - Plan " + planInfo.PlanID;
        ClearIterations();
        for(int i = 0; i < planInfo.iterations.Count; i++)
        {
            if(i == 0)
            {
                AddGoal();
            }
            else
            {
                AddIteration(planInfo.iterations[i]);
            }
        }
        AddFinal();

        goal.text = "Goal:\n" +  planInfo.goalInfo;
        actionQueue.text = "Action Queue:\n" + planInfo.actionQueueInfo;
    }

    public override void HideWindow()
    {
        base.HideWindow();
    }

    void AddIteration(PlanInfo.IterationInfo iterationInfo)
    {
        IterationContentPanel iteration = Instantiate(iterationPanelPrefab, scrollContentParent).GetComponent<IterationContentPanel>();
        iteration.SetContent(iterationInfo);
    }

    void AddGoal()
    {
        SimpleContentPanel goal = Instantiate(goalPanelPrefab, scrollContentParent).GetComponent<SimpleContentPanel>();
        goal.SetContent("GOAL", planInfo.goalInfo);
    }

    void AddFinal()
    {
        SimpleContentPanel final = Instantiate(finalPanelPrefab, scrollContentParent).GetComponent<SimpleContentPanel>();
        final.SetContent("START", planInfo.iterations[planInfo.iterations.Count-1].chosenNode);
    }

    void ClearIterations()
    {

        IterationContentPanel[] iterations = scrollContentParent.GetComponentsInChildren<IterationContentPanel>();
        for (int i = 0; i < iterations.Length; i++)
        {
            Destroy(iterations[i].gameObject);
        }
        SimpleContentPanel[] simple = scrollContentParent.GetComponentsInChildren<SimpleContentPanel>();
        for (int i = 0; i < simple.Length; i++)
        {
            Destroy(simple[i].gameObject);
        }
    }
}
