using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Rbots.Characters
{
	[RequireComponent(typeof(CharacterController))]
	public class PlayerControls : MonoBehaviour
	{
		[SerializeField] List<GameObject> playerTypes;
		[SerializeField] float Speed = 1;
		[SerializeField] float GroundDistance = 0.1f;
		[SerializeField] LayerMask Ground;
		[SerializeField] float jumpForce = 10f;
		[SerializeField] Ability[] abilities = new Ability[2];

		PlayerAnimations playerAnim;
		float verticalVelocity;
		float Gravity = 14f;
		float currentGravity = 0f;
		float desiredSpeed = 0f;
		float currentSpeed = 0f;
		CharacterController _controller;
		float verticalInput;
		float horizontalInput;
		Vector3 forward;
		Vector3 right;
		Vector3 moveDirection;
		GameObject currentPlayerType;
		Player c_Player;
		bool changeInProgress = false;

		private void Awake()
		{
			currentPlayerType = playerTypes[0];
			ChangeBody();
		}

		private void ChangeBody()
		{
			GameObject PlayerBody = GameObject.FindGameObjectWithTag("PlayerBody");
			if (PlayerBody) {
				Destroy(PlayerBody);
			}

			PlayerBody = Instantiate(currentPlayerType, gameObject.transform);
			playerAnim = PlayerBody.GetComponent<PlayerAnimations>();

			changeInProgress = false;
		}

		void Start()
		{
			_controller = GetComponent<CharacterController>();
			c_Player = GetComponent<Player>();

			// gravity value has to be absolute
			Gravity = Mathf.Abs(Gravity);
			playerAnim.PowerOn();
		}

		void Update()
		{
			ApplyMoves();
			ApplyActions();
		}

		void ApplyMoves()
		{
			horizontalInput = Input.GetAxisRaw("Horizontal");
			verticalInput = Input.GetAxisRaw("Vertical");

			if (_controller.isGrounded) {
				forward = Camera.main.transform.TransformDirection(Vector3.forward);
				forward.y = 0;
				forward = forward.normalized;

				right = new Vector3(forward.z, 0, -forward.x);
				moveDirection = (horizontalInput * right + verticalInput * forward).normalized;

				desiredSpeed = Speed;
				moveDirection *= Mathf.Lerp(currentSpeed, desiredSpeed, Time.deltaTime);

				verticalVelocity = -Gravity * Time.deltaTime;
				if (Input.GetButtonDown("Jump")) {
					verticalVelocity = jumpForce;
				}
			} else {
				verticalVelocity -= Gravity * Time.deltaTime;
			}

			moveDirection.y = verticalVelocity;

			_controller.Move(moveDirection * Time.deltaTime);

			// check current movement if no move don't change rotation
			Vector3 lookDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
			currentSpeed = lookDirection.magnitude;

			// Turn to the direction we want to go
			if (moveDirection.x != 0 && moveDirection.z != 0)
				LookTo(lookDirection);

			playerAnim.SetMoveSpeed(currentSpeed);
		}

		public void LookTo(Vector3 lookDirection)
		{
			transform.rotation = Quaternion.LookRotation(lookDirection);
		}

		void ApplyActions()
		{
			if (Input.GetButton("Fire1") && c_Player.CanAttack(abilities[0].EnergyCost)) {
				c_Player.CalculateDamage(abilities[0].DamageMultiplier);
				playerAnim.ApplyAttack(abilities[0].AnimatorParameter);
			}
			if (Input.GetButtonUp("Fire1")) {
				playerAnim.StopAttack(abilities[0].AnimatorParameter);
			}

			if (Input.GetButtonDown("Fire2") && c_Player.CanAttack(abilities[1].EnergyCost)) {
				c_Player.CalculateDamage(abilities[1].DamageMultiplier);
				playerAnim.ApplyAttack(abilities[1].AnimatorParameter);
			}
			if (Input.GetButtonUp("Fire2")) {
				playerAnim.StopAttack(abilities[1].AnimatorParameter);
			}

			if (Input.GetKey(KeyCode.P)) {
				playerAnim.PowerOff();
			}
			if (Input.GetKey(KeyCode.O)) {
				playerAnim.PowerOn();
			}
		}

		public void ChangeRbot(PlayerTypes type)
		{
			currentPlayerType = playerTypes[(int)type];

			ChangeBody();
		}


		// TODO remove that from here !!!
		public CursorLockMode wantedMode;

		// Apply requested cursor state
		void SetCursorState()
		{
			Cursor.lockState = wantedMode;
			// Hide cursor when locking
			Cursor.visible = (CursorLockMode.Locked != wantedMode);
		}

		void OnGUI()
		{
			GUILayout.BeginVertical();
			// Release cursor on escape keypress
			if (Input.GetKeyDown(KeyCode.Escape))
				Cursor.lockState = wantedMode = CursorLockMode.None;

			switch (Cursor.lockState) {
				case CursorLockMode.None:
					GUILayout.Label("Cursor is normal");
					if (GUILayout.Button("Lock cursor"))
						wantedMode = CursorLockMode.Locked;
					if (GUILayout.Button("Confine cursor"))
						wantedMode = CursorLockMode.Confined;
					break;
				case CursorLockMode.Confined:
					GUILayout.Label("Cursor is confined");
					if (GUILayout.Button("Lock cursor"))
						wantedMode = CursorLockMode.Locked;
					if (GUILayout.Button("Release cursor"))
						wantedMode = CursorLockMode.None;
					break;
				case CursorLockMode.Locked:
					GUILayout.Label("Cursor is locked");
					if (GUILayout.Button("Unlock cursor"))
						wantedMode = CursorLockMode.None;
					if (GUILayout.Button("Confine cursor"))
						wantedMode = CursorLockMode.Confined;
					break;
			}

			GUILayout.EndVertical();

			SetCursorState();
		}
		// end todo
	}
}
