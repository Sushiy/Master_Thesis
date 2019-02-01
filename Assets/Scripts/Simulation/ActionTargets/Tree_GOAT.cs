using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_GOAT : GameObjectActionTarget
{
    const int MAXGROWTHSTATE = 5;
    const float GROWTHTIME = 200f; // time it takes to increase growthstate
    //State of growth from 1-5
    int currentGrowthState = MAXGROWTHSTATE;

    Vector3 originalScale;

    float growthTimeAlpha = 0.0f;

    public int WoodAmount
    {
        get { return currentGrowthState; }
    }

    private void Start()
    {
        originalScale = transform.localScale;
        SetGrowthState((int)Random.Range(1, MAXGROWTHSTATE));
    }

    private void Update()
    {
        if (currentGrowthState != MAXGROWTHSTATE)
        {
            growthTimeAlpha += Time.deltaTime;
            if (growthTimeAlpha >= GROWTHTIME)
            {
                growthTimeAlpha = 0f;
                SetGrowthState(currentGrowthState + 1);
            }
        }
    }

    public override bool IsAvailable()
    {
        if(currentGrowthState == MAXGROWTHSTATE)
        {
            return true;
        }
        return false;
    }

    void SetGrowthState(int i)
    {
        currentGrowthState = i;
        transform.localScale = i * originalScale / MAXGROWTHSTATE;
    }

    public void CutDown()
    {
        currentGrowthState = 0;
        growthTimeAlpha = 0f;
    }
}
