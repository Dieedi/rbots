using UnityEngine;
using UnityEngine.UI;

namespace FloatingBars
{
	public class FloatingBarController : MonoBehaviour
	{

		public FloatVariable resource;
		public FloatVariable Min;
		public FloatVariable Max;

		Camera mainCam;
		Image damageFiller;
		FloatingBarVariableController fbv_controller;

		private void Awake()
		{
			// Instantiate before starting all controllers
			resource = Instantiate(resource);
		}

		private void Start()
		{
			//fbv_controller = GetComponentInParent<FloatingBarVariableController>();
			mainCam = Camera.main;
			damageFiller = GetComponentsInChildren<Image>()[1]; // the foreground bar
		}

		// Update is called once per frame
		void Update()
		{
			// Always view the bar
			
			transform.LookAt(mainCam.transform);
			// I don't remind where I found that but it works ...
			damageFiller.fillAmount = Mathf.Clamp01(Mathf.InverseLerp(Min.Value, Max.Value, resource.Value));
		}
	}
}
