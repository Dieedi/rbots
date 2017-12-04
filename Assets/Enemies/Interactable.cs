using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Interactable : MonoBehaviour
{
	// TODO Scriptable ?
	public enum Interactables
	{
		GroundEnemy,
		FlyingEnemy,
		Object,
		NPC
	}

	public Interactables interactableType;
	[HideInInspector]
	public NavMeshAgent movingNavMeshAgent;

	private bool hasInteracted;
	private float deffaultStoppingRadius;
	private float movingAgentRadius;

	private void Start()
	{
		deffaultStoppingRadius = GetComponent<NavMeshAgent>().radius;
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
			
			float remainingDistance = movingNavMeshAgent.remainingDistance - deffaultStoppingRadius;

			if (remainingDistance <= GetStoppingDistance()) {
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

	float GetStoppingDistance()
	{
		float additionalStoppingDistance;

		switch (interactableType) {
			case Interactables.GroundEnemy:
				additionalStoppingDistance = 0;
				break;
			case Interactables.FlyingEnemy:
				additionalStoppingDistance = 0.5f;
				break;
			case Interactables.Object:
				additionalStoppingDistance = 0.5f;
				break;
			case Interactables.NPC:
				additionalStoppingDistance = 0.5f;
				break;
			default:
				additionalStoppingDistance = 0;
				break;
		}

		return movingNavMeshAgent.stoppingDistance = movingAgentRadius * 3 + deffaultStoppingRadius * 3 + additionalStoppingDistance;
	}
}
