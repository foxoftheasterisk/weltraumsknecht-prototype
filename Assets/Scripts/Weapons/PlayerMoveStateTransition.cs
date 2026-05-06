using Platformer.Mechanics;
using System;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// A transition that checks the player's current state against certain movement states to determine if it should activate.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMoveStateTransition", menuName = "Weapon Transitions/PlayerMoveStateTransition")]
    public class PlayerMoveStateTransition : WeaponTransition
    {
        public enum PlayerMoveState
        {
            Air,
            Ground,
            Dashing
        }
        PlayerMoveState targetState;

        internal override bool ShouldAdvance(ActivePhase phaseState, PlayerController player)
        {
            switch (targetState)
            {
                case PlayerMoveState.Air:
                    return !player.IsGrounded;
                case PlayerMoveState.Ground:
                    return player.IsGrounded;
                case PlayerMoveState.Dashing:
                    return player.IsDashing();
                default:
                    throw new NotImplementedException("Undefined PlayerMoveState for PlayerMoveStateTransition!");
            }
        }
    }
}
