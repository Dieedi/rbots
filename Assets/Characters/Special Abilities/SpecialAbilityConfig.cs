using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rbots.Characters
{
	public abstract class SpecialAbilityConfig : ScriptableObject
	{
		[Header("Special Ability General")]
		[SerializeField] float energyCost = 10f;

		abstract public ISpecialAbility AttachComponentTo(GameObject gameObjectToAttachTo);
	}
}