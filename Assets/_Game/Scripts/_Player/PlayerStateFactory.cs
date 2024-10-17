/*
 *  Author: rutvij
 *  Created On: 9/24/2024 12:52:17 AM
 *
 *  Copyright (c) 2024 CosmicPull
 */


using Soulslike.Player.States;

namespace Soulslike.Player
{
	public enum PlayerStates
	{
		// Movement States
		Idle,
		Move,
		Jump,
		DoubleJump,
		Fall,
		Landing,

		// Attack States
		LightAttack,
		HeavyAttack,
		AirAttack,

		// Special Action States
		Dash,
		Dodge,
		SwitchWeapon,

		// Damage States
		Hit,
		Stun,

		// Death State
		Dead
	}

	public class PlayerStateFactory
	{
		private PlayerStateMachine _context;

		public PlayerStateFactory(PlayerStateMachine currentContext)
		{
			_context = currentContext;
		}

		public PlayerState GetState(PlayerStates state)
		{
			switch (state)
			{
				case PlayerStates.Idle:
					return Idle();
				case PlayerStates.Move:
					return Move();
				case PlayerStates.Jump:
					return Jump();
				case PlayerStates.DoubleJump:
					return DoubleJump();
				case PlayerStates.Fall:
					return Fall();
				case PlayerStates.Landing:
					return Landing();
				case PlayerStates.LightAttack:
					return LightAttack();
				case PlayerStates.HeavyAttack:
					return HeavyAttack();
				case PlayerStates.AirAttack:
					return AirAttack();
				case PlayerStates.Dash:
					return Dash();
				case PlayerStates.Dodge:
					return Dodge();
				case PlayerStates.Hit:
					return GetHit();
				case PlayerStates.Stun:
					return Stun();
				case PlayerStates.Dead:
					return Dead();
				default:
					return null;
			}
		}

		// State creation methods

		public PlayerState Idle() => new IdleState(_context, this);
		
		public PlayerState Move() => new MoveState(_context, this);
		
		public PlayerState Jump() => new JumpState(_context, this);
		
		public PlayerState DoubleJump() => new DoubleJumpState(_context, this);
		
		public PlayerState Fall() => new FallState(_context, this);
		
		public PlayerState Landing() => new LandingState(_context, this);
		
		public PlayerState LightAttack() => new LightAttackState(_context, this);
		
		public PlayerState HeavyAttack() => new HeavyAttackState(_context, this);
		
		public PlayerState AirAttack() => new AirAttackState(_context, this);
		
		public PlayerState Dash() => new DashState(_context, this);
		
		public PlayerState Dodge() => new DodgeState(_context, this);
		
		public PlayerState GetHit() => new HitState(_context, this);
		
		public PlayerState Stun() => new StunState(_context, this);
		
		public PlayerState Dead() => new DeadState(_context, this);
	}
}