using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowController : MonoBehaviour {

	// camera target
	public Transform Player;

	// starting distance
	public float distance = 7.0f;

	// Camera controls
	public float camXSpeed = 3.0f;
	public float camYSpeed = 3.0f;
	public float yMinLimit = 20f;
	public float yMaxLimit = 80f;
	public float distanceMin = 3f;
	public float distanceMax = 10f;

	private float x = 0.0f;
	private float y = 0.0f;
	private float obstacleZoomSpeed = 0.1f;
	private float playerTopOffset = 1.0f;

	private Vector3 playerPos;
	private Quaternion cameraRotation;

	// Use this for initialization
	void Start ()
	{
		playerPos = Player.position;

		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
		cameraRotation = Quaternion.Euler(y, x, 0);
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		playerPos = new Vector3(
			Player.position.x,
			Player.position.y + playerTopOffset,
			Player.position.z
		);

		if (Input.GetMouseButton(1)) 
		{
			// Rotate around
			x += Input.GetAxis("Mouse X") * camXSpeed;
			y -= Input.GetAxis("Mouse Y") * camYSpeed;

			y = ClampAngle(y, yMinLimit, yMaxLimit);

			cameraRotation = Quaternion.Euler(y, x, 0);
		}

		distance = Mathf.Clamp(
			distance - Input.GetAxis("Mouse ScrollWheel") * 5,
			distanceMin,
			distanceMax
		);
		
		RaycastHit hit;
		if (Physics.Linecast(playerPos, transform.position, out hit)) {
			distance -= hit.distance * obstacleZoomSpeed;
		}

		Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
		Vector3 position = cameraRotation * negDistance + playerPos;

		transform.rotation = cameraRotation;
		transform.position = position;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}
}
