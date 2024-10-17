/*
*  Author: rutvij
*  Created On: 9/24/2024 2:50:16 AM
* 
*  Copyright (c) 2024 CosmicPull
*/

using UnityEngine;

namespace Soulslike.Player
{
    public class PlayerEvents : MonoBehaviour
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
