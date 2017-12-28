using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandUIController : MonoBehaviour {

	Camera mainCam;

	// Use this for initialization
	void Start ()
	{
		mainCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Always view the bar
		transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
	}
}
