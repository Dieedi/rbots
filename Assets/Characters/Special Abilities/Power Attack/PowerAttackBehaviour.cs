using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rbots.Characters
{
	public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
	{
		PowerAttackConfig config;

		public void SetConfig(PowerAttackConfig configToSet)
		{
			this.config = configToSet;
		}

		public void Use()
		{
			throw new NotImplementedException();
		}
	}
}