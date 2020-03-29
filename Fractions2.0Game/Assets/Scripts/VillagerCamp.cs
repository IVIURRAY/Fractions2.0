using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerCamp : MonoBehaviour
{
    [SerializeField]
    GameObject superVillager;

    [SerializeField]
    GameObject home;
    [SerializeField]
    GameObject foodStore;
    [SerializeField]
    GameObject woodStore;
    [SerializeField]
    GameObject stoneStore;

    [SerializeField]
    private int stoneCount = 0;
    [SerializeField]
    private int woodCount = 0;
    [SerializeField]
    private int foodCount = 0;
    private string newsUpdates = "Welcome to Camp!";

    private Spawner spawner;
    private float eatFoodTimer;
    private float notEnoughtFoodTime;
    private bool spawnedSuperVillager;


    private void Start()
    {
        spawner = GetComponent<Spawner>();
    }

    private void Update()
    {
        VillagersEatFood();
        EnoughStoneForSuperVillager();
    }

    private void EnoughStoneForSuperVillager()
    {
        if (GetStoneCount() % 2 == 0 && !spawnedSuperVillager)
        {
            AddNewsUpdate("Super Villager was born!");
            WanderingAI wanderingAi = superVillager.GetComponent<WanderingAI>();
            wanderingAi.SetCamp(this);

            spawner.SpawnPrefab(superVillager);
            spawnedSuperVillager = true;
        }
    }

    private void VillagersEatFood()
    {
        // If we don't have enough food, villagrs die.
        if (GetFoodCount() == 0)
        {
            notEnoughtFoodTime += Time.deltaTime;
            if (notEnoughtFoodTime > 5)
            {
                spawner.KillSpawnWithTag("Villager");
                notEnoughtFoodTime = 0;
                AddNewsUpdate("Not enough food. Villager died.");
            }
        }
        else
        {
            // Eat food periodically
            eatFoodTimer += Time.deltaTime;
            if (eatFoodTimer > 10)
            {
                RemoveFood();
                eatFoodTimer = 0;
                AddNewsUpdate("Villager eat food.");
            }
        }



    }

    private GameObject GetHome() {
        return home;
    }

    private GameObject GetFoodStore()
    {
        return foodStore;
    }

    private GameObject GetStoneStore()
    {
        return stoneStore;
    }

    private GameObject GetWoodStore()
    {
        return woodStore;
    }

    public GameObject GetResourcesHome(GameObject resource)
    {
        GameObject store = null;
        if (resource.CompareTag("WoodResource"))
            store = GetWoodStore();
        else if (resource.CompareTag("StoneResource"))
            store = GetStoneStore();
        else if (resource.CompareTag("FoodResource"))
            store = GetFoodStore();
        else
            GetHome();

        return store;
    }

    private void AddStone() {
        stoneCount += 1;
        spawnedSuperVillager = false;
    }

	private void AddWood() => woodCount += 1;
	private void AddFood() => foodCount += 1;

    private void RemoveStone() => stoneCount = Math.Max(stoneCount -= 1, 0);
    private void RemoveWood() => woodCount = Math.Max(woodCount -= 1, 0);
    private void RemoveFood() => foodCount = Math.Max(foodCount -= 1, 0);

    public int GetStoneCount() => stoneCount;
	public int GetWoodCount() => woodCount;
	public int GetFoodCount() => foodCount;
    public string GetNewsUpdates() => newsUpdates;

    private void AddNewsUpdate(string update)
    {
        newsUpdates = update + "\n" + newsUpdates;
    }

	internal void DepositResouce(GameObject resource)
	{
		if (resource.CompareTag("WoodResource"))
			AddWood();
		else if (resource.CompareTag("StoneResource"))
			AddStone();
		else if (resource.CompareTag("FoodResource"))
			AddFood();

		Destroy(resource);
	}
}
