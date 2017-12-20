using UnityEngine;

namespace Rbots.Characters
{
	[RequireComponent(typeof(PlayerAnimations))]
	[RequireComponent(typeof(CharacterController))]
	public class PlayerControls : MonoBehaviour
	{
		public float Speed = 1;
		public float GroundDistance = 0.1f;
		public LayerMask Ground;

		PlayerAnimations playerAnim;
		float verticalVelocity;
		float Gravity = 14f;
		float jumpForce = 10f;
		float currentGravity = 0f;
		float desiredSpeed = 0f;
		float currentSpeed = 0f;
		CharacterController _controller;
		float verticalInput;
		float horizontalInput;
		Vector3 forward;
		Vector3 right;
		Vector3 moveDirection;

		void Start()
		{
			_controller = GetComponent<CharacterController>();
			playerAnim = GetComponent<PlayerAnimations>();

			// gravity value has to be absolute
			Gravity = Mathf.Abs(Gravity);
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
			if (Input.GetButton("Fire1")) {
				playerAnim.ApplyBaseAttack();
			}
			if (Input.GetButtonUp("Fire1")) {
				playerAnim.StopBaseAttack();
			}
		}
	}
}
