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
    HashSet<GOAP_Action> availableActions;
    Queue<GOAP_Action> currentActions;

    public GOAP_Planner planner;
    [HideInInspector]
    public GOAP_Character character;

    public float planningWaitTimer = 2.0f;
    bool allowedToPlan = true;

    public GOAP_Quest postedQuest = null;

    void Awake()
    {
        //Load available actions
        availableActions = new HashSet<GOAP_Action>(gameObject.GetComponents<GOAP_Action>());
        currentActions = new Queue<GOAP_Action>();
        character = GetComponent<GOAP_Character>();

        HashSet<GOAP_Worldstate> goal1 = new HashSet<GOAP_Worldstate>();// = agent.getCurrentGoal();
        goal1.Add(new GOAP_Worldstate(WorldStateKey.bHasWood, true, null));
    }

    // Update is called once per frame
    void Update ()
    {
	    if(currentState == FSM_State.IDLE && allowedToPlan && postedQuest == null)
        {
            //Fetch a new Plan from the planner
            Queue<GOAP_Action> newPlan = planner.Plan(this, availableActions, FetchWorldState());
            if (newPlan != null)
            {
                //do what the plan says!
                currentActions = newPlan;
                currentState = FSM_State.PERFORMACTION;
            }
            else
            {
                //try again? or something...

            }
        }

        else if(currentState == FSM_State.PERFORMACTION)
        {
            GOAP_Action nextAction = null;
            //Use one of the currentActions
            if (currentActions.Count > 0)
            {
                nextAction = currentActions.Dequeue();

            }

            if(nextAction != null)
            {
                if(nextAction.RequiresInRange())
                {

                }
                nextAction.Run(this);
            }
            else
            {
                currentState = FSM_State.IDLE;
            }
        }

        else if(currentState == FSM_State.MOVETO)
        {
            //Move to the target!

        }
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

        return result;
    }
}
