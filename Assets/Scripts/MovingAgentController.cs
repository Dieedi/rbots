using UnityEngine;
using UnityEngine.AI;
using Utility;

// control moving agent special behavior
[RequireComponent (typeof(NavMeshAgent))]
public class MovingAgentController : MonoBehaviour
{
	public Interactables interactableType;

	private float movingAgentRadius;

	// I AM THE MOVING AGENT !!!!
	private NavMeshAgent myAgent;

	private void Start()
	{
		myAgent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{
		if (!myAgent.pathPending) {
			// get the true remaining distance
			float remainingDistance = myAgent.remainingDistance - myAgent.radius;

			if (remainingDistance <= GetStoppingDistance()) {
				// Reset path stops the movement
				myAgent.ResetPath();
				Interactable.hasInteracted = true;
			}
		}
	}

	public float GetStoppingDistance()
	{
		float additionalStoppingDistance;

		switch (interactableType) {
			case Interactables.GroundEnemy:
			case Interactables.Player:
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

		// return updated radius depending on target type ?
		return myAgent.stoppingDistance = movingAgentRadius * 3 + myAgent.radius * 3 + additionalStoppingDistance;
	}
}
