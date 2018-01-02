using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Utility;
using FloatingBars;
using Rbots.Core;

namespace Rbots.Characters
{
	[RequireComponent(typeof(Animator))]
	public class EnemyController : MonoBehaviour, IDamageable
	{
		public Interactables interactableType;

		[SerializeField] FieldOfViewController myEye;

		[SerializeField] bool ResetHP;
		// [SerializeField] FloatVariable RegenRate;

		[SerializeField] AttackTypeController.AttackType attackType; // determine attack distance (H2H / ranged)
		[SerializeField] GameObject LightningCast;
		[SerializeField] GameObject CastSpawner;
		GameObject CurrentCast;

		[SerializeField] GameObject Projectile;
		[SerializeField] GameObject ProjectileSpawner;
		[SerializeField] float damagePerShot = 5f;
		[SerializeField] float secondsPerShot = 1f;
		bool isAttacking = false;
		bool isMovingAgent = false;

		[Tooltip("Determine the chasing range depending on attackRadius")]
		[SerializeField]
		float attackRadiusMultiplicator = 2;
		[HideInInspector]
		public float chaseRange;

		FloatVariable HP;
		FloatVariable StartingHP;
		FloatVariable MinHP;
		FloatingBarController fbc;
		[HideInInspector]
		public bool isDead = false;
		
		private GameObject CurrentTarget;
		private Animator anim;
		private NavMeshAgent agent;
		private float defaultStoppingRadius;

		// Utilities
		private AttackTypeController atc = new AttackTypeController();

		[HideInInspector]
		public float attackRadius
		{
			get {
				switch (attackType) {
					case AttackTypeController.AttackType.closed:
						return atc.CloseRadius;
					case AttackTypeController.AttackType.ranged:
						return atc.RangeRadius;
					default:
						return atc.CloseRadius;
				}
			}
		}

		private void Awake()
		{
			chaseRange = attackRadius * attackRadiusMultiplicator;
			anim = GetComponent<Animator>();
			agent = GetComponent<NavMeshAgent>();
			defaultStoppingRadius = agent.radius;
		}

		// Use this for initialization
		void Start()
		{
			PrepareFloatingBar();
		}

		private void PrepareFloatingBar()
		{
			fbc = GetComponentInChildren<FloatingBarController>();
			if (fbc) {
				HP = fbc.resource;
				StartingHP = fbc.Max;
				MinHP = fbc.Min;

				if (ResetHP)
					HP.SetValue(StartingHP);
			}
			ChangeFbcDisplay(false);
		}

		public void ChangeFbcDisplay(bool isTargetted)
		{
			fbc.gameObject.SetActive(isTargetted);
		}

		// Update is called once per frame
		void Update()
		{
			if (!isDead) {
				// Run animation blend tree
				anim.SetFloat("EnemyVelocity", Mathf.Abs(agent.velocity.x));
				CurrentTarget = myEye.Target;

				if (CurrentTarget) {
					float distanceToTarget = Vector3.Distance(agent.transform.position, CurrentTarget.transform.position);

					// myEye can see target if it's in chaseRange and angle
					if (myEye.CanSeeTarget()) {
						MoveToInteract(CurrentTarget);
						// keep looking at target
						gameObject.transform.LookAt(CurrentTarget.transform);
					} else {
						if (isAttacking && distanceToTarget <= chaseRange) {
							// stop attack
							StopInteraction(CurrentTarget);
							// but try to catch it
							MoveToInteract(CurrentTarget);
						} else
							StopInteraction(CurrentTarget);
					}
				}

				if (isMovingAgent && !agent.pathPending) {
					// get the true remaining distance
					float remainingDistance = agent.remainingDistance > 0 ? Mathf.Abs(agent.remainingDistance - agent.radius) : 0;

					if (remainingDistance <= GetStoppingDistance()) {
						if (!isAttacking) {
							// launch interaction and stop movements
							ResetAgentValues();
							Interactable.movingNavMeshAgent = agent;
							Interactable.hasInteracted = true;
							HandleInteraction(CurrentTarget);
						}
					}
				}
			}
		}

		//=================
		//  MOVEMENT
		//=================
		public void MoveToInteract(GameObject target)
		{
			isMovingAgent = true;
			agent.destination = target.transform.position;
			CurrentTarget = target;
		}
		public void MoveToInteract(Vector3 interactionPoint)
		{
			isMovingAgent = true;
			agent.destination = interactionPoint;
			CurrentTarget = null;
		}

		// Called by animation
		public void CastParticle()
		{
			isAttacking = true;
			CurrentCast = Instantiate(
				LightningCast,
				CastSpawner.transform
			);
		}

		public void StopCasting()
		{
			if (CurrentCast)
				Destroy(CurrentCast);
		}

		private void LaunchProjectile()
		{
			GameObject newProjectile = Instantiate(
				Projectile,
				ProjectileSpawner.transform.position,
				Quaternion.identity,
				ProjectileSpawner.transform
			);
			Projectile c_Projectile = newProjectile.GetComponent<Projectile>();
			c_Projectile.damageAmount = damagePerShot;

			//Vector3 targetTopPos = gac[CurrentTarget];
			Vector3 direction = (CurrentTarget.transform.position - ProjectileSpawner.transform.position) + Vector3.up;
			float projectileSpeed = c_Projectile.projectileSpeed;
			newProjectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
		}

		public void HandleInteraction(GameObject target)
		{
			if (target.name == "Player" && !isAttacking) {
				isAttacking = true;
				if (!target.GetComponent<Player>().myTarget) {
					target.GetComponent<Player>().myTarget = gameObject;
				}

				if (attackType == AttackTypeController.AttackType.ranged && CastSpawner != null) {
					InvokeRepeating("LaunchProjectile", secondsPerShot, secondsPerShot);
					anim.SetBool("IsAttacking", true);
				} else if (attackType == AttackTypeController.AttackType.ranged)
					InvokeRepeating("LaunchProjectile", secondsPerShot, secondsPerShot);
				else if (attackType == AttackTypeController.AttackType.closed)
					anim.SetBool("IsAttacking", true);
			}
		}

		public void StopInteraction(GameObject target)
		{
			if (target.name == "Player") {
				isAttacking = false;

				if (attackType == AttackTypeController.AttackType.ranged)
					CancelInvoke();
				else if (attackType == AttackTypeController.AttackType.closed) {
					anim.SetBool("IsAttacking", false);
					StopCasting();
				}
			}
		}

		public void TakeDamage(float amount)
		{
			if (!isAttacking)
				HandleInteraction(CurrentTarget);

			HP.ApplyChange(-amount);
			if (HP.Value <= MinHP.Value) {
				Die();
			}
		}

		void Die()
		{
			isDead = true;
			Player.targetIsDead = true;
			StopInteraction(CurrentTarget);
			anim.SetBool("IsDead", true);
			Destroy(fbc.gameObject);
		}

		public void CallDestroy()
		{
			StartCoroutine("EraseMe");
		}

		IEnumerator EraseMe()
		{
			yield return new WaitForSeconds(5f);

			Destroy(gameObject);
		}

		public float GetStoppingDistance()
		{
			float additionalStoppingDistance;

			switch (interactableType) {
				case Interactables.Player:
					additionalStoppingDistance = 0;
					break;
				case Interactables.GroundEnemy:
					additionalStoppingDistance = attackRadius - 1;
					break;
				case Interactables.FlyingEnemy:
					additionalStoppingDistance = attackRadius - 1;
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
			return agent.stoppingDistance = additionalStoppingDistance;
		}

		public void ResetAgentValues()
		{
			// Reset path stops the movement
			isMovingAgent = false;
			agent.ResetPath();
			agent.stoppingDistance = defaultStoppingRadius;
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, attackRadius);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, chaseRange);
		}
	}
}
