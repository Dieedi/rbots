﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Launch interactions
public class Interactable : MonoBehaviour
{
	[HideInInspector]
	public static NavMeshAgent movingNavMeshAgent;

	[HideInInspector]
	public static bool hasInteracted;

	private float movingAgentRadius;

	private void Start()
	{
	}

	void Update()
	{
		if (hasInteracted) { 
			Interact();
		}
	}

	public void Interact()
	{
		print("Interacting with base class.");
		// TODO Maybe wait to user or game action before resetting...
		StartCoroutine("WaitInteractionEnd");
	}

	IEnumerator WaitInteractionEnd()
	{
		yield return new WaitForSeconds(1);

		print("interaction done.");
		hasInteracted = false;
		movingNavMeshAgent.GetComponent<MovingAgentController>().ResetAgentValues();
	}
}
