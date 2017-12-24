using System;
using Rbots.Core;
using UnityEngine;
using System.Collections;

public class WaterController : MonoBehaviour, IDamageable {

	float damageAmount = 5f;
	GameObject colliderObject;

	public void TakeDamage(float amount)
	{
		throw new NotImplementedException();
	}

	private void OnTriggerEnter(Collider other)
	{
		colliderObject = other.gameObject;

		StartCoroutine("DealDamage");
	}

	private void OnTriggerExit(Collider other)
	{
		StopAllCoroutines();
	}

	IEnumerator DealDamage()
	{
		while (true) {
			yield return new WaitForSeconds(1f);

			Component damageableComponent = colliderObject.GetComponent(typeof(IDamageable));

			if (damageableComponent) {
				(damageableComponent as IDamageable).TakeDamage(damageAmount);
			}
		}
	}
}
