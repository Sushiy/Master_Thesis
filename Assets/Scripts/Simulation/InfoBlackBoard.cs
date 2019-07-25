using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBlackBoard : MonoBehaviour
{

    public static InfoBlackBoard instance;

    public GameObjectActionTarget campFireLocation;

    [Header("ActionLocations")]
    public GameObjectActionTarget questBoardLocation;
    [Space]
    public GameObjectActionTarget[] chopTreeLocations;
    public GameObjectActionTarget[] buyResourceLocations;
    public GameObjectActionTarget[] woodWorkshopLocations;
    public GameObjectActionTarget[] smithingWorkshopLocations;
    public GameObjectActionTarget[] mineIronLocations;
    public GameObjectActionTarget[] farmingLocations;
    public GameObjectActionTarget[] millingLocations;
    public GameObjectActionTarget[] gatherWoodLocations;


    // Use this for initialization
    void Awake()
    {
        GOAP_Planner.Init();
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public IActionTarget FindClosest(IActionTarget[] transforms, Vector3 position)
    {
        IActionTarget closest = null;
        float minDistance = Mathf.Infinity;
        foreach(IActionTarget t in transforms)
        {
            if (t == null) continue;
            float distance = Vector3.Distance(t.GetPosition(), position);
            if (distance < minDistance)
            {
                closest = t;
                minDistance = distance;
            }
        }
        return closest;
    }

    public IActionTarget FindClosestAvailable(IActionTarget[] transforms, Vector3 position)
    {
        IActionTarget closest = null;
        float minDistance = Mathf.Infinity;
        foreach (IActionTarget t in transforms)
        {
            if (t == null|| !t.IsAvailable()) continue;
            float distance = Vector3.Distance(t.GetPosition(), position);
            if (distance < minDistance)
            {
                closest = t;
                minDistance = distance;
            }
        }
        return closest;
    }

    private float DistanceToClosest(IActionTarget[] transforms, Vector3 position)
    {
        return Vector3.Distance(FindClosest(transforms,position).GetPosition(), position);
    }
}
