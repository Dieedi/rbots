using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningCast : MonoBehaviour {
	
	[SerializeField] float damageAmount = 10f;

	float waitTime = .5f;
	float damageTimer = 0;
	GameObject CurrentTarget;

	void OnParticleCollision (GameObject collider)
	{
		CurrentTarget = collider;
		damageTimer += Time.deltaTime;
		if (damageTimer > waitTime) {
			DealDamage();
			damageTimer = 0;
		}
	}

	void DealDamage()
	{
		Component damageableComponent = CurrentTarget.GetComponent(typeof(IDamageable));

		if (damageableComponent) {
			(damageableComponent as IDamageable).TakeDamage(damageAmount);
		}
	}
}
