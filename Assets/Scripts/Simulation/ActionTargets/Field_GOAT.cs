using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_GOAT : GameObjectActionTarget
{
    const int MAXGROWTHSTATE = 5;
    public const float GROWTHTIME = 30f; // time it takes to increase growthstate
    //State of growth from 1-5
    public int currentGrowthState = MAXGROWTHSTATE;

    public float growthTimeAlpha = 0.0f;
    bool tendedTo = false;

    public MeshRenderer[] plantRenderers;
    public Gradient plantColor;

    public bool IsAlreadyTendedTo
    {
        get
        {
            return tendedTo;
        }
    }
    private void Start()
    {
        SetGrowthState(Random.Range(1, MAXGROWTHSTATE));
    }

    private void Update()
    {
        if(currentGrowthState != MAXGROWTHSTATE)
        {
            growthTimeAlpha += Time.deltaTime * (tendedTo ? 2f : 1f);
            if (growthTimeAlpha >= GROWTHTIME)
            {
                growthTimeAlpha = 0f;
                SetGrowthState(currentGrowthState+1);
                tendedTo = false;
            }
        }
    }

    void SetGrowthState(int state)
    {
        currentGrowthState = Mathf.Clamp(state, 1, MAXGROWTHSTATE);
        float alpha = (float)currentGrowthState / (float)MAXGROWTHSTATE;
        for (int i = 0; i < plantRenderers.Length; i++)
        {
            plantRenderers[i].material.color = plantColor.Evaluate(alpha);
            plantRenderers[i].transform.localScale = new Vector3(0.7f, alpha * 0.7f, 0.7f);
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
        if(!tendedTo)
        {
            tendedTo = true;
        }
    }

    public void Harvest()
    {
        if (!IsAvailable()) return;

        tendedTo = false;
        SetGrowthState(0);
        growthTimeAlpha = 0;
    }
}
