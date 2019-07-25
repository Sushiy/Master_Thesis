using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AvailableActionsWindow : BasicWindow
{
    public static AvailableActionsWindow instance;

    public RectTransform actionsParent;
    public GameObject actionPanelPrefab;

    [HideInInspector]
    public GOAP_Character.CharacterData characterData;

    string[] allActions;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
        if (allActions == null)
        {
            //Add all actions to the list
            allActions = GOAP_Action.GetAllActionNames();
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        //UpdateWindow();
    }

    public void ShowWindow(GOAP_Character.CharacterData characterData)
    {
        this.characterData = characterData;
        characterData.InitBaseActions(allActions);
        characterData.RemoveWrongActions(allActions);
        UpdateWindow();
        base.ShowWindow();
    }

    private void UpdateWindow()
    {
        for (int i = 0; i < allActions.Length; i++)
        {
            string actionName = allActions[i].ToString();
            AvailableActionPanel actionPanel = Instantiate(actionPanelPrefab, actionsParent).GetComponent<AvailableActionPanel>();
            actionPanel.SetContent(actionName, characterData.availableActions.Contains(actionName));

            if (GOAP_Action.baseActions.Contains(actionName))
            {
                actionPanel.buttonImage.GetComponent<Button>().interactable = false;
                actionPanel.buttonImage.color = Color.grey;
                actionPanel.buttonLabel.text = "BASE";
                actionPanel.transform.SetAsFirstSibling();
                if(!characterData.availableActions.Contains(actionName))
                {
                    characterData.availableActions.Add(actionName);
                }
            }
        }

        //remove all actions not on the list from available
        for (int i = characterData.availableActions.Count - 1; i >= 0; i--)
        {
            string action = characterData.availableActions[i];
            if (!allActions.Contains(action) || action == "Action_CompleteQuest" || action == "Action_WaitForQuest" || action == "Action_PostQuest")
            {
                characterData.availableActions.RemoveAt(i);
            }
        }
    }

    public override void HideWindow()
    {
        base.HideWindow();
    }

    public void AddAction(GOAP_Action action)
    {
        GameObject g = Instantiate(actionPanelPrefab, actionsParent);
    }

    public bool TogglePanel(string actionName)
    {
        bool contained = characterData.availableActions.Contains(actionName);
        if (!contained)
        {
            characterData.availableActions.Add(actionName);
            return true;
        }
        else
        {
            characterData.availableActions.Remove(actionName);
            return false;
        }
    }
}