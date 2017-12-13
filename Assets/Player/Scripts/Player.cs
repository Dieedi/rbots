using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour {

	[SerializeField] FieldOfViewController myEye;

	public FloatVariable HP;
	public bool ResetHP;
	public FloatVariable StartingHP;
	public FloatVariable RegenRate;

	private bool regenerating = false;
	private Animator anim;
	private NavMeshAgent agent;
	private bool isAttacking = false;
	private GameObject myTarget;
	private GameObject enemyTarget;

	// Use this for initialization
	void Start () {
		if (ResetHP)
			HP.SetValue(StartingHP);

		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (HP.Value < StartingHP.Value && !regenerating) {
			regenerating = true;
			StartCoroutine("RegenHP");
		}

		if (myTarget && !myEye.CanSeeTarget()) {
			transform.LookAt(myTarget.transform);
		}

		if (enemyTarget) {
			myTarget.GetComponent<EnemyController>().IsAttackedBy(gameObject);
		}

		anim.SetFloat("PlayerVelocityX", Mathf.Abs(agent.velocity.x));
	}

	public void HandleInteraction(GameObject target)
	{
		myTarget = target;
		myEye.Target = myTarget;

		if (target.layer == LayerMask.NameToLayer("Enemy")) {
			enemyTarget = target;
			isAttacking = true;
			anim.SetBool("IsAttacking", isAttacking);
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
