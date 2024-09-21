using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
	Entity.Entity entity;
	private void Awake()
	{
		entity = GetComponent<Entity.Entity>();
	}

	public void SetLock(bool locked)
	{
		
	}

	public void TakeDamage(float damage)
	{
		Debug.Log($"Enemy taking damage {damage}");
		if(entity != null)
			entity.TakeDamage((int)damage);
	}
}
