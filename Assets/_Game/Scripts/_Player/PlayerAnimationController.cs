/*
*  Author: rutvij
*  Created On: 9/24/2024 2:00:37 AM
* 
*  Copyright (c) 2024 CosmicPull
*/

using DG.Tweening;
using UnityEngine;

namespace Soulslike
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        internal void Initialize()
        {
            
        }

        public void ChangeFacingDirection(bool facingRight)
        {
            _animator.SetFloat("FacingDirection", (facingRight ? 1 : 0));
        }
        
        private float _facingDirectionValue;
        private Tween _facingDirectionTween; // Store the tween reference

        public void SetMovement(float horizontal)
        {
            _animator.SetFloat("H_Movement", horizontal);
        }
        public void ChangeFacingDirectionTween(bool facingRight)
        {
            // Kill the previous tween if it's active
            if (_facingDirectionTween != null && _facingDirectionTween.IsActive())
            {
                _facingDirectionTween.Kill();
            }

            // Determine the target value (1 for right, 0 for left)
            float targetValue = facingRight ? 1f : 0f;

            // Use DOTween to smoothly lerp the FacingDirection value over 0.25 seconds
            _facingDirectionTween = DOTween.To(() => _facingDirectionValue, x => _facingDirectionValue = x, targetValue, 0.25f)
                .OnUpdate(() =>
                {
                    // Update the animator parameter on each frame of the lerp
                    _animator.SetFloat("FacingDirection", _facingDirectionValue);
                });
        }
    }
}
