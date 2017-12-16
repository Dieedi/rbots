using UnityEngine;
using UnityEngine.AI;

namespace Utility
{
	public enum Layer
	{
		Enemy = 9,
		RbotsWalkable = 10,
		RaycastEndStop = -1
	}

	public enum Interactables
	{
		Player,
		GroundEnemy,
		FlyingEnemy,
		Object,
		NPC
	}

	public class AttackTypeController
	{
		float closeRadius = 3;
		float rangeRadius = 10;

		public float CloseRadius { get { return closeRadius; } }
		public float RangeRadius { get { return rangeRadius; } }

		public enum AttackType
		{
			closed,
			ranged
		}


	};
	
	//public class GetAgentCenter
	//{
	//	public Vector3 this[GameObject agent] {
	//		get {
	//			NavMeshAgent navMeshAgent = agent.GetComponent<NavMeshAgent>();
	//			Vector3 topPos = (agent.gameObject.transform.position + new Vector3(0f, navMeshAgent.height, 0f)).normalized;
	//			Debug.Log(topPos + "agent.gameObject.transform.position");
	//			return topPos;
	//		}
	//	}
	//}
}