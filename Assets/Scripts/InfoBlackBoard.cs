using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBlackBoard : MonoBehaviour
{
    public enum LOCATIONS
    {
        CHOPTREE,
        BUYRESOURCE,
        WOODWORKSHOP,
        SMITHWORKSHOP
    }

    public static InfoBlackBoard instance;

    [Header("ActionLocations")]
    public GameObjectActionTarget questBoardLocation;
    [Space]
    public GameObjectActionTarget[] chopTreeLocations;
    public GameObjectActionTarget[] buyResourceLocations;
    public GameObjectActionTarget[] woodWorkshopLocations;
    public GameObjectActionTarget[] smithingWorkshopLocations;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public IActionTarget FindClosest(LOCATIONS location, Vector3 position)
    {
        switch(location)
        {
            case LOCATIONS.CHOPTREE:
            {
                return FindClosest(chopTreeLocations, position);
            }
            case LOCATIONS.BUYRESOURCE:
            {
                return FindClosest(buyResourceLocations, position);
            }
            case LOCATIONS.WOODWORKSHOP:
            {
                return FindClosest(woodWorkshopLocations, position);
            }
            case LOCATIONS.SMITHWORKSHOP:
            {
                return FindClosest(smithingWorkshopLocations, position);
            }
            default:
                return null;
        }
    }

    private IActionTarget FindClosest(IActionTarget[] transforms, Vector3 position)
    {
        IActionTarget closest = null;
        float minDistance = Mathf.Infinity;
        foreach(IActionTarget t in transforms)
        {
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
