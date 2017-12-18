using UnityEngine;
using UnityEngine.AI;
using Utility;
using FloatingBars;
using Rbots.Core;

namespace Rbots.Characters
{
	public class Player : MonoBehaviour, IDamageable
	{

		[SerializeField] FieldOfViewController myEye;

		[HideInInspector]
		public bool isAttacking = false;

		//=============================
		// HEALTH
		//=============================
		[SerializeField] bool ResetHP;

		FloatVariable HP;
		FloatVariable StartingHP;
		FloatVariable MinHP;

		Animator anim;
		NavMeshAgent playerNavMeshAgent;
		GameObject myTarget;
		GameObject enemyTarget;
		[HideInInspector]
		public static bool targetIsDead = false;

		//=============================
		// MOVEMENTS
		//=============================
		[SerializeField] Interactables interactableType;

		float defaultStoppingRadius;
		float attackRadius = 3;
		bool isMovingAgent = false;

		//=============================
		// INTERACTIONS
		//=============================
		RaycastHit interactionHit;
		[SerializeField] float damageAmount = 25f;

		void Start()
		{
			FloatingBarController fbc = GetComponentInChildren<FloatingBarController>();
			HP = fbc.resource;
			StartingHP = fbc.Max;
			MinHP = fbc.Min;

			if (ResetHP)
				HP.SetValue(StartingHP);

			anim = GetComponent<Animator>();
			playerNavMeshAgent = GetComponent<NavMeshAgent>();
			defaultStoppingRadius = playerNavMeshAgent.radius;
		}

		// Update is called once per frame
		void Update()
		{
			if (targetIsDead) {
				StopInteraction(enemyTarget);
				ResetAgentValues();
			}
			//=============================
			// INTERACTIONS - listen click
			//=============================
			if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
				GetInteraction();
			}

			//=============================
			// MOVEMENTS
			//=============================
			anim.SetFloat("PlayerVelocityX", Mathf.Abs(playerNavMeshAgent.velocity.x));
			myEye.Target = myTarget;

			if (myTarget != null) {
				float distanceToTarget = Vector3.Distance(playerNavMeshAgent.transform.position, myTarget.transform.position);

				if (!myEye.CanSeeTarget() && isAttacking && distanceToTarget <= attackRadius) {
					gameObject.transform.LookAt(myTarget.transform);
				} else if (myEye.CanSeeTarget()) {
					if (isAttacking)
						StopInteraction(myTarget);

					// if I see target, I begin chasing and move
					MoveToInteract(myTarget);
				}
			}

			if (isMovingAgent && !playerNavMeshAgent.pathPending) {
				// get the true remaining distance
				float remainingDistance = playerNavMeshAgent.remainingDistance > 0 ? Mathf.Abs(playerNavMeshAgent.remainingDistance - playerNavMeshAgent.radius) : 0;

				if (remainingDistance <= GetStoppingDistance()) {
					if (!isAttacking && myTarget != null) {
						// launch interaction and stop movements
						ResetAgentValues();
						Interactable.movingNavMeshAgent = playerNavMeshAgent;
						Interactable.hasInteracted = true;
						HandleInteraction(myTarget);
					}
				} else {
					if (myTarget != null)
						playerNavMeshAgent.destination = myTarget.transform.position;
				}

				if (isMovingAgent) {
					StopInteraction(myTarget);
				}
			}
		}

		//=============================
		// INTERACTIONS
		//=============================
		void GetInteraction()
		{
			if (targetIsDead)
				targetIsDead = false;

			Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(interactionRay, out interactionHit)) {
				GameObject interactedObject = interactionHit.collider.gameObject;
				if (interactedObject.tag == "Interactable") {
					MoveToInteract(interactedObject);
				} else {
					MoveToInteract(interactionHit.point);
				}
			}
		}

		public void MoveToInteract(GameObject target)
		{
			isMovingAgent = true;
			playerNavMeshAgent.destination = target.transform.position;
			myTarget = target;
		}
		public void MoveToInteract(Vector3 interactionPoint)
		{
			isMovingAgent = true;
			playerNavMeshAgent.destination = interactionPoint;
			myTarget = null;
		}

		public void HandleInteraction(GameObject target)
		{
			myTarget = target;
			myEye.Target = myTarget;

			if (target.GetComponent<EnemyController>()) {
				enemyTarget = target;
				isAttacking = true;
				anim.SetBool("IsAttacking", isAttacking);
			}
		}

		public void StopInteraction(GameObject target)
		{
			myTarget = target;
			isAttacking = false;
			anim.SetBool("IsAttacking", isAttacking);
		}

		void DealDamage()
		{
			Component damageableComponent = myTarget.GetComponent(typeof(IDamageable));

			if (damageableComponent) {
				(damageableComponent as IDamageable).TakeDamage(damageAmount);
			}
		}

		//=============================
		// HEALTH - Damages
		//=============================
		public void TakeDamage(float amount)
		{
			HP.ApplyChange(-amount);

			if (HP.Value <= MinHP.Value) {
				//TODO Die()
			}
		}

		//=====================
		// MOVEMENT
		//=====================
		// TODO Extract this !
		public float GetStoppingDistance()
		{
			float additionalStoppingDistance;

			switch (interactableType) {
				case Interactables.Player:
					additionalStoppingDistance = 0;
					break;
				case Interactables.GroundEnemy:
				case Interactables.FlyingEnemy:
					additionalStoppingDistance = 0;
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
			if (myTarget != null)
				return playerNavMeshAgent.stoppingDistance = playerNavMeshAgent.radius + myTarget.GetComponent<NavMeshAgent>().radius + additionalStoppingDistance;
			else
				return 0;
		}

		public void ResetAgentValues()
		{
			// Reset path stops the movement
			isMovingAgent = false;
			playerNavMeshAgent.ResetPath();
			playerNavMeshAgent.stoppingDistance = defaultStoppingRadius;
		}
	}
}
