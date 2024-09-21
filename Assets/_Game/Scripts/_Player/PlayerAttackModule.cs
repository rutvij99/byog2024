using System;
using UnityEngine;

public class PlayerAttackModule : MonoBehaviour
{

	private AttackInfo currentAttackInfo;

	public void SetAttackInfo(AttackInfo info)
	{
		currentAttackInfo = info;
	}
	private void OnTriggerEnter(Collider other)
	{
		var target = other.transform.GetComponentInParent<PlayerTarget>();
		if (target != null)
		{
			Debug.Log($"Player attacked -> {other.gameObject.name}");
			target.TakeDamage(currentAttackInfo.damage);
		}
	}
}
