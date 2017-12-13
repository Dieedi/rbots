using UnityEngine;
using UnityEngine.AI;
using Utility;

// control moving agent special behavior
[RequireComponent (typeof(NavMeshAgent))]
public class MovingAgentController : MonoBehaviour
{
	public Interactables interactableType;

	private float movingAgentRadius;
	private float defaultStoppingRadius;
	private bool isMovingAgent = false;

	// I AM THE MOVING AGENT !!!!
	private NavMeshAgent myAgent;
	private GameObject targetAgent;
	private Player player;
	private EnemyController enemy;

	private void Start()
	{
		myAgent = GetComponent<NavMeshAgent>();
		defaultStoppingRadius = myAgent.radius;

		player = myAgent.GetComponent<Player>();
		enemy = myAgent.GetComponent<EnemyController>();
	}

	private void Update()
	{
		if (isMovingAgent && !myAgent.pathPending) {
			Debug.Log("should appen if agent move to destination");
			// get the true remaining distance
			float remainingDistance = myAgent.remainingDistance - myAgent.radius;

			if (remainingDistance <= GetStoppingDistance()) {
				if (interactableType == Interactables.GroundEnemy) {
					// don't reset values, continue chasing until die ! 
				} else {
					ResetAgentValues();
					isMovingAgent = false;
					Interactable.movingNavMeshAgent = myAgent;
					Interactable.hasInteracted = true;
				}

				if (player) {
					player.HandleInteraction(targetAgent);
				}

				if (enemy) {
					enemy.HandleInteraction(targetAgent);
				}
			}
		}
	}

	public void MoveToInteract(GameObject target)
	{
		isMovingAgent = true;
		myAgent.destination = target.transform.position;
		targetAgent = target;
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
				return 0;
		}

		// return updated radius depending on target type ?
		return myAgent.stoppingDistance = movingAgentRadius * 3 + myAgent.radius * 3 + additionalStoppingDistance;
	}

	public void ResetAgentValues()
	{
		// Reset path stops the movement
		isMovingAgent = false;
		myAgent.ResetPath();
		myAgent.stoppingDistance = defaultStoppingRadius;
	}
}
