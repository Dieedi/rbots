using UnityEngine;

namespace Rbots.CameraUI
{
	public class CursorController : MonoBehaviour
	{
		[SerializeField] Texture2D walkCursor = null;
		[SerializeField] Texture2D combatCursor = null;
		[SerializeField] Texture2D otherCursor = null;
		[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

		[SerializeField] const int walkableLayerNumber = 10;
		[SerializeField] const int enemyLayerNumber = 9;
		
		// Called by GameEvent
		public void OnLayerChange()
		{
			int layer = GetComponent<CameraRaycaster>().topPriorityLayerLastFrame;
			switch (layer) {
				case enemyLayerNumber:
					Cursor.SetCursor(combatCursor, cursorHotspot, CursorMode.Auto);
					break;
				case walkableLayerNumber:
					Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
					break;
				default:
					//Cursor.SetCursor(otherCursor, cursorHotspot, CursorMode.Auto);
					return;
			}
		}
	}
}
