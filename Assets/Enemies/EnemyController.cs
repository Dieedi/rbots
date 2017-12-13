using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour, IDamageable
{
	[SerializeField] FieldOfViewController myEye;

	public bool ResetHP;
	public FloatVariable RegenRate;

	private FloatVariable HP;
	private FloatVariable StartingHP;
	private FloatVariable MinHP;

	// TODO chasing target
	private bool isChasingTarget = false;
	private GameObject CurrentTarget;
	private Animator anim;
	private NavMeshAgent agent;

	// Use this for initialization
	void Start()
	{
		FloatingBarController fbc = GetComponentInChildren<FloatingBarController>();
		HP = fbc.resource;
		StartingHP = fbc.Max;
		MinHP = fbc.Min;

		if (ResetHP)
			HP.SetValue(StartingHP);

		anim = GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update()
	{
		anim.SetFloat("EnemyVelocity", Mathf.Abs(agent.velocity.x));

		if (myEye.CanSeeTarget() || (isChasingTarget && CurrentTarget != null)) {
			isChasingTarget = true;
			CurrentTarget = myEye.Target;
			GetComponent<MovingAgentController>().MoveToInteract(CurrentTarget);
		}
	}

	public void HandleInteraction(GameObject target)
	{
		if (target.name == "Player") {
			anim.SetBool("IsAttacking", true);
		}
	}

	public void IsAttackedBy(GameObject target)
	{
		transform.LookAt(target.transform);
		anim.SetBool("IsAttacking", true);
	}

	public void TakeDamage(float amount)
	{
		float newValue = HP.Value - amount;
		HP.ApplyChange(-amount);
	}
}
