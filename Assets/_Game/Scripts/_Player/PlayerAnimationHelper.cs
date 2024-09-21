using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-98)]
public class PlayerAnimationHelper : MonoBehaviour
{
	private PlayerStats playerStats;
	private PlayerController _controller;
	private PlayerAttackModule attackModule;

	private List<Collider> _colliders = new List<Collider>();
	private WeaponType currentType = WeaponType.None;
	private void Start()
	{
		playerStats = this.transform.GetComponentInParent<PlayerStats>();
		_controller = this.transform.GetComponentInParent<PlayerController>();
		attackModule = this.transform.GetComponentInParent<PlayerAttackModule>();
		currentType = playerStats.currentWeaponType;
		foreach (var collider in _colliders)
		{
			collider.enabled = false;
		}
		_colliders.Clear();
		foreach (var trigger in playerStats.currentWeaponData.triggerObjs)
		{
			var col = FindDeepChild<Collider>(this.transform, trigger);
			col.enabled = false;
			if (col != null)
				_colliders.Add(col);
		}
	}

	private void Update()
	{
		if (currentType != playerStats.currentWeaponType)
		{
			currentType = playerStats.currentWeaponType;
			foreach (var collider in _colliders)
			{
				collider.enabled = false;
			}
			_colliders.Clear();
			foreach (var trigger in playerStats.currentWeaponData.triggerObjs)
			{
				var col = FindDeepChild<Collider>(this.transform, trigger);
				col.enabled = false;
				if (col != null)
					_colliders.Add(col);
			}
		}
	}

	public void SpawnProjectile(string projectileSpawnPoint)
	{
		var pointMain = FindDeepChild(this.transform, projectileSpawnPoint);
		var point = new GameObject();
		point.transform.position = _controller.transform.position + new Vector3(0, 1f, 0);
		if (pointMain != null)
		{
			point.transform.position = pointMain.transform.position;
		}
		
		attackModule.SpawnProjectile(point);
	}

	public void EnableTrigger(string triggerName)
	{
		Debug.Log($"Enable Trigger Called {triggerName}");
		foreach (var col in _colliders)
		{
			col.enabled = true;
		}
	}

	public void DisableTrigger(string triggerName)
	{
		Debug.Log($"Disable Trigger Called {triggerName}");
		foreach (var col in _colliders)
		{
			col.enabled = false;
		}
	}

	public static T FindDeepChild<T>(Transform aParent, string aName, bool exactMatch = true) where T : Component
	{
		var transform = FindDeepChild(aParent, aName, exactMatch);
		if (transform != null)
		{
			return transform.GetComponent<T>();
		}

		return null;
	}

	public static Transform FindDeepChild(Transform aParent, string aName, bool exactMatch = true)
	{
		if (aParent == null)
		{
			return null;
		}

		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			var c = queue.Dequeue();
			if (c == null) continue;
			if (exactMatch)
			{
				if (c.name == aName)
				{
					return c;
				}
			}
			else
			{
				if (c.name.Contains(aName))
				{
					return c;
				}
			}

			foreach (Transform t in c)
				queue.Enqueue(t);
		}

		return null;
	}

	public static List<Transform> FindDeepChildsWithName(Transform aParent, string aName, bool exactMatch)
	{
		List<Transform> allT = new List<Transform>();

		Queue<Transform> queue = new Queue<Transform>();
		queue.Enqueue(aParent);
		while (queue.Count > 0)
		{
			var c = queue.Dequeue();
			if (exactMatch)
			{
				if (c.name == aName)
				{
					allT.Add(c);
				}
			}
			else
			{
				if (c.name.Contains(aName))
				{
					allT.Add(c);
				}
			}

			foreach (Transform t in c)
				queue.Enqueue(t);
		}

		return allT;
	}
}