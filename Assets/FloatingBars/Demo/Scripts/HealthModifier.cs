using UnityEngine;
using UnityEngine.UI;

namespace FloatingBars
{
	public class HealthModifier : MonoBehaviour
	{

		public GameObject capsule;

		private FloatVariable capsuleHP;
		private FloatVariable maxHP;
		private Slider slider;

		// Use this for initialization
		void Start()
		{
			capsuleHP = capsule.GetComponent<FloatingBarVariableController>().currentValue;
			maxHP = capsule.GetComponent<FloatingBarVariableController>().startingValue;
			slider = GetComponent<Slider>();
		}

		// Update is called once per frame
		void Update()
		{
			capsuleHP.SetValue(slider.value * maxHP.Value);
		}
	}
}
