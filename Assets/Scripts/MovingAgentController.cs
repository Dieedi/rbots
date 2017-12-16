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
			// get the true remaining distance
			float remainingDistance = myAgent.remainingDistance > 0 ? Mathf.Abs(myAgent.remainingDistance - myAgent.radius) : 0;
			
			if (remainingDistance <= GetStoppingDistance()) {
				if (player) {
					player.HandleInteraction(targetAgent);
				}

				if (enemy) {
					if (!enemy.isAttacking) {
						// launch interaction and stop movements
						ResetAgentValues();
						isMovingAgent = false;
						Interactable.movingNavMeshAgent = myAgent;
						Interactable.hasInteracted = true;
						enemy.HandleInteraction(targetAgent);
					}

					if (isMovingAgent)
						enemy.StopInteraction(targetAgent);
				}
			}
		}
		//else {
		//	if (enemy)
		//		enemy.StopInteraction(targetAgent);
		//}
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
			case Interactables.Player:
				additionalStoppingDistance = 0;
				break;
			case Interactables.GroundEnemy:
			case Interactables.FlyingEnemy:
				additionalStoppingDistance = enemy.attackRadius - 1;
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
		return myAgent.stoppingDistance = movingAgentRadius + myAgent.radius + additionalStoppingDistance;
	}

	public void ResetAgentValues()
	{
		// Reset path stops the movement
		isMovingAgent = false;
		myAgent.ResetPath();
		myAgent.stoppingDistance = defaultStoppingRadius;
	}
}
