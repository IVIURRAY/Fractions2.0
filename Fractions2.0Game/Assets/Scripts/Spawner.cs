using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	// Instantiates prefabs in a circle formation
	public GameObject prefab;
	public int numberOfObjects = 20;
	public float radius = 5f;

    private List<GameObject> spawns = new List<GameObject>();

	void Start()
	{
		for (int i = 0; i < numberOfObjects; i++)
		{
			float angle = i * Mathf.PI * 2 / numberOfObjects;
			float x = Mathf.Cos(angle) * radius;
			float z = Mathf.Sin(angle) * radius;
			Vector3 pos = transform.position + new Vector3(x, 0, z);
			float angleDegrees = -angle * Mathf.Rad2Deg;
			Quaternion rot = Quaternion.Euler(0, angleDegrees, 0);

			// Set the AI home
			WanderingAI wanderingAi = prefab.GetComponent<WanderingAI>();
			VillagerCamp villagerCamp = GetComponent<VillagerCamp>();
			if (wanderingAi)
				wanderingAi.SetCamp(villagerCamp);

            spawns.Add(Instantiate(prefab, pos, rot));
		}
	}

    public void KillSpawnWithTag(string tag)
    {
        GameObject found = spawns.Find(go => go.CompareTag(tag));
        if (found)
            spawns.Remove(found);
            Destroy(found);
    }

    public void SpawnPrefab(GameObject go)
    {
        spawns.Add(Instantiate(go, transform.position, transform.rotation));
    }
}
