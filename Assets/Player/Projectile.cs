using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField] float damageAmount = 10f;

	private void OnTriggerEnter(Collider other)
	{
		Component damageableComponent = other.gameObject.GetComponent(typeof(IDamageable));

		if (damageableComponent) {
			Debug.Log("hit : " + damageableComponent);
			(damageableComponent as IDamageable).TakeDamage(damageAmount);
		}
	}
}
