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

		public void ApplyAttack(string attackBoolName)
		{
			anim.SetBool(attackBoolName, true);
		}

		public void StopAttack(string attackBoolName)
		{
			anim.SetBool(attackBoolName, false);
		}

		void DealDamage()
		{
			Player.DealDamage();
		}

		public void PowerOff()
		{
			anim.SetBool("ShutDown", true);
		}

		public void PowerOn()
		{
			anim.SetBool("ShutDown", false);
		}
	}
}
