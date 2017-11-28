using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimationManager : MonoBehaviour {

	private NavMeshAgent navMeshAgent;
	private Animator anim;
	private bool isWalking;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
		//if (navMeshAgent.velocity != Vector3.zero) {
		//	isWalking = true;
		//} else {
		//	isWalking = false;
		//}

		//anim.SetBool("IsWalking", isWalking);

		anim.SetFloat("PlayerVelocity", navMeshAgent.velocity.magnitude);
	}
}
