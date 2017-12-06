using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public FloatVariable HP;
	public bool ResetHP;
	public FloatVariable StartingHP;
	public FloatVariable RegenRate;

	private bool regenerating = false;

	// Use this for initialization
	void Start () {
		if (ResetHP)
			HP.SetValue(StartingHP);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (HP.Value < StartingHP.Value && !regenerating) {
			regenerating = true;
			StartCoroutine("RegenHP");
		}
	}

	IEnumerator RegenHP ()
	{
		while (HP.Value < StartingHP.Value) {
			yield return new WaitForSeconds(1f);

			if (HP.Value >= StartingHP.Value)
				regenerating = false;
			else
				HP.ApplyChange(RegenRate.Value);
		}
	}
}
