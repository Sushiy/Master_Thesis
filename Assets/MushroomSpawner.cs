using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomSpawner : MonoBehaviour
{
    public GameObject[] mushroomPrefabs;

    public float mushroomStandardSize;
    public float spawnRadius = 5f;
    public float spawnInterval = 2.0f;
    float spawnalpha = 0.0f;
    public int maxSpawnCount = 3;
	
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
        }
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.4f, 0.3f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    void SpawnRandomMushrooms()
    {
        GameObject prefab = mushroomPrefabs[Random.Range(0, mushroomPrefabs.Length - 1)];

        Vector3 offset = Random.insideUnitCircle * spawnRadius;
        offset.z = offset.y;
        offset.y = 0;

        Instantiate(prefab, transform.position + offset, Quaternion.Euler(0, Random.Range(0, 359), 0),transform);
    }
}
