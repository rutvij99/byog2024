using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
	// Reference to the Animator component
	[SerializeField] private Animator animator;

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

	public void TriggerAttack(int comboStep)
	{
		animator.SetInteger("comboCounter", comboStep);
		animator.SetTrigger("attack");
		animator.SetBool("isAttacking", true);
	}
	public void ResetAttack()
	{
		animator.SetBool("isAttacking" , false);
		// animator.SetInteger("comboCounter", 0);
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