using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		//Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
		raycaster.O_LayerChange += OnLayerChange;
	}

	void OnLayerChange(Layer layerHit)
	{
		switch (layerHit) {
			case Layer.Enemy:
				Cursor.SetCursor(combatCursor, cursorHotspot, CursorMode.Auto);
				break;
			case Layer.Walkable:
				Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
				break;
			default:
				//Cursor.SetCursor(otherCursor, cursorHotspot, CursorMode.Auto);
				return;
		}		
	}
}
