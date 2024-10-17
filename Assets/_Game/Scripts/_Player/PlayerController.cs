/*
*  Author: rutvij
*  Created On: 9/24/2024 12:50:43 AM
* 
*  Copyright (c) 2024 CosmicPull
*/

using System;
using DG.Tweening;
using Soulslike.Common.Helpers;
using UnityEngine;

namespace Soulslike.Player
{
    public class PlayerController : SingletonMonoBehaviour<PlayerController>
    {
        public PlayerStateMachine StateMachine { get; private set; }

        
        // Components
        public PlayerStats Stats { get; private set; }
        public InputHandler InputHandler { get; private set; }
        public PlayerMovement Movement { get; private set; }
        public PlayerCombat Combat { get; private set; }
        public PlayerAnimationController AnimationController { get; private set; }
        public PlayerAudio Audio { get; private set; }
        public PlayerEvents Events { get; private set; }
        
        private void Awake()
        {
            DOTween.Init();
            StateMachine = new PlayerStateMachine(this);
            
            // Get components
            InputHandler = GetComponent<InputHandler>();
            Movement = GetComponent<PlayerMovement>();
            Stats = GetComponent<PlayerStats>();
            Combat = GetComponent<PlayerCombat>();
            AnimationController = GetComponentInChildren<PlayerAnimationController>();
            Events = GetComponent<PlayerEvents>();
            Audio = GetComponentInChildren<PlayerAudio>();
        }

        private void Start()
        {
            StateMachine.Initialize(PlayerStates.Idle);
            
            // Initialize components
            InputHandler.Initialize();
            Movement.Initialize();
            Stats.Initialize();
            Combat.Initialize();
            AnimationController.Initialize();
            Events.Initialize();
            Audio.Initialize();

            RotateBaseOnInput(false, false);
        }

        
        private void Update()
        {
            InputHandler.HandleInput();


            var isMovingBackward = InputHandler.MoveInput < 0;
            AnimationController.SetMovement(InputHandler.MoveInput);
            RotateBaseOnInput(isMovingBackward, InputHandler.MoveInput == 0);
            
            StateMachine.Update();
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        private void LateUpdate()
        {
            StateMachine.LateUpdate();
        }
        
        private void RotateBaseOnInput(bool isMovingBackward ,bool isIdle)
        {
            if (isIdle)
            {
                transform.rotation = Quaternion.Euler(0, 90, 0);
                return;
            }
            if (isMovingBackward && IsFacingRight)
            {
                RotateToDirection(false); // Face left (-90°)
            }
            else if (!isMovingBackward && !IsFacingRight)
            {
                RotateToDirection(true); // Face right (90°)
            }
        }
    
        private void RotateToDirection(bool faceRight)
        {
            IsFacingRight = faceRight;
            transform.rotation = Quaternion.Euler(0, faceRight ? 0 : 180, 0);
            // AnimationController.ChangeFacingDirection(IsFacingRight);
            AnimationController.ChangeFacingDirectionTween(IsFacingRight);
        }

        public bool IsFacingRight { get; private set; }
    }
}
