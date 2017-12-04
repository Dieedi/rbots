using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public FloatVariable HP;
	public bool ResetHP;
	public FloatVariable StartingHP;
	public FloatVariable RegenRate;

	// Use this for initialization
	void Start () {
		if (ResetHP)
			HP.SetValue(StartingHP);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (HP.Value < StartingHP.Value) {
			StartCoroutine("RegenHP");
		}
	}

	IEnumerator RegenHP ()
	{
		while (HP.Value < StartingHP.Value) {
			HP.ApplyChange(RegenRate.Value);
			yield return new WaitForSeconds(1f);
		}
	}
}
