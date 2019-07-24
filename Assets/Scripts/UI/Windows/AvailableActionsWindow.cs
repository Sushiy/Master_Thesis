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
    public GOAP_Character character;

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

    public void ShowWindow(GOAP_Character character)
    {
        this.character = character;
        character.characterData.InitBaseActions(allActions);
        character.characterData.RemoveWrongActions(allActions);
        UpdateWindow();
        base.ShowWindow();
    }

    private void UpdateWindow()
    {
        for (int i = 0; i < allActions.Length; i++)
        {
            string actionName = allActions[i].ToString();
            AvailableActionPanel actionPanel = Instantiate(actionPanelPrefab, actionsParent).GetComponent<AvailableActionPanel>();
            actionPanel.SetContent(actionName, character.characterData.availableActions.Contains(actionName));

            if (GOAP_Action.baseActions.Contains(actionName))
            {
                actionPanel.buttonImage.GetComponent<Button>().interactable = false;
                actionPanel.buttonLabel.text = "BASE";
                actionPanel.transform.SetAsFirstSibling();
                if(!character.characterData.availableActions.Contains(actionName))
                {
                    character.characterData.availableActions.Add(actionName);
                }
            }
        }

        //remove all actions not on the list from available
        for (int i = character.characterData.availableActions.Count - 1; i >= 0; i--)
        {
            string action = character.characterData.availableActions[i];
            if (!allActions.Contains(action) || action == "Action_CompleteQuest" || action == "Action_WaitForQuest" || action == "Action_PostQuest")
            {
                character.characterData.availableActions.RemoveAt(i);
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
        bool contained = character.characterData.availableActions.Contains(actionName);
        if (!contained)
        {
            character.characterData.availableActions.Add(actionName);
            return true;
        }
        else
        {
            character.characterData.availableActions.Remove(actionName);
            return false;
        }
    }
}