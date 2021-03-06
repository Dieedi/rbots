using UnityEngine;
using Utility;
using FloatingBars;
using Rbots.Core;
using System.Collections;

namespace Rbots.Characters
{
	public class Player : MonoBehaviour, IDamageable
	{
		public FieldOfViewController myEye;
		
		//=============================
		// HEALTH
		//=============================
		[SerializeField] bool ResetHP;
		[SerializeField] FloatVariable HPRegenRate;
		[HideInInspector] public FloatVariable HP;
		public FloatingBarController fbcHealth;

		FloatVariable StartingHP;
		FloatVariable MinHP;
		float currentHPRegenRate;

		//=============================
		// HEAT
		//=============================
		[SerializeField] bool ResetHeat;
		[SerializeField] FloatVariable HeatCoolingRate;
		public FloatingBarController fbcHeat;

		FloatVariable Heat;
		FloatVariable StartingHeat;
		FloatVariable MaxHeat;
		float attackHeatCost = 0f;
		bool cooling = false;
		float currentCoolingRate;

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
		
		//=============================
		// MESSAGES
		//=============================
		public GameEvent TargetEvent;

		//=============================
		// SPAWNER
		//=============================
		public Transform SpawnPoint;

		private void Awake()
		{
			transform.position = SpawnPoint.position;
		}

		void Start()
		{
			PrepareFloatingBar();
		}

		private void PrepareFloatingBar()
		{
			Heat = fbcHeat.resource;
			StartingHeat = fbcHeat.Min;
			MaxHeat = fbcHeat.Max;
			currentHPRegenRate = HPRegenRate.Value;

			if (ResetHeat)
				Heat.SetValue(StartingHeat);

			ChangefbcHeatDisplay(false);
			
			HP = fbcHealth.resource;
			StartingHP = fbcHealth.Max;
			MinHP = fbcHealth.Min;
			currentCoolingRate = HeatCoolingRate.Value;

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
			if (myEye.visibleTargets.Count > 0 && TargetEvent != null) {
				TargetEvent.Raise();
				TargetEvent = null;
			}

			if (Input.GetButtonDown("Targetting")) {
				SwitchTarget();
			}

			if (targetIsDead) {
				myTarget = myEye.Target = null;
				targetIsDead = false;
			}

			if (!TargetSelectedProjector && myTarget)
				ShowSelectedTarget();
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
			myTarget = myEye.Target = lastVisibleTarget.gameObject;

			ShowSelectedTarget();
		}

		void ShowSelectedTarget()
		{
			myEye.Target = myTarget;
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
			attackHeatCost = heatCost;
			return Heat.Value + heatCost <= MaxHeat.Value;
		}

		public void CalculateDamage(float multiplier)
		{
			calculatedDamage = baseDamageAmount * multiplier;
		}

		public void DealDamage()
		{
			if (myTarget && myEye.CanSeeTarget()) {
				Heat.ApplyChange(attackHeatCost);
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
				Die();
			}
		}
		
		public void ApplyRegen (float repairMultiplicator = 1f, float coolingMultiplicator = 1f)
		{
			currentHPRegenRate = currentHPRegenRate * repairMultiplicator;
			currentCoolingRate = currentCoolingRate * coolingMultiplicator;
			StartCoroutine("RegenHP");
			StartCoroutine("CoolingHeat");
		}

		public void StopRegenerating()
		{
			currentHPRegenRate = HPRegenRate.Value;
			currentCoolingRate = HeatCoolingRate.Value;
			StopCoroutine("RegenHP");
		}

		void Die ()
		{
			myTarget = null;
			PrepareFloatingBar();
			ReSpawn();
		}

		void ReSpawn()
		{
			transform.position = SpawnPoint.position;
		}

		//=============================
		// HEALTH - Regeneration
		//=============================
		IEnumerator RegenHP()
		{
			while (HP.Value < StartingHP.Value) {
				yield return new WaitForSeconds(1f);

				HP = fbcHealth.resource;

				if (HP.Value >= StartingHP.Value) {
					HP.SetValue(StartingHP);
					yield break;
				} else {
					HP.ApplyChange(currentHPRegenRate);
				}
			}
		}

		//=============================
		// HEAT - Cooling
		//=============================
		IEnumerator CoolingHeat()
		{
			while (true) {
				yield return new WaitForSeconds(1f);
				
				if (Heat.Value <= StartingHeat.Value) {
					Heat.SetValue(StartingHeat);
					cooling = false;
					ChangefbcHeatDisplay(false);
					yield break;
				} else {
					Heat.ApplyChange(currentCoolingRate);
				}
			}
		}
	}
}
