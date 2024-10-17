/*
*  Author: rutvij
*  Created On: 9/24/2024 1:01:19 AM
* 
*  Copyright (c) 2024 CosmicPull
*/

using UnityEngine;

namespace Soulslike.Player
{
    public class InputHandler : MonoBehaviour
    {
        public float MoveInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool DashInput { get; private set; }
        public bool DodgeInput { get; private set; }
        public bool LightAttackInput { get; private set; }
        public bool HeavyAttackInput { get; private set; }
        public bool BlockInput { get; private set; }
        public bool TargetLockInput { get; private set; }
        public bool SwitchWeaponInput { get; private set; }

        internal void Initialize()
        {
            
        }

        public void HandleInput()
        {
            MoveInput = Input.GetAxis("Horizontal");
            JumpInput = Input.GetKeyDown(KeyCode.Space);
            DashInput = Input.GetKeyDown(KeyCode.LeftAlt);
            DodgeInput = Input.GetKeyDown(KeyCode.LeftControl);
            LightAttackInput = Input.GetKeyDown(KeyCode.Mouse0);
            HeavyAttackInput = Input.GetKeyDown(KeyCode.Mouse1);
            BlockInput = Input.GetKey(KeyCode.E);
            TargetLockInput = Input.GetKeyDown(KeyCode.Q);

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
            {
                SwitchWeaponInput = true;
            }
        }

        public void ResetInputs()
        {
            JumpInput = false;
            DashInput = false;
            DodgeInput = false;
            LightAttackInput = false;
            HeavyAttackInput = false;
            TargetLockInput = false;
        }
    }
}
