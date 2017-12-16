using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float projectileSpeed;
	public float damageAmount = 10f;

	//public void AddForce(Vector3 force)
	//{
	//	GetComponent<Rigidbody>().AddForce(force * projectileSpeed);
	//}

	private void OnTriggerEnter(Collider other)
	{
		Component damageableComponent = other.gameObject.GetComponent(typeof(IDamageable));

		if (damageableComponent) {
			(damageableComponent as IDamageable).TakeDamage(damageAmount);
		}

		Destroy(gameObject);
	}
}
