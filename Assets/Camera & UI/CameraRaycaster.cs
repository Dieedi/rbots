using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
	public Layer[] layerPriorities = {
		Layer.Enemy,
		Layer.Walkable
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

	public delegate void OnLayerChange(Layer newLayer); // declare delegate
	public event OnLayerChange O_LayerChange; // instantiate observers

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
					O_LayerChange(layer); // call delegate
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
		int layerMask = 1 << (int)layer; // See Unity docs for mask formation
		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

		RaycastHit hit; // used as an out parameter
		bool hasHit = Physics.Raycast(ray, out hit, distanceToBackground, layerMask);
		if (hasHit) {
			return hit;
		}
		return null;
	}
}
