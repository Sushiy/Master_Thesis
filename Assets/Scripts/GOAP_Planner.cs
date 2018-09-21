using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Planner : MonoBehaviour
{
    private List<GOAP_Action> closedStarts;
	// Use this for initialization
	void Awake()
    {
        closedStarts = new List<GOAP_Action>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, HashSet<GOAP_Action> availableActions, HashSet<GOAP_Worldstate> currentWorldState)
    {

        int actionCount = availableActions.Count;
        Queue<GOAP_Action> plan = new Queue<GOAP_Action>();

        /***** GET ACTIVE GOAL AND FIND SUITABLE STARTACTIONS *****/

        //Get the goal from agent
        HashSet<GOAP_Worldstate> goal = new HashSet<GOAP_Worldstate>();// = agent.getCurrentGoal();

        GOAP_Action start = null;

        foreach (GOAP_Action action in availableActions)
        {
            if(CompareWorldStates(action.SatisfyWorldStates, goal))
            {
                if(!closedStarts.Contains(action))
                {
                    start = action;
                    closedStarts.Add(start);
                    break;
                }
            }

        }

        if(start == null)
        {
            //TODO: needs goals name or something to identify it
            Debug.Log("There is no action available to satisfy goal:");
            return null;
        }

        /***** IMPLEMENT A* fOR ACTIONS *****/

        //closest neigbour for each considered Action
        List<GOAP_Action> bestNeighbourMap = new List<GOAP_Action>();

        //Actions already considered
        List<GOAP_Action> closedSet = new List<GOAP_Action>();
        //Actions not yet considered
        List<GOAP_Action> openSet = new List<GOAP_Action>(availableActions);

        //For each Action in the availableActions set:
        //the cost to get to this Action from the Start
        float[] gScore = new float[actionCount];
        gScore.SetValue(Mathf.Infinity, 0, actionCount-1);

        //TODO: we are missing the index of the first Action


        //the cost to go to the goal through this Action
        float[] fScore = new float[actionCount];
        fScore.SetValue(Mathf.Infinity, 0, actionCount - 1);




        //Return null if you couldn't find a plan!
        return plan;
    }

    //Compare these Worldstates to determine if they match up
    private bool CompareWorldStates(HashSet<GOAP_Worldstate> A, HashSet<GOAP_Worldstate> B)
    {
        return A.SetEquals(B);
    }
}
