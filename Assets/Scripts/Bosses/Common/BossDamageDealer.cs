using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bosses.Common
{
    public class BossDamageDealer : MonoBehaviour
    {
        public virtual void DealDamage(int damage)
        {
            Debug.Log("Dealing Damage");
        }
        
    }
}