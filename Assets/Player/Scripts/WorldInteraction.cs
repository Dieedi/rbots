using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldInteraction : MonoBehaviour
{
	NavMeshAgent playerNavMeshAgent;
	RaycastHit interactionHit;

	void Start()
	{
		playerNavMeshAgent = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
			GetInteraction();
		}
	}

	void GetInteraction()
	{
		Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(interactionRay, out interactionHit)) {
			GameObject interactedObject = interactionHit.collider.gameObject;
			if (interactedObject.tag == "Interactable") {
				interactedObject.GetComponent<Interactable>().MoveToInteract(playerNavMeshAgent);
				// playerNavMeshAgent.destination = interactionHit.point;
			} else {
				playerNavMeshAgent.destination = interactionHit.point;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(playerNavMeshAgent.transform.position, playerNavMeshAgent.destination);
		Gizmos.DrawSphere(interactionHit.point, .1f);
	}
}
