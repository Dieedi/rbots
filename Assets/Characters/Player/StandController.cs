using System.Collections;
using UnityEngine;
using FloatingBars;

public class StandController : MonoBehaviour {

	[SerializeField] GameObject Player;
	[SerializeField] GameObject RegenParticle;
	
	AudioSource audioSource;

	//=============================
	// HEALTH
	//=============================
	[SerializeField] FloatVariable RegenRate;

	FloatVariable HP;
	FloatVariable StartingHP;
	FloatVariable MinHP;
	FloatingBarController fbc;
	bool regenerating = false;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	private void OnTriggerEnter(Collider other)
	{
		RegenParticle.SetActive(true);
		audioSource.Play();
		
		// TODO Player choice ?
		regenerating = true;

		fbc = Player.GetComponentInChildren<FloatingBarController>();
		HP = fbc.resource;
		StartingHP = fbc.Max;

		//=============================
		// HEALTH - Regeneration Start
		//=============================
		if (HP.Value < StartingHP.Value && regenerating) {
			StartCoroutine("RegenHP");
		}
	}

	private void OnTriggerStay(Collider other)
	{
	}

	private void OnTriggerExit(Collider other)
	{
		RegenParticle.SetActive(false);
		audioSource.Stop();
		regenerating = false;
	}

	//=============================
	// HEALTH - Regeneration
	//=============================
	IEnumerator RegenHP()
	{
		while (HP.Value < StartingHP.Value) {
			yield return new WaitForSeconds(1f);

			HP = fbc.resource;

			if (HP.Value >= StartingHP.Value) {
				yield break;
			}
			else {
				HP.ApplyChange(RegenRate);
			}
		}
	}
}
