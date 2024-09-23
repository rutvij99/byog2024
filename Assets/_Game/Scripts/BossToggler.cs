using System;
using Bosses;
using Unity.VisualScripting;
using UnityEngine;

public class BossToggler : MonoBehaviour
{
   [SerializeField] 
   private BossBase boss;
   
   [SerializeField] 
   private AudioClip ambiance;

   private bool toggled = false;
   
   private void Start()
   {
      AudioManager.PlayBackgroundMusic(null);
      AudioManager.PlayAmbiance(ambiance);
      boss = FindAnyObjectByType<BossBase>();
   }

   private void OnTriggerEnter(Collider other)
   {
      if (boss && !toggled)
      {
         toggled = true;
         boss.ActivateBoss();
         Destroy(this.gameObject, 0.5f);
      }
   }
}
