using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Launch interactions
public class Interactable : MonoBehaviour
{
	[HideInInspector]
	public NavMeshAgent movingNavMeshAgent;

	[HideInInspector]
	public static bool hasInteracted;

	private float deffaultStoppingRadius;
	private float movingAgentRadius;

	private void Start()
	{
		deffaultStoppingRadius = GetComponent<NavMeshAgent>().radius;
	}

	void Update()
	{
		// TODO Calculate stopping distance or remaining distance depending on target object
		if (hasInteracted) { 
			Interact();
		}
	}

	public virtual void MoveToInteract(NavMeshAgent movingAgent)
	{
		movingNavMeshAgent = movingAgent;
		movingAgentRadius = movingNavMeshAgent.radius;
		movingNavMeshAgent.destination = transform.position;
	}

	public void Interact()
	{
		print("Interacting with base class.");
		// TODO Maybe wait to user or game action before resetting...
		StartCoroutine("WaitInteractionEnd");
	}

	IEnumerator WaitInteractionEnd()
	{
		yield return new WaitForSeconds(1);

		print("interaction done.");
		hasInteracted = false;
	}
}
