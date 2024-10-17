/*
*  Author: rutvij
*  Created On: 9/24/2024 12:44:15 AM
* 
*  Copyright (c) 2024 CosmicPull
*/


namespace Soulslike.Player
{
    public class PlayerStateMachine
    {
        public PlayerState CurrentState { get; private set; }
        public PlayerController Player { get; private set; }
        public PlayerStateFactory Factory { get; private set; }

        public PlayerStateMachine(PlayerController player)
        {
            Player = player;
            Factory = new PlayerStateFactory(this);
        }

        public void Initialize(PlayerStates startingState)
        {
            CurrentState = Factory.GetState(startingState);
            CurrentState.Enter();
        }

        public void ChangeState(PlayerStates newState)
        {
            CurrentState.Exit();
            CurrentState = Factory.GetState(newState);
            CurrentState.Enter();
        }

        public void Update()
        {
            CurrentState.Update();
        }

        public void FixedUpdate()
        {
            CurrentState.FixedUpdate();
        }

        public void LateUpdate()
        {
            CurrentState.LateUpdate();
        }
    }
}
