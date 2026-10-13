using System;
using UnityEngine;

namespace Weltraumsknecht.Attacks
{
    /// <summary>
    /// Shared base for entities that can host attacks (e.g. enemies and player)
    /// </summary>
    public abstract class Attacker : MonoBehaviour
    {
        public abstract bool IsFacingLeft();

        /// <summary>
        /// Called by an attack when its first non-warmup phase is completed. Generally used to start cooldowns.
        /// </summary>
        public abstract void EndWarmup();
    }
}