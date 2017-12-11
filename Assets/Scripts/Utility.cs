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
}