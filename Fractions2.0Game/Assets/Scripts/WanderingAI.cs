using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderingAI : MonoBehaviour
{

	public float wanderRadius;
	public float wanderTimer;
    public float wanderSpeed;

	private NavMeshAgent agent;
	private List<GameObject> resources = new List<GameObject>();
	private float timer;
    [SerializeField]
    private bool walkingToLastKnownResource;
    [SerializeField]
    private bool walkingHome;
	private Transform holdingArea;
	private GameObject holdingResource;
    private Vector3 lastKnownResourcePosition;
	public VillagerCamp villagerCamp;
	private Animator animator;


	public void SetCamp(VillagerCamp camp) {
		villagerCamp = camp;
	}

    // Use this for initialization
    void Start()
	{
        agent = GetComponent<NavMeshAgent>();
        agent.speed = wanderSpeed;
        timer = wanderTimer;
		walkingHome = false;
        walkingToLastKnownResource = false;
        holdingArea = gameObject.transform.Find("HoldingArea");
		resources = FindAllResources();
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update()
	{
		if (walkingHome)
			WalkHome();

        if (holdingResource)
			HoldResource(holdingResource);

        if (walkingToLastKnownResource)
            WalkToLastKnownResourceLocation();
        
        ForageForResources();
	}

    private void WalkToLastKnownResourceLocation()
    {
        agent.SetDestination(lastKnownResourcePosition);
        walkingToLastKnownResource = Vector3.Distance(transform.position, lastKnownResourcePosition) >= 1;
    }

    private void ForageForResources()
	{
		timer += Time.deltaTime;

		if (timer >= wanderTimer && !walkingHome && !walkingToLastKnownResource)
		{
			if (IsCloseToResource())
			{
				MoveToResource(FindClosestResource());
			}
			else
			{
				MoveToRandomLocation();
			}
		}
	}

	private List<GameObject> FindAllResources() 
	{
		GameObject[] wood = GameObject.FindGameObjectsWithTag("WoodResource");
		GameObject[] stone = GameObject.FindGameObjectsWithTag("StoneResource");
		GameObject[] food = GameObject.FindGameObjectsWithTag("FoodResource");

		resources.AddRange(wood);
		resources.AddRange(stone);
		resources.AddRange(food);

		return resources;

	}

	private void WalkHome()
	{
		float distanceToHome = Vector3.Distance(transform.position, villagerCamp.GetResourcesHome(holdingResource).transform.position);
		if (distanceToHome < 1)
		{
            // Deposit resouce in camp
            villagerCamp.DepositResouce(holdingResource);
            resources.Remove(holdingResource);

            // Tidy up state
            walkingHome = false;
            holdingResource = null;
            walkingToLastKnownResource = true;

            animator.SetBool("IsCarrying", false);
		}

	}

	private void MoveToResource(GameObject resource)
	{
		float distance = Vector3.Distance(transform.position, resource.transform.position);
		agent.SetDestination(resource.transform.position);
		if (distance < 1)
		{
            PlayPickupSound();
			TakeResourceToCamp(resource);
		}
	}

	private static void PlayPickupSound()
	{
		List<string> sounds = new List<string>(new string[] { "Ohhh", "Whoopie", "Yeahh" });
        FindObjectOfType<AudioManager>().Play(sounds[UnityEngine.Random.Range(0, sounds.Count)]);
	}

	private void TakeResourceToCamp(GameObject resource)
	{
        lastKnownResourcePosition = resource.transform.position;
        walkingHome = true;
		HoldResource(resource);
		agent.SetDestination(villagerCamp.GetResourcesHome(holdingResource).transform.position);
	}

	private void HoldResource(GameObject resource)
	{
        Resource res = resource.GetComponent<Resource>();
		res.IsAqurired = true;
		animator.SetBool("IsCarrying", true);
		holdingResource = resource;
		resource.transform.position = holdingArea.position;
	}

	private bool IsCloseToResource()
	{
		GameObject closest = FindClosestResource();
        if (!closest || closest.GetComponent<Resource>().IsAqurired)
			return false; // Dont move to resouce if someone has it

		if (Vector3.Distance(FindClosestResource().transform.position, transform.position) < 4)
			return true;

		return false;
	}

	private GameObject FindClosestResource() {
		float distance = Mathf.Infinity;
		GameObject closest = null;

		foreach (GameObject resource in resources)
		{
			if (resource != null)
			{
				float distanceToResource = Vector3.Distance(resource.gameObject.transform.position, transform.position);
				if (distanceToResource < distance)
				{
					distance = distanceToResource;
					closest = resource;
				}
			}
		}

		return closest;
	}

	private void MoveToRandomLocation()
	{
		Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
		agent.SetDestination(newPos);
		timer = 0;
	}

	public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
	{
		Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

		randDirection += origin;

		NavMeshHit navHit;

		NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

		return navHit.position;
	}
}
