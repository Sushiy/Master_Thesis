using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum FSM_State
{
    IDLE,
    PERFORMACTION,
    MOVETO
}

public class GOAP_Agent : MonoBehaviour
{
    FSM_State currentState = FSM_State.IDLE;

    HashSet<GOAP_Worldstate> standardGoal;
    public HashSet<GOAP_Worldstate> goal;

    HashSet<GOAP_Action> availableActions;
    Queue<GOAP_Action> currentActions;
    GOAP_Action activeAction;
    
    [HideInInspector]
    public GOAP_Character character;

    public float planningWaitTimer = 2.0f;
    bool allowedToPlan = true;
    bool actionCompleted = true;

    public GOAP_Quest postedQuest = null;
    public GOAP_Quest activeQuest = null;

    void Awake()
    {
        //Load available actions
        availableActions = new HashSet<GOAP_Action>(gameObject.GetComponents<GOAP_Action>());
        currentActions = new Queue<GOAP_Action>();
        character = GetComponent<GOAP_Character>();

        standardGoal = new HashSet<GOAP_Worldstate>(); // TODO: implement proper goals for agents; => agent.getCurrentGoal();
        standardGoal.Add(new GOAP_Worldstate(WorldStateKey.bHasWood, true, null)); //TODO: remove this when I have proper goals
        goal = standardGoal;
    }

    // Update is called once per frame
    void Update ()
    {
	    if(currentState == FSM_State.IDLE && allowedToPlan && postedQuest == null)
        {
            activeQuest = CheckForQuests();
            if (activeQuest == null) goal = standardGoal;
            else goal = activeQuest.RequiredStates;

            //Fetch a new Plan from the planner
            Queue<GOAP_Action> newPlan = GOAP_Planner.instance.Plan(this, goal, availableActions, FetchWorldState());
            if (newPlan != null)
            {
                //do what the plan says!
                currentActions = newPlan;
                currentState = FSM_State.PERFORMACTION;
            }
            else
            {
                //try again? or something...
                Debug.Log("No plan?");
                StartCoroutine(DelayPlanning());
            }
        }

        else if(currentState == FSM_State.PERFORMACTION)
        {
            //Use one of the currentActions
            if (actionCompleted)
            {
                if(currentActions.Count > 0)
                {
                    activeAction = currentActions.Dequeue();

                }
                else
                {
                    if(activeQuest != null)
                    {
                        Debug.Log("<color=#0000cc>" + character.characterName + "</color> completed Quest " + activeQuest.id);

                        activeQuest.Complete();
                        activeQuest = null;
                    }
                    activeAction = null;
                }

            }

            if (activeAction != null)
            {
                if(!activeAction.IsInRange(this))
                {
                    currentState = FSM_State.MOVETO;
                }
                else
                {
                    actionCompleted = activeAction.Run(this);
                }
            }
            else
            {
                currentState = FSM_State.IDLE;
            }
        }

        else if(currentState == FSM_State.MOVETO)
        {
            //Move to the target!
            if(!activeAction.IsInRange(this))
            {
                transform.position += (activeAction.ActionTarget.transform.position - transform.position).normalized * 3.0f * Time.deltaTime;
            }
            else
            {
                currentState = FSM_State.PERFORMACTION;
            }
        }
	}

    public GOAP_Quest CheckForQuests()
    {
        GOAP_Quest result = null;
        foreach(GOAP_Quest quest in GOAP_QuestBoard.instance.quests)
        {
            if (quest.Owner != this)
            {
                result = quest;
                break;
            }
        }
        if(result != null)
        {
            Debug.Log("<color=#0000cc>" + character.characterName + "</color> chose Quest " + result.id);
            GOAP_QuestBoard.instance.ChooseQuest(result);
        }
        return result;
    }

    private IEnumerator DelayPlanning()
    {
        allowedToPlan = false;
        yield return new WaitForSeconds(planningWaitTimer);
        allowedToPlan = true;
    }

    private HashSet<GOAP_Worldstate> FetchWorldState()
    {
        HashSet<GOAP_Worldstate> result = new HashSet<GOAP_Worldstate>();
        if(activeQuest != null)
        {
            foreach(GOAP_Worldstate state in activeQuest.ProvidedStates)
            {
                result.Add(state);
            }
        }

        return result;
    }
}
