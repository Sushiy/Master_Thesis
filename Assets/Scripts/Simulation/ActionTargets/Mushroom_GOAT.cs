using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_GOAT : GameObjectActionTarget
{
    const int MAXGROWTHSTATE = 3;
    const float GROWTHTIME = 20f; // time it takes to increase growthstate
    float randGrowthTime;
    //State of growth from 1-3
    int currentGrowthState = MAXGROWTHSTATE;

    Vector3 originalScale;

    float growthTimeAlpha = 0.0f;

    private void Start()
    {
        originalScale = transform.localScale;
        SetGrowthState(1);
    }

    private void Update()
    {
        if (currentGrowthState != MAXGROWTHSTATE)
        {
            growthTimeAlpha += Time.deltaTime;
            if (growthTimeAlpha >= randGrowthTime)
            {
                growthTimeAlpha = 0f;
                SetGrowthState(currentGrowthState + 1);
            }
        }
    }

    public override bool IsAvailable()
    {
        if (currentGrowthState == MAXGROWTHSTATE)
        {
            return true;
        }
        return true;
    }

    void SetGrowthState(int i)
    {
        
        currentGrowthState = Mathf.Clamp(i, 1, MAXGROWTHSTATE);
        randGrowthTime = GROWTHTIME * Random.Range(0.5f, 1.5f);
        transform.localScale = currentGrowthState * originalScale / MAXGROWTHSTATE;
    }

    public void Gather()
    {
        SetGrowthState(1);
        Destroy(this);
    }
}
