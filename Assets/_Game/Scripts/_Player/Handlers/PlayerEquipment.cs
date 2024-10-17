/*
*  Author: rutvij
*  Created On: 9/26/2024 6:11:33 PM
* 
*  Copyright (c) 2024 CosmicPull
*/

using UnityEngine;

namespace Soulslike.Player
{
    public class PlayerEquipment : MonoBehaviour
    {
        private PlayerController _player;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();
        }
    
        internal void Initialize()
        {
            
        }
    
        private void Update()
        {
            
        }
    }
}
