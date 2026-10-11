using Platformer.Mechanics;
using System;
using UnityEngine;
using Weltraumsknecht.Attacks;

namespace Weltraumsknecht.Attacks
{

    /// <summary>
    /// A transition that checks the player's current state against certain movement states to determine if it should activate.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMoveStateCondition", menuName = "Weapon Conditions/PlayerMoveStateCondition")]
    public class PlayerMoveStateCondition : TransitionCondition
    {
        public enum PlayerMoveState
        {
            Air,
            Ground,
            Dashing,
            NotDashing
        }
        public PlayerMoveState targetState;

        internal override bool CheckCondition(AttackEvent e)
        {
            if (e.weaponInfo is null)
                throw new ArgumentNullException("PlayerMoveStateCondition cannot be used when weaponInfo is null!");

            PlayerController player = e.weaponInfo?.Player;
            switch (targetState)
            {
                case PlayerMoveState.Air:
                    return !player.IsGrounded;
                case PlayerMoveState.Ground:
                    return player.IsGrounded;
                case PlayerMoveState.Dashing:
                    return player.IsDashing();
                case PlayerMoveState.NotDashing:
                    return !player.IsDashing();
                default:
                    throw new NotImplementedException("Undefined PlayerMoveState for PlayerMoveStateCondition!");
            }
        }
    }
}
