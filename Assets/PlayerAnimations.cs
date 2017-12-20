using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rbots.Characters
{
	[RequireComponent(typeof(Animator))]
	public class PlayerAnimations : MonoBehaviour
	{
		Animator anim;
		float Speed = 0f;

		private void Awake()
		{
			anim = GetComponent<Animator>();
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
	}
}
