/*
*  Author: rutvij
*  Created On: 9/24/2024 1:59:32 AM
* 
*  Copyright (c) 2024 CosmicPull
*/

using UnityEngine;

namespace Soulslike.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerController _player;
        private Rigidbody _rb;
        private CharacterController _cc;
        
        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _rb = GetComponent<Rigidbody>();
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
