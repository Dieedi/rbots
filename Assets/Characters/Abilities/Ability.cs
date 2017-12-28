using Rbots.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rbots.Characters
{
	[CreateAssetMenu(menuName = ("Rbots/New Ability"))]
	public class Ability : ScriptableObject
	{
		[Header("Ability General")]
		[SerializeField] float energyCost = 10f;
		public float EnergyCost { get { return this.energyCost; } }

		[SerializeField] float damageMultiplier = 2f;
		public float DamageMultiplier { get { return this.damageMultiplier; } }

		[Tooltip("Exact name of parameter used to toggle attack in animator")]
		[SerializeField] string animatorParameter;
		public string AnimatorParameter { get { return this.animatorParameter; } }
	}
}
