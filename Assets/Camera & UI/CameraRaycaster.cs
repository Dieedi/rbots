using UnityEngine;
using Utility;

public class CameraRaycaster : MonoBehaviour
{
	public Layer[] layerPriorities = {
		Layer.RbotsWalkable,
		Layer.Enemy,
	};

	[SerializeField] float distanceToBackground = 100f;
	Camera viewCamera;

	RaycastHit m_hit;
	public RaycastHit hit
	{
		get { return m_hit; }
	}

	Layer layerHit;
	public Layer LayerHit
	{
		get { return layerHit; }
	}

	// Add GameEvent Raiser
	public GameEvent eventToRaise;

	void Start() // TODO Awake?
	{
		viewCamera = Camera.main;
	}

	void Update()
	{
		// Look for and return priority layer hit
		foreach (Layer layer in layerPriorities) {
			var hit = RaycastForLayer(layer);

			if (hit.HasValue) {
				m_hit = hit.Value;

				if (layerHit != layer) {
					layerHit = layer;
					// Raise GameEvent
					eventToRaise.Raise();
				}
				layerHit = layer;
				return;
			}
		}

		// Otherwise return background hit
		m_hit.distance = distanceToBackground;
		layerHit = Layer.RaycastEndStop;
	}

	RaycastHit? RaycastForLayer(Layer layer)
	{
		// Layer is defined by an enum in Utility.cs
		int layerMask = 1 << (int)layer; // Convert the int "layer" to his corresponding value in bitmask value which a layermask is.
		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit; // used as an out parameter
		bool hasHit = Physics.Raycast(ray, out hit, distanceToBackground, layerMask);
		if (hasHit) {
			return hit;
		}
		return null;
	}
}
