using UnityEngine;

namespace Rbots.Characters
{
	public class PlayerAnimations : MonoBehaviour
	{
		Animator anim;
		Player Player;
		float Speed = 0f;

		private void Awake()
		{
			anim = GetComponent<Animator>();
			Player = GetComponentInParent<Player>();
		}

		private void Update()
		{
			ApplyAnimations();
		}

		void ApplyAnimations()
		{
			anim.SetFloat("Speed", Speed);
		}

		public void SetMoveSpeed(float currentSpeed)
		{
			Speed = currentSpeed;
		}

		public void ApplyBaseAttack()
		{
			anim.SetBool("IsAttacking", true);
		}

		public void StopBaseAttack()
		{
			anim.SetBool("IsAttacking", false);
		}

		void DealDamage()
		{
			Player.DealDamage();
		}
	}
}
