using System;
using Rbots.Core;
using UnityEngine;
using System.Collections;

public class WaterController : MonoBehaviour, IDamageable {

	[SerializeField] float damageAmount = 5f;
	GameObject colliderObject;

	public void TakeDamage(float amount)
	{
		Debug.LogError("should never appear, water can't be damage !");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "Player") {
			colliderObject = other.gameObject;
			StartCoroutine("DealDamage");
		}
	}

	private void OnTriggerExit(Collider other)
	{
		StopCoroutine("DealDamage");
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
