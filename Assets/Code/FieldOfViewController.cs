using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rbots.Characters
{
	public class FieldOfViewController : MonoBehaviour
	{
		// targetting variables (for player)
		public float targettingRadius;
		//[Range(0,360)]
		//public float targettingAngle;
		public LayerMask targetMask;
		public LayerMask obstacleMask;
		public Vector3 direction;

		public List<Transform> visibleTargets = new List<Transform>();

		// known target sight check
		public GameObject Target; // Maybe not only the player !
		[Tooltip("Max view angle")]
		[SerializeField] float fieldOfView = 120; // Angle

		private float chaseRange;
		private RaycastHit hit;

		private void Start()
		{
			if (GetComponentInParent<EnemyController>())
				chaseRange = GetComponentInParent<EnemyController>().chaseRange;
			if (GetComponentInParent<Player>())
				chaseRange = GetComponentInParent<Player>().attackRadius;

			StartCoroutine("FindTargetsWithDelay", .2f);
		}

		// if known target
		public bool CanSeeTarget()
		{
			// reset y position of target to avoid bad angle return
			Vector3 newTargetPos = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
			// get the direction of the target based on resetted y
			Vector3 fixedDirection = (newTargetPos - transform.position);
			// get the angle between target and eye
			float angle = Mathf.Abs(Vector3.SignedAngle(transform.forward, fixedDirection, Vector3.right));

			// get the correct direction
			direction = Target.transform.position - transform.position + Vector3.up;
			
			Debug.DrawRay(transform.position, direction, Color.red);
			// 'cast a ray' from eye to player direction
			if (angle < fieldOfView / 2 && Physics.Raycast(transform.position, direction, out hit, chaseRange)) {
				// check collider to avoid return true on hitting ground (could use Layer too, should ? TODO)
				if (hit.collider.gameObject == Target) {
					return true;
				}
			}

			return false;
		}

		// Player targetting
		IEnumerator FindTargetsWithDelay(float delay)
		{
			while(true) {
				yield return new WaitForSeconds(delay);
				FindVisibleTargets();
			}
		}

		void FindVisibleTargets()
		{
			visibleTargets.Clear();
			Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, targettingRadius, targetMask);

			for (int i = 0; i < targetsInViewRadius.Length; i++) {
				Transform target = targetsInViewRadius[i].transform;

				if (!target.GetComponent<EnemyController>().isDead) {
					Vector3 dirToTarget = (target.position - transform.position).normalized;

					visibleTargets.Add(target);
				}


				//if (Vector3.Angle (transform.forward, dirToTarget) < targettingAngle / 2) {
				//	float distToTarget = Vector3.Distance(transform.position, target.position);
				//	visibleTargets.Add(target);
				//	//if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask)) {
				//	//	visibleTargets.Add(target);
				//	//}
				//}
			}
		}

		public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
		{
			if (!angleIsGlobal)
				angleInDegrees += transform.eulerAngles.y;

			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}
	}
}
