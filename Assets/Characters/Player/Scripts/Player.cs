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
		public Material outlineMaterial;
		
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
				// tab targetting ?
				int index = myEye.visibleTargets.IndexOf(lastVisibleTarget);
				myTarget = myEye.visibleTargets[index == -1 ? 0 : index % myEye.visibleTargets.Count].gameObject;
				Material baseMaterial = myTarget.GetComponentInChildren<Renderer>().material;
				myTarget.GetComponentInChildren<Renderer>().materials = new Material[2] { baseMaterial, outlineMaterial };
				Debug.Log("acquiring target : " + myTarget);
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
			if (myTarget) {
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
