using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	[SerializeField] FieldOfViewController myEye;

	// TODO chasing target
	private bool isChasingTarget = false;
	private GameObject CurrentTarget;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (myEye.CanSeeTarget() || (isChasingTarget && CurrentTarget != null)) {
			isChasingTarget = true;
			CurrentTarget = myEye.Target;
			GetComponent<MovingAgentController>().MoveToInteract(CurrentTarget);
		}
	}
}
