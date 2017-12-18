using UnityEngine;
using Rbots.Core;

namespace Rbots.Characters
{
	public class Projectile : MonoBehaviour
	{

		public float projectileSpeed;
		public float damageAmount = 10f;

		float lifeTime = 5f;
		float aliveSince = 0f;

		private void Update()
		{
			aliveSince += Time.deltaTime;
			if (aliveSince >= lifeTime) {
				aliveSince = 0;
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.name == "Player") {
				Component damageableComponent = other.gameObject.GetComponent(typeof(IDamageable));

				if (damageableComponent) {
					(damageableComponent as IDamageable).TakeDamage(damageAmount);
				}

				Destroy(gameObject);
			} else if (other.gameObject.layer != 9) {
				Destroy(gameObject);
			}
		}
	}
}
