using System;
using UnityEngine;

namespace Weltraumsknecht.Attacks
{
    /// <summary>
    /// Shared base for entities that can host attacks (e.g. enemies and player)
    /// </summary>
    public abstract class Attacker : MonoBehaviour
    {
        protected Attack currentAttack;

        public abstract bool IsFacingLeft();

        /// <summary>
        /// Called by an attack when its first non-warmup phase is completed. Generally used to start cooldowns.
        /// </summary>
        public abstract void EndWarmup();

        public virtual bool IsFacingBlocked()
        {
            if (currentAttack is null)
                return false;

            return currentAttack.IsBlockingFacing();
        }

        public virtual bool IsMovementBlocked()
        {
            if (currentAttack is null)
                return false;

            return currentAttack.IsBlockingMovement();
        }

        public virtual bool IsAttackingBlocked()
        {
            if (currentAttack is null)
                return false;

            return currentAttack.IsBlockingAttacks();
        }
    }
}