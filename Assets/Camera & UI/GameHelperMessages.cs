using UnityEngine;
using UnityEngine.UI;

namespace Rbots.CameraUI
{
	[CreateAssetMenu(menuName = ("Rbots/New Message"))]
	public class GameHelperMessages : ScriptableObject
	{
		[SerializeField] string helperMessageText;
		public string HelperMessageText { get { return helperMessageText; } }
		[SerializeField] float messageDisplayTime = 6f;
		public float MessageDisplayTime { get { return messageDisplayTime; } }
		public bool messageHasBeenDisplayed = false;

		float messageDisplayTimer = 0f;

		public void UseHelperMessage (string message)
		{
			messageHasBeenDisplayed = true;
		}

		//public static void EraseHelperMessage()
		//{
		//	helperMessageText.text = "";
		//	messageDisplayTimer = 0f;
		//	messageIsDisplayed = false;
		//}
	}
}
