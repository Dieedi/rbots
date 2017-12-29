using UnityEngine;
using UnityEngine.AI;
using Utility;
using FloatingBars;
using Rbots.Core;
using System.Collections;

namespace Rbots.Characters
{
	public class Player : MonoBehaviour, IDamageable
	{
		[SerializeField] FieldOfViewController myEye;

		//=============================
		// HEALTH
		//=============================
		[SerializeField] bool ResetHP;
		[HideInInspector] public FloatVariable HP;
		[HideInInspector] public FloatingBarController fbcHealth;

		FloatVariable StartingHP;
		FloatVariable MinHP;

		//=============================
		// HEAT
		//=============================
		[SerializeField] bool ResetHeat;
		[SerializeField] FloatVariable HeatCoolingRate;

		FloatVariable Heat;
		FloatVariable StartingHeat;
		FloatVariable MaxHeat;
		FloatingBarController fbcHeat;
		bool cooling = false;

		//=============================
		// MOVEMENTS
		//=============================
		[SerializeField] Interactables interactableType;

		//=============================
		// INTERACTIONS
		//=============================
		public float baseDamageAmount = 25f;
		float calculatedDamage;
		public float CalculatedDamage { get { return this.calculatedDamage; } set { this.calculatedDamage = value; } }

		[SerializeField] GameObject SelectProjector;
		public float attackRadius;
		[HideInInspector]
		public bool isAttacking = false;
		[HideInInspector]
		public GameObject myTarget;
		[HideInInspector]
		public static bool targetIsDead = false;

		GameObject TargetSelectedProjector;
		Transform lastVisibleTarget;

		void Awake()
		{
			PrepareFloatingBar();
		}

		private void PrepareFloatingBar()
		{
			fbcHeat = GameObject.FindGameObjectWithTag("PlayerHeat").GetComponent<FloatingBarController>();
			Heat = fbcHeat.resource;
			StartingHeat = fbcHeat.Min;
			MaxHeat = fbcHeat.Max;

			if (ResetHeat)
				Heat.SetValue(StartingHeat);

			ChangefbcHeatDisplay(false);

			fbcHealth = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<FloatingBarController>();
			HP = fbcHealth.resource;
			StartingHP = fbcHealth.Max;
			MinHP = fbcHealth.Min;

			if (ResetHP)
				HP.SetValue(StartingHP);

			ChangefbcHealthDisplay(false);
		}

		void ChangefbcHealthDisplay(bool isDamaged)
		{
			fbcHealth.gameObject.SetActive(isDamaged);
		}

		void ChangefbcHeatDisplay(bool isDamaged)
		{
			fbcHeat.gameObject.SetActive(isDamaged);
		}

		// Update is called once per frame
		void Update()
		{
			CheckResources();

			//=============================
			// INTERACTIONS
			//=============================
			//if (myEye.visibleTargets.Count > 0 && !myTarget) {
			//	myTarget = myEye.visibleTargets[0].gameObject;
			//}

			if (Input.GetButtonDown("Targetting")) {
				SwitchTarget();
			}

			if(myTarget)
				myEye.Target = myTarget;
		}

		private void CheckResources()
		{
			//=============================
			// HEALTH
			//=============================
			if (HP.Value < StartingHP.Value)
				ChangefbcHealthDisplay(true);
			else
				ChangefbcHealthDisplay(false);

			//=============================
			// HEAT
			//=============================
			if (Heat.Value > StartingHeat.Value && !cooling) {
				cooling = true;
				ChangefbcHeatDisplay(true);
				StartCoroutine("CoolingHeat");
			}
		}

		private void SwitchTarget()
		{
			if (TargetSelectedProjector) {
				Destroy(TargetSelectedProjector);

				if (myTarget)
					myTarget.GetComponent<EnemyController>().ChangeFbcDisplay(false);

				myTarget = null;
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

		public bool CanAttack(float heatCost)
		{
			return Heat.Value + heatCost <= MaxHeat.Value;
		}

		public void CalculateDamage(float multiplier)
		{
			calculatedDamage = baseDamageAmount * multiplier;
		}

		public void DealDamage()
		{
			if (myTarget && myEye.CanSeeTarget()) {
				Heat.ApplyChange(5f);
				Component damageableComponent = myTarget.GetComponent(typeof(IDamageable));

				if (damageableComponent) {
					(damageableComponent as IDamageable).TakeDamage(calculatedDamage);
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

		//=============================
		// HEAT - Cooling
		//=============================
		IEnumerator CoolingHeat()
		{
			while (Heat.Value > StartingHeat.Value) {
				yield return new WaitForSeconds(1f);
				
				if (Heat.Value <= StartingHeat.Value) {
					cooling = false;
					ChangefbcHeatDisplay(false);
					yield break;
				} else {
					Heat.ApplyChange(HeatCoolingRate);
				}
			}
		}
	}
}
