using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Interactable : MonoBehaviour
{
	[HideInInspector]
	public NavMeshAgent movingNavMeshAgent;
	private bool hasInteracted;
	private float interactableRadius;
	private float movingAgentRadius;

	private void Start()
	{
		interactableRadius = GetComponent<NavMeshAgent>().radius;
	}

	public virtual void MoveToInteract(NavMeshAgent playerNavMeshAgent)
	{
		movingNavMeshAgent = playerNavMeshAgent;
		movingAgentRadius = movingNavMeshAgent.radius;
		movingNavMeshAgent.destination = transform.position;
	}

	void Update()
	{
		// TODO Calculate stopping distance or remaining distance depending on target object
		if (!hasInteracted && movingNavMeshAgent != null && !movingNavMeshAgent.pathPending) {
			movingNavMeshAgent.destination = transform.position;

			movingNavMeshAgent.stoppingDistance = movingAgentRadius*3 + interactableRadius*3;
			float remainingDistance = movingNavMeshAgent.remainingDistance - interactableRadius;
			Debug.Log("remaining : " + remainingDistance + " stopping : " + movingNavMeshAgent.stoppingDistance);
			if (remainingDistance <= movingNavMeshAgent.stoppingDistance) {
				// Reset the given path to stop the navmeshagent from trying to move again.
				movingNavMeshAgent.ResetPath();
				hasInteracted = true;
				Interact();
			}
		}

	}

	public virtual void Interact()
	{
		print("Interacting with base class.");
		// TODO Maybe wait to user or game action before resetting...
		StartCoroutine("WaitInteractionEnd");
	}

	IEnumerator WaitInteractionEnd()
	{
		yield return new WaitForSeconds(1);

		print("interaction done.");
		movingNavMeshAgent.stoppingDistance = 0;
		movingNavMeshAgent = null;
		hasInteracted = false;
	}

}
