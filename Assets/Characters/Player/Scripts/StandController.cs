using UnityEngine;
using FloatingBars;
using Utility;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rbots.CameraUI;

namespace Rbots.Characters
{
	public class StandController : MonoBehaviour
	{
		[SerializeField] GameObject Player;
		[SerializeField] GameObject RegenParticle;
		Player c_Player;
		PlayableDirector playableDirector;
		PlayerAnimations playerAnimations;
		bool padInUse = false;
		bool padActive = false;

		AudioSource audioSource;

		[SerializeField] GameObject StandControls;
		[SerializeField] GameObject FirstSelectedMenuItem;
		EventSystem eventSystem;

		//=============================
		// HEALTH
		//=============================
		[SerializeField] float HPRegenRateBooster;
		[SerializeField] float coolingRegenRateBooster;
		[SerializeField] Text hullStateText; 

		FloatVariable HP;
		FloatVariable StartingHP;
		FloatVariable MinHP;
		FloatingBarController fbc;
		bool regenerating = false;

		//=============================
		// MESSAGES
		//=============================
		public GameEvent PadEnterEvent;
		public GameEvent PadExitEvent;

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
			eventSystem = FindObjectOfType<EventSystem>();
			c_Player = Player.GetComponent<Player>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (PadEnterEvent != null) {
				PadEnterEvent.Raise();
				PadEnterEvent = null;
			}


			this.padActive = true;

			fbc = Player.GetComponent<Player>().fbcHealth;
			HP = fbc.resource;
			StartingHP = fbc.Max;

			if (FirstSelectedMenuItem) {
				//eventSystem.SetSelectedGameObject = null;
				eventSystem.SetSelectedGameObject(FirstSelectedMenuItem);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if(Input.GetButton("Use") && !padInUse && this.padActive) {
				padInUse = true;
				playerAnimations = Player.GetComponentInChildren<PlayerAnimations>();
				playerAnimations.PowerOff();
				UsePad();
			}


			float hullPercentage = (HP.Value / StartingHP.Value) * 100;
			hullStateText.text = hullPercentage + "%";
		}

		private void OnTriggerExit(Collider other)
		{
			if (PadExitEvent != null) {
				PadExitEvent.Raise();
				PadExitEvent = null;
			}

			StopRegen();
			padActive = false;
		}

		void LaunchRegen()
		{
			//standCam.gameObject.SetActive(true);
			RegenParticle.SetActive(true);
			audioSource.Play();
			regenerating = true;

			//=============================
			// HEALTH - Regeneration Start
			//=============================
			if (HP.Value < StartingHP.Value && regenerating) {
				c_Player.ApplyRegen(HPRegenRateBooster, coolingRegenRateBooster);
			}
		}

		void StopRegen()
		{
			RegenParticle.SetActive(false);
			audioSource.Stop();
			c_Player.StopRegenerating();
			regenerating = false;
		}

		public void OnClickRepairBtn()
		{
			LaunchRegen();
		}

		public void OnClickChangeSpawnPoint()
		{
			c_Player.SpawnPoint = transform;
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
