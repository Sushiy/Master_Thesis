using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBlackBoard : MonoBehaviour
{
    public static InfoBlackBoard instance;

    public Transform[] chopTreeLocations;
    public Transform[] resourceBuyLocations;
    public Transform[] woodWorkshopLocations;

    // Use this for initialization
    void Awake()
    {
        instance = this;	
	}

    public Transform FindClosest(Transform[] transforms, Vector3 position)
    {
        Transform closest = null;
        float minDistance = Mathf.Infinity;
        foreach(Transform t in transform)
        {
            float distance = Vector3.Distance(t.position, position);
            if (distance < minDistance)
            {
                closest = t;
                minDistance = distance;
            }
        }
        return closest;
    }

    public float DistanceToClosest(Transform[] transforms, Vector3 position)
    {
        return Vector3.Distance(FindClosest(transforms,position).position, position);
    }
}
