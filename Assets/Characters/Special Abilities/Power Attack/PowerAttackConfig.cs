using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rbots.Characters
{
	[CreateAssetMenu(menuName = ("Rbots/Special Ability/Power Attack"))]
	public class PowerAttackConfig : SpecialAbilityConfig
	{
		[Header("Power Attack Specifics")]
		[SerializeField] float damageMultiplier = 10f;

		public override ISpecialAbility AttachComponentTo(GameObject gameObjectToAttachTo)
		{
			var behaviourComponent = gameObjectToAttachTo.AddComponent<PowerAttackBehaviour>();
			behaviourComponent.SetConfig(this);

			return behaviourComponent;
		}
	}
}