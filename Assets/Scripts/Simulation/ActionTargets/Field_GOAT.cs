using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_GOAT : GameObjectActionTarget
{
    const int MAXGROWTHSTATE = 5;
    const float GROWTHTIME = 60f; // time it takes to increase growthstate
    //State of growth from 1-5
    int currentGrowthState = MAXGROWTHSTATE;

    float growthTimeAlpha = 0.0f;
    bool tendedTo = false;

    public bool IsAlreadyTendedTo
    {
        get
        {
            return tendedTo;
        }
    }
    private void Start()
    {
        currentGrowthState = Random.Range(1, MAXGROWTHSTATE);
    }

    private void Update()
    {
        if(currentGrowthState != MAXGROWTHSTATE)
        {
            growthTimeAlpha += Time.deltaTime * (tendedTo ? 2f : 1f);
            if (growthTimeAlpha >= GROWTHTIME)
            {
                growthTimeAlpha = 0f;
                currentGrowthState++;
                tendedTo = false;
            }
        }
    }

    public override bool IsAvailable()
    {
        if (currentGrowthState == MAXGROWTHSTATE)
        {
            return true;
        }
        return false;
    }

    public void TendToField()
    {
        if(tendedTo)
        {
            tendedTo = true;
        }
    }

    public void Harvest()
    {
        if (!IsAvailable()) return;

        tendedTo = false;
        currentGrowthState = 0;
        growthTimeAlpha = 0;
    }
}
