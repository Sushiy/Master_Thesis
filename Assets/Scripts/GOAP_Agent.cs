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

    GOAP_Planner planner;

    void Awake()
    {
        availableActions = new HashSet<GOAP_Action>();
        currentActions = new Queue<GOAP_Action>();

        //Load available actions
        GOAP_Action[] actionsArray = gameObject.GetComponents<GOAP_Action>();
        foreach(GOAP_Action action in actionsArray)
        {
            availableActions.Add(action);
        }
    }

	// Update is called once per frame
	void Update ()
    {
	    if(currentState == FSM_State.IDLE)
        {
            //Fetch a new Plan from the planner
            Queue<GOAP_Action> newPlan = planner.Plan(this, availableActions, FetchWorldState());
            if (newPlan != null)
            {
                //do what the plan says!

            }
            else
            {
                //try again? or something...
            }
        }

        else if(currentState == FSM_State.PERFORMACTION)
        {
            //Use one of the currentActions
            GOAP_Action nextAction = currentActions.Peek();
            if(nextAction != null)
            {
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

    private HashSet<KeyValuePair<string, object>> FetchWorldState()
    {
        HashSet<KeyValuePair<string, object>> result = new HashSet<KeyValuePair<string, object>>();

        return result;
    }
}
