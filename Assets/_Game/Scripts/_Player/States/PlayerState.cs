/*
*  Author: rutvij
*  Created On: 9/24/2024 12:47:39 AM
* 
*  Copyright (c) 2024 CosmicPull
*/


namespace Soulslike.Player
{
    public abstract class PlayerState
    {
        protected PlayerStateMachine _context;
        protected PlayerStateFactory _factory;

        protected PlayerState(PlayerStateMachine context, PlayerStateFactory factory)
        {
            _context = context;
            _factory = factory;
        }


        public abstract void Enter();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void LateUpdate();
        public abstract void Exit();
    }
}
