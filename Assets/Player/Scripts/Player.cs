using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Utility;

public class Player : MonoBehaviour, IDamageable {

	[SerializeField] FieldOfViewController myEye;

	public bool ResetHP;
	public FloatVariable RegenRate;

	[HideInInspector]
	public bool isAttacking = false;

	private FloatVariable HP;
	private FloatVariable StartingHP;
	private FloatVariable MinHP;

	private bool regenerating = false;
	private Animator anim;
	NavMeshAgent playerNavMeshAgent;
	private GameObject myTarget;
	private GameObject enemyTarget;
	private float attackRadius = 3;

	//=============================
	// MOVEMENTS
	//=============================
	public Interactables interactableType;
	
	private float defaultStoppingRadius;
	private bool isMovingAgent = false;

	// I AM THE MOVING AGENT !!!!
	private GameObject targetAgent;
	private Player player;
	private EnemyController enemy;

	//=============================
	// INTERACTIONS
	//=============================
	RaycastHit interactionHit;

	
	void Start () {
		FloatingBarController fbc = GetComponentInChildren<FloatingBarController>();
		HP = fbc.resource;
		StartingHP = fbc.Max;
		MinHP = fbc.Min;

		if (ResetHP)
			HP.SetValue(StartingHP);

		anim = GetComponent<Animator>();
		playerNavMeshAgent = GetComponent<NavMeshAgent>();
		defaultStoppingRadius = playerNavMeshAgent.radius;

		//=============================
		// INTERACTIONS
		//=============================
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
			GetInteraction();
		}
		anim.SetFloat("PlayerVelocityX", Mathf.Abs(playerNavMeshAgent.velocity.x));

		if (HP.Value < StartingHP.Value && !regenerating) {
			regenerating = true;
			StartCoroutine("RegenHP");
		}

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
				if (!isAttacking && targetAgent != null) {
					// launch interaction and stop movements
					ResetAgentValues();
					Interactable.movingNavMeshAgent = playerNavMeshAgent;
					Interactable.hasInteracted = true;
					HandleInteraction(targetAgent);
				}
			} else {
				if (targetAgent != null)
					playerNavMeshAgent.destination = targetAgent.transform.position;
			}

			if (isMovingAgent) {
				StopInteraction(targetAgent);
			}
		}
	}

	//=============================
	// INTERACTIONS
	//=============================
	void GetInteraction()
	{
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
		targetAgent = target;
	}
	public void MoveToInteract(Vector3 interactionPoint)
	{
		isMovingAgent = true;
		playerNavMeshAgent.destination = interactionPoint;
		targetAgent = null;
	}

	public void HandleInteraction(GameObject target)
	{
		Debug.Log("handle interact");
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

	IEnumerator RegenHP ()
	{
		while (HP.Value < StartingHP.Value) {
			yield return new WaitForSeconds(1f);

			if (HP.Value >= StartingHP.Value)
				regenerating = false;
			else
				HP.ApplyChange(RegenRate.Value);
		}
	}

	public void TakeDamage(float amount)
	{
		float newValue = HP.Value - amount;
		HP.ApplyChange(-amount);
	}

	//=====================
	// MOVEMENT
	//=====================
	public float GetStoppingDistance()
	{
		float additionalStoppingDistance;

		switch (interactableType) {
			case Interactables.Player:
				additionalStoppingDistance = 0;
				break;
			case Interactables.GroundEnemy:
			case Interactables.FlyingEnemy:
				additionalStoppingDistance = enemy.attackRadius;
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
		return playerNavMeshAgent.stoppingDistance = playerNavMeshAgent.radius + additionalStoppingDistance;
	}

	public void ResetAgentValues()
	{
		// Reset path stops the movement
		isMovingAgent = false;
		playerNavMeshAgent.ResetPath();
		playerNavMeshAgent.stoppingDistance = defaultStoppingRadius;
	}
}
