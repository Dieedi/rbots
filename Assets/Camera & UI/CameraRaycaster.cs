using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rbots.CameraUI
{
	public class CameraRaycaster : MonoBehaviour
	{
		// INSPECTOR PROPERTIES RENDERED BY CUSTOM EDITOR SCRIPT
		[SerializeField] int[] layerPriorities;

		[SerializeField] float maxRaycastDepth = 100f;
		public int topPriorityLayerLastFrame = -1; // So get ? from start with Default layer terrain

		// Setup GameEvents for broadcasting layer changes to other classes
		public GameEvent cursorLayerChange;
		public GameEvent clickPriority;

		void Update()
		{
			// Check if pointer is over an interactable UI element
			if (EventSystem.current.IsPointerOverGameObject()) {
				NotifyIfLayerChanged(5);
				return; // Stop looking for other objects
			}

			// Raycast to max depth, every frame as things can move under mouse
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] raycastHits = Physics.RaycastAll(ray, maxRaycastDepth);

			RaycastHit? priorityHit = FindTopPriorityHit(raycastHits);
			if (!priorityHit.HasValue) // if hit no priority object
			{
				NotifyIfLayerChanged(0); // broadcast default layer 0
				return;
			}

			// Notify delegates of layer change
			var layerHit = priorityHit.Value.collider.gameObject.layer;
			NotifyIfLayerChanged(layerHit);

			// Notify delegates of highest priority game object under mouse when clicked
			if (Input.GetMouseButton(0)) {
				clickPriority.Raise(); // (priorityHit.Value, layerHit);
			}
		}

		void NotifyIfLayerChanged(int newLayer)
		{
			if (newLayer != topPriorityLayerLastFrame) {
				topPriorityLayerLastFrame = newLayer;
				cursorLayerChange.Raise();
				// notifyLayerChangeObservers(newLayer);
			}
		}

		RaycastHit? FindTopPriorityHit(RaycastHit[] raycastHits)
		{
			// Form list of layer numbers hit
			List<int> layersOfHitColliders = new List<int>();
			foreach (RaycastHit hit in raycastHits) {
				layersOfHitColliders.Add(hit.collider.gameObject.layer);
			}

			// Step through layers in order of priority looking for a gameobject with that layer
			foreach (int layer in layerPriorities) {
				foreach (RaycastHit hit in raycastHits) {
					if (hit.collider.gameObject.layer == layer) {
						return hit; // stop looking
					}
				}
			}
			return null; // because cannot use GameObject? nullable
		}
	}
}
