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
		
		//=============================
		// HEALTH
		//=============================
		[SerializeField] bool ResetHP;

		FloatVariable HP;
		FloatVariable StartingHP;
		FloatVariable MinHP;
		
		GameObject myTarget;
		[HideInInspector]
		public static bool targetIsDead = false;
		FloatingBarController fbc;

		//=============================
		// MOVEMENTS
		//=============================
		[SerializeField] Interactables interactableType;
		
		//=============================
		// INTERACTIONS
		//=============================
		[SerializeField] float damageAmount = 25f;
		[SerializeField] GameObject SelectProjector;
		public float attackRadius;
		[HideInInspector]
		public bool isAttacking = false;

		GameObject TargetSelectedProjector;
		Transform lastVisibleTarget;

		void Start()
		{
			PrepareFloatingBar();
		}

		private void PrepareFloatingBar()
		{
			fbc = GetComponentInChildren<FloatingBarController>();
			HP = fbc.resource;
			StartingHP = fbc.Max;
			MinHP = fbc.Min;

			if (ResetHP)
				HP.SetValue(StartingHP);

			ChangeFbcDisplay(false);
		}

		void ChangeFbcDisplay(bool isDamaged)
		{
			fbc.gameObject.SetActive(isDamaged);
		}

		// Update is called once per frame
		void Update()
		{
			//=============================
			// HEALTH
			//=============================
			if (HP.Value < StartingHP.Value)
				ChangeFbcDisplay(true);
			else
				ChangeFbcDisplay(false);

			//=============================
			// INTERACTIONS
			//=============================
			if (myEye.visibleTargets.Count > 0 && !myTarget) {
				myTarget = myEye.visibleTargets[0].gameObject;
			}

			if (Input.GetButtonDown("Fire2")) {
				SwitchTarget();
			}

			myEye.Target = myTarget;
		}

		private void SwitchTarget()
		{
			if (TargetSelectedProjector) {
				Destroy(TargetSelectedProjector);
				myTarget.GetComponent<EnemyController>().ChangeFbcDisplay(false);
			}

			// tab targetting
			int index = lastVisibleTarget ? myEye.visibleTargets.IndexOf(lastVisibleTarget) : -1;

			lastVisibleTarget = myEye.visibleTargets[index == -1 ? 0 : (index + 1 < myEye.visibleTargets.Count) ? index + 1 : 0];
			myTarget = lastVisibleTarget.gameObject;
			myTarget.GetComponent<EnemyController>().ChangeFbcDisplay(true);

			TargetSelectedProjector = Instantiate(SelectProjector, myTarget.transform);
		}

		//=============================
		// INTERACTIONS
		//=============================
		void GetInteraction()
		{
			if (targetIsDead)
				targetIsDead = false;
		}

		public void DealDamage()
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
			Debug.Log(amount);
			HP.ApplyChange(-amount);

			if (HP.Value <= MinHP.Value) {
				//TODO Die()
			}
		}
	}
}
