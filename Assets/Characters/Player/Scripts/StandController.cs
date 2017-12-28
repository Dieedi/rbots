using System.Collections;
using UnityEngine;
using FloatingBars;
using Utility;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

namespace Rbots.Characters
{
	public class StandController : MonoBehaviour
	{
		[SerializeField] GameObject Player;
		[SerializeField] GameObject RegenParticle;
		PlayableDirector playableDirector;
		PlayerAnimations playerAnimations;
		bool padInUse = false;
		bool padActive = false;

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

		void UsePad()
		{
			// play timeline
			playableDirector.Play();
		}

		public void ClosePad()
		{
			playableDirector.Stop();
			padInUse = false;
			playerAnimations = Player.GetComponentInChildren<PlayerAnimations>();
			playerAnimations.PowerOn();
		}

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			playableDirector = GetComponent<PlayableDirector>();
		}

		private void OnTriggerEnter(Collider other)
		{
			this.padActive = true;
			//standCam.gameObject.SetActive(true);
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
			if(Input.GetKey(KeyCode.E) && !padInUse && this.padActive) {
				padInUse = true;
				playerAnimations = Player.GetComponentInChildren<PlayerAnimations>();
				playerAnimations.PowerOff();
				UsePad();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			//standCam.gameObject.SetActive(false);
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
