using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBarsController : MonoBehaviour {

	Camera mainCam;

	// Use this for initialization
	void Start ()
	{
		mainCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(mainCam.transform);
	}
}
