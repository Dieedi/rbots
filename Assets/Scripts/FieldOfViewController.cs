using UnityEngine;

public class FieldOfViewController : MonoBehaviour {

	public GameObject Target; // Maybe not only the player !
	[SerializeField] float fieldOfView = 120; // Angle
	
	public bool CanSeeTarget () {
		Debug.Log("I see you");
		// reset z position of target to avoid bad angle return
		Vector3 newTargetPos = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
		// get the direction of the target
		Vector3 direction = (newTargetPos - transform.position);
		// get the angle between target and eye
		float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, direction, Vector3.right));

		// cast a ray from eye to player direction
		if (angle < fieldOfView / 2) {
			return true;
		}

		return false;
	}
}
