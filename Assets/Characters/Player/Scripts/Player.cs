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
		GameObject myTarget;
		[HideInInspector]
		public static bool targetIsDead = false;

		//=============================
		// MOVEMENTS
		//=============================
		[SerializeField] Interactables interactableType;
		
		//=============================
		// INTERACTIONS
		//=============================
		Transform lastVisibleTarget;
		[SerializeField] GameObject SelectProjector;
		GameObject TargetSelectedProjector;
		public float attackRadius;

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
		}

		// Update is called once per frame
		void Update()
		{
			//=============================
			// INTERACTIONS
			//=============================
			if (myEye.visibleTargets.Count > 0 && !myTarget) {
				myTarget = myEye.visibleTargets[0].gameObject;
			}

			if (Input.GetButtonDown("Fire2")) {
				if (TargetSelectedProjector) Destroy(TargetSelectedProjector);

				// tab targetting
				int index = lastVisibleTarget ? myEye.visibleTargets.IndexOf(lastVisibleTarget) : -1;
				
				lastVisibleTarget = myEye.visibleTargets[index == -1 ? 0 : (index + 1 < myEye.visibleTargets.Count) ? index + 1 : 0];
				myTarget = lastVisibleTarget.gameObject;
				
				TargetSelectedProjector = Instantiate(SelectProjector, myTarget.transform);
			}

			myEye.Target = myTarget;
		}

		//=============================
		// INTERACTIONS
		//=============================
		void GetInteraction()
		{
			if (targetIsDead)
				targetIsDead = false;
		}

		public void HandleInteraction(GameObject target)
		{
			myTarget = target;
			myEye.Target = myTarget;

			if (target.GetComponent<EnemyController>()) {
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
			if (myTarget && myEye.CanSeeTarget()) {
				Component damageableComponent = myTarget.GetComponent(typeof(IDamageable));

				if (damageableComponent) {
					(damageableComponent as IDamageable).TakeDamage(damageAmount);
				}

				if (myTarget.GetComponent<EnemyController>().isDead)
					myTarget = null;
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
	}
}
