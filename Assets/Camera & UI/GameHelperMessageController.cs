using UnityEngine;
using UnityEngine.UI;

namespace Rbots.CameraUI
{
	public class GameHelperMessageController : MonoBehaviour
	{
		Text helperMessage;

		float displayDelay = 0f;
		float displayTimer = 0f;
		bool messageIsDisplayed = false;

		private void Awake()
		{
			helperMessage = GetComponentInChildren<Text>();
		}

		// Update is called once per frame
		void Update()
		{
			if (messageIsDisplayed) {
				displayTimer += Time.deltaTime;

				if (displayTimer >= displayDelay) {
					StopDisplay();
				}
			}
		}

		public void DisplayMessage (GameHelperMessages ghm)
		{
			if (!ghm.messageHasBeenDisplayed) {
				displayDelay = ghm.MessageDisplayTime;
				helperMessage.text = ghm.HelperMessageText;
				ghm.messageHasBeenDisplayed = true;
				messageIsDisplayed = true;
			}
		}

		void StopDisplay ()
		{
			helperMessage.text = "";
			messageIsDisplayed = false;
			displayTimer = 0f;
		}
	}
}
