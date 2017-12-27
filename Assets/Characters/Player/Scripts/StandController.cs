using System.Collections;
using UnityEngine;
using FloatingBars;
using Utility;

namespace Rbots.Characters
{
	public class StandController : MonoBehaviour
	{

		[SerializeField] GameObject Player;
		[SerializeField] GameObject RegenParticle;
		[SerializeField] Cinemachine.CinemachineVirtualCamera standCam;

		AudioSource audioSource;

		//=============================
		// HEALTH
		//=============================
		[SerializeField] FloatVariable RegenRate;

		FloatVariable HP;
		FloatVariable StartingHP;
		FloatVariable MinHP;
		FloatingBarController fbc;
		bool regenerating = false;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void OnTriggerEnter(Collider other)
		{
			standCam.gameObject.SetActive(true);
			RegenParticle.SetActive(true);
			audioSource.Play();

			// TODO Player choice ?
			regenerating = true;

			fbc = Player.GetComponent<Player>().fbcHealth;
			HP = fbc.resource;
			StartingHP = fbc.Max;

			//=============================
			// HEALTH - Regeneration Start
			//=============================
			if (HP.Value < StartingHP.Value && regenerating) {
				StartCoroutine("RegenHP");
			}
		}

		private void OnTriggerStay(Collider other)
		{
		}

		private void OnTriggerExit(Collider other)
		{
			standCam.gameObject.SetActive(false);
			RegenParticle.SetActive(false);
			audioSource.Stop();
			regenerating = false;
		}

		//=============================
		// HEALTH - Regeneration
		//=============================
		IEnumerator RegenHP()
		{
			while (HP.Value < StartingHP.Value) {
				yield return new WaitForSeconds(1f);

				HP = fbc.resource;

				if (HP.Value >= StartingHP.Value) {
					yield break;
				} else {
					HP.ApplyChange(RegenRate);
				}
			}
		}

		public void OnClickLoadRedButton()
		{
			Player.GetComponent<PlayerControls>().ChangeRbot(PlayerTypes.Red);
		}

		public void OnClickLoadYellowButton()
		{
			Player.GetComponent<PlayerControls>().ChangeRbot(PlayerTypes.Yellow);
		}

		public void OnClickLoadPurpleButton()
		{
			Player.GetComponent<PlayerControls>().ChangeRbot(PlayerTypes.Purple);
		}
	}
}