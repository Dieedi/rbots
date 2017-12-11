using UnityEngine;
using Utility;

public class CursorController : MonoBehaviour
{
	[SerializeField] Texture2D walkCursor = null;
	[SerializeField] Texture2D combatCursor = null;
	[SerializeField] Texture2D otherCursor = null;
	[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

	private CameraRaycaster raycaster;

	// Use this for initialization
	void Start()
	{
		raycaster = GetComponent<CameraRaycaster>();
	}

	// Called by GameEvent
	public void OnLayerChange()
	{
		Debug.Log(raycaster.LayerHit);
		switch (raycaster.LayerHit) {
			case Layer.Enemy:
				Cursor.SetCursor(combatCursor, cursorHotspot, CursorMode.Auto);
				break;
			case Layer.RbotsWalkable:
				Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
				break;
			default:
				//Cursor.SetCursor(otherCursor, cursorHotspot, CursorMode.Auto);
				return;
		}		
	}
}
