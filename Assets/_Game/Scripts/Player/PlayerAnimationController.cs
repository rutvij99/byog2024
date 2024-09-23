using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAnimationController : MonoBehaviour
{
	[SerializeField] private Animator animator;

	[FormerlySerializedAs("meleeObjects")] [SerializeField] private List<GameObject> swordShieldObjects;
	[FormerlySerializedAs("rangeObjects")] [SerializeField] private List<GameObject> wandObjects;
	[SerializeField] private List<GameObject> greatSwordObjects;
	
	private int upperBodyLayerIndex = 1; 
	private int fullBodyLayerIndex = 2;

	public void SetMovement(float x_movement)
	{
		animator.SetFloat("x_movement", x_movement);
	}

	public void SetGrounded(bool isGrounded)
	{
		animator.SetBool("isGrounded", isGrounded);
	}

	public void TriggerJump(bool isDoubleJump)
	{
		animator.SetTrigger(isDoubleJump ? "doubleJump" : "jump");
	}

	public void TriggerDash(bool isForward)
	{
		if(isForward)
			animator.SetTrigger("dashForward");
		else
			animator.SetTrigger("dashBackward");
	}

	public void TriggerDodgeRoll(bool isForward)
	{
		if (isForward)
			animator.SetTrigger("dodgeRollForward");
		else
			animator.SetTrigger("dodgeRollBackward");
	}

	public void TriggerAttack(AttackInfo attackInfo)
	{
		string attackStateName = attackInfo.attackId;
		animator.CrossFadeInFixedTime(attackStateName, 0.1f, fullBodyLayerIndex);
	}
	
	public void SwitchWeapon(WeaponData data)
	{
		if (animator.runtimeAnimatorController != data.animatorController)
		{
			animator.runtimeAnimatorController = data.animatorController;
		}
		foreach (GameObject obj in swordShieldObjects)
		{
			obj.SetActive(false);
		}
		foreach (GameObject obj in wandObjects)
		{
			obj.SetActive(false);
		}
		foreach (GameObject obj in greatSwordObjects)
		{
			obj.SetActive(false);
		}
		Sequence sequence = DOTween.Sequence()
			.AppendInterval(0.55f)
			.AppendCallback(() =>
			{
				foreach (GameObject obj in swordShieldObjects)
				{
					obj.SetActive(data.weaponType == WeaponType.SwordShield);
				}
				foreach (GameObject obj in wandObjects)
				{
					obj.SetActive(data.weaponType == WeaponType.Wand);
				}
				foreach (GameObject obj in greatSwordObjects)
				{
					obj.SetActive(data.weaponType == WeaponType.GreatSword);
				}
				// Notify the UI about the weapon change
				PlayerUI.Instance.UpdateWeaponUI();
			});
		animator.CrossFadeInFixedTime("SwitchWeapon", 0.1f, upperBodyLayerIndex);
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