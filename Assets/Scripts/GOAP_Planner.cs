using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAP_Planner : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public Queue<GOAP_Action> Plan(GOAP_Agent agent, HashSet<GOAP_Action> availableActions, HashSet<KeyValuePair<string, object>> currentWorldState)
    {

        Queue<GOAP_Action> plan = new Queue<GOAP_Action>();

        //Return null if you couldn't find a plan!
        return plan;
    }
}
