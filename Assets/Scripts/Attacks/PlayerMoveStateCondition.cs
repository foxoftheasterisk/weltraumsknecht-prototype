using Platformer.Mechanics;
using System;
using UnityEngine;
using Weltraumsknecht.Attacks;

using Weltraumsknecht.Player;

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
            if (e.attacker.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                return targetState switch
                {
                    PlayerMoveState.Air => !player.IsGrounded,
                    PlayerMoveState.Ground => player.IsGrounded,
                    PlayerMoveState.Dashing => player.IsDashing(),
                    PlayerMoveState.NotDashing => !player.IsDashing(),
                    _ => throw new NotImplementedException("Undefined PlayerMoveState for PlayerMoveStateCondition!"),
                };
            }
            else
                throw new ArgumentException("PlayerMoveStateCondition cannot be used when attacker is not the player!");
        }
    }
}
