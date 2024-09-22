using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
	Entity.Entity entity;
	private bool isLocked;
	private void Awake()
	{
		entity = GetComponent<Entity.Entity>();
	}

	public void SetLock(bool locked)
	{
		isLocked = locked;
		PlayerUI.Instance.ShowTracker(locked);
	}

	private void Update()
	{
		if (isLocked)
		{
			PlayerUI.Instance.UpdateLockPoisiton(transform.position + new Vector3(0,2,0));
		}
	}

	public void TakeDamage(float damage)
	{
		Debug.Log($"Enemy taking damage {damage}");
		if(entity != null)
			entity.TakeDamage((int)damage);
	}
}
