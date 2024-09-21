using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerWeaponData", menuName = "Scriptable Objects/PlayerAttackData")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType;
    public RuntimeAnimatorController animatorController;
    public List<AttackInfo> airAttacks;
    public List<AttackInfo> lightMeleeAttacks;
    public List<AttackInfo> heavyMeleeAttacks;
    public float maxWalkSpeed;
    public float maxRunSpeed;
}
