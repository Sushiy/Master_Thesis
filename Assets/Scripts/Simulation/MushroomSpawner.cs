using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSpawner : GameObjectActionTarget
{
    public GameObject[] mushroomPrefabs;

    public float mushroomStandardSize;
    public float spawnRadius = 5f;
    public float spawnInterval = 2.0f;
    float spawnalpha = 0.0f;
    public int maxSpawnCount = 3;

    private void Start()
    {
        SpawnRandomMushrooms();
    }
    // Update is called once per frame
    void Update ()
    {
        spawnalpha += Time.deltaTime;
        if(spawnalpha >= spawnInterval)
        {
            for(int i = 0; i < Random.Range(1, maxSpawnCount); i++)
            {
                SpawnRandomMushrooms();
            }
            spawnalpha = 0;
        }
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.3f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    void SpawnRandomMushrooms()
    {
        if (mushroomPrefabs.Length == 0) return;
        GameObject prefab = mushroomPrefabs[Random.Range(0, mushroomPrefabs.Length - 1)];

        Vector3 offset = Random.insideUnitCircle * spawnRadius;
        offset.z = offset.y;
        offset.y = 0;

        Instantiate(prefab, transform.position + offset, Quaternion.Euler(0, Random.Range(0, 359), 0), transform);

    }

    public override bool IsAvailable()
    {
        int count = 0;
        for(int i = 0; i < transform.childCount; i++)
        {
            Mushroom_GOAT m = transform.GetChild(i).GetComponent<Mushroom_GOAT>();
            if (m.IsAvailable())
            {
                count++;
            }
        }

        return count >= 3;
    }

    public void Gather()
    {
        int count = 3;
        for (int i = transform.childCount - 1; i >= 0 ; i--)
        {
            Mushroom_GOAT m = transform.GetChild(i).GetComponent<Mushroom_GOAT>();
            if (m.IsAvailable())
            {
                m.Gather();
                if (count == 0) break;
            }
        }
    }
}
