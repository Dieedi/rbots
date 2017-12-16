using UnityEngine;

public class FieldOfViewController : MonoBehaviour {

	public GameObject Target; // Maybe not only the player !
	[Tooltip("Max view angle")]
	[SerializeField] float fieldOfView = 120; // Angle

	private float chaseRange;
	private RaycastHit hit;

	private void Start()
	{
		if (GetComponentInParent<EnemyController>())
			chaseRange = GetComponentInParent<EnemyController>().chaseRange;
	}

	public bool CanSeeTarget () {
		// reset y position of target to avoid bad angle return
		Vector3 newTargetPos = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
		// get the direction of the target based on resetted y
		Vector3 fixedDirection = (newTargetPos - transform.position);
		// get the angle between target and eye
		float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, fixedDirection, Vector3.right));

		// get the correct direction
		Vector3 direction = Target.transform.position - transform.position + Vector3.up;
		// 'cast a ray' from eye to player direction
		if (angle < fieldOfView / 2 && Physics.Raycast(transform.position, direction, out hit, chaseRange)) {
			// check collider to avoid return true on hitting ground (could use Layer too, should ? TODO)
			if (hit.collider.gameObject == Target) {
				return true;
			}
		}

		return false;
	}
}
