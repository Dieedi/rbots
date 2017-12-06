using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingBarController : MonoBehaviour {

	public FloatVariable resource;
	public FloatVariable Min;
	public FloatVariable Max;

	private Camera mainCam;
	private Image damageFiller;

	private void Start()
	{
		mainCam = Camera.main;
		damageFiller = GetComponentsInChildren<Image>()[1]; // the foreground bar
	}

	// Update is called once per frame
	void Update () {
		transform.LookAt(mainCam.transform);

		damageFiller.fillAmount = Mathf.Clamp01(Mathf.InverseLerp(Min.Value, Max.Value, resource.Value));
	}
}
