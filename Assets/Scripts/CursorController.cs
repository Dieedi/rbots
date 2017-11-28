using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

	private CameraRaycaster raycaster;

	// Use this for initialization
	void Start()
	{
		raycaster = GetComponent<CameraRaycaster>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButton(0)) {
			switch (raycaster.layerHit) {
				case Layer.Enemy:
					Debug.Log("you click on ennemy !");
					break;
				case Layer.Walkable:
					Debug.Log("you click a walkable area... walk !");
					break;
				default:
					Debug.Log("other action");
					break;
			}
		}
	}

	private void LateUpdate()
	{
		// change cursor icon depending on object overed
	}
}
