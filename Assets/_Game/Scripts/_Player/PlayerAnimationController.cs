using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
	// Reference to the Animator component
	[SerializeField] private Animator animator;
	// The index of the full-body layer in the Animator
	private int fullBodyLayerIndex = 2; // Set this to the correct index of your "FullBodyCombat" layer

	void Start()
	{
		// Get the Animator component attached to the player
		// animator = GetComponent<Animator>();
	}

	// This method sets the x_movement value (movement state: -1, 0, 1, 2)
	public void SetMovement(float x_movement)
	{
		animator.SetFloat("x_movement", x_movement);
	}

	// This method sets the grounded state (true if grounded, false if not)
	public void SetGrounded(bool isGrounded)
	{
		animator.SetBool("isGrounded", isGrounded);
	}

	// This method triggers the jump animation
	public void TriggerJump(bool isDoubleJump)
	{
		animator.SetTrigger(isDoubleJump ? "doubleJump" : "jump");
	}

	// These methods trigger dash animations for forward and backward
	public void TriggerDash(bool isForward)
	{
		if(isForward)
			animator.SetTrigger("dashForward");
		else
			animator.SetTrigger("dashBackward");
	}

	// These methods trigger dodge roll animations for forward and backward
	public void TriggerDodgeRoll(bool isForward)
	{
		if (isForward)
			animator.SetTrigger("dodgeRollForward");
		else
			animator.SetTrigger("dodgeRollBackward");
	}

	// Trigger attack animation based on combo step, playing a state repeatedly
	public void TriggerAttack(AttackInfo attackInfo)
	{
		// Ensure the attack state corresponds to the combo step
		string attackStateName = attackInfo.attackId;
		// Crossfade to the desired attack state in the FullBodyCombat layer (layer index 1)
		animator.CrossFadeInFixedTime(attackStateName, 0.1f, fullBodyLayerIndex);
	}


	public void TriggerHit()
	{
		animator.SetTrigger("hit");
	}
	
	public void SetStun(bool stunned)
	{
		animator.SetBool("isStunned", stunned);
	}
	
	public void TriggerDeath()
	{
		animator.SetTrigger("death");
	}

	public void SetBlock(bool blocking)
	{
		animator.SetBool("isBlocking", blocking);
	}

}