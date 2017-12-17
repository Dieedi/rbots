using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float projectileSpeed;
	public float damageAmount = 10f;
	
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "Player") {
			Component damageableComponent = other.gameObject.GetComponent(typeof(IDamageable));

			if (damageableComponent) {
				(damageableComponent as IDamageable).TakeDamage(damageAmount);
			}

			Destroy(gameObject);
		}
	}
}
