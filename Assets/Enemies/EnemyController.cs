using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
	[SerializeField] FieldOfViewController myEye;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (myEye.CanSeeTarget()) {
			GetComponent<NavMeshAgent>().SetDestination(myEye.Target.transform.position);
		}
	}
}
