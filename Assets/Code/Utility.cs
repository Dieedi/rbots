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

	public enum PlayerTypes
	{
		Red,
		Yellow,
		Purple
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
}