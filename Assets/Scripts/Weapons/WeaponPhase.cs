using UnityEngine;

namespace Weltraumsknecht.Weapons
{
    /// <summary>
    /// A phase in a weapon's activation.
    /// Most weapons have at least a warmup phase and a projectile phase.
    /// </summary>
    public class WeaponPhase : ScriptableObject
    {
        /// <summary>
        /// A weapon's cooldown is not triggered until it enters a non-warmup phase.
        /// (Thus, if the weapon's activation is interrupted during its warmup, the cooldown is not triggered.)
        /// </summary>
        public bool isWarmup;
        
        public bool blocksMovement;
        public float moveSpeedPenalty = 0; //1 for no volitional movement, 0 for no penalty
        public bool blocksWeapons;
        public bool blocksFacing;

        /// <summary>
        /// Defines a created projectile's behavior.
        /// A Melee projectile is centered on the player, and follows them as they move;
        /// a Ranged projectile is created at the player's location, but then moves independently;
        /// a Remote projectile is created at the previous phase's center, and moves independently.
        /// </summary>
        public enum ProjectileLocale
        {
            Melee,
            Ranged,
            Remote
        }
        public ProjectileLocale locale;

        public GameObject projectilePrefab;

        /// <summary>
        /// If false, the weapon is considered inactive even if this phase is active.
        /// In most cases, this is not desired.
        /// </summary>
        public bool activeLink = true;

        /// <summary>
        /// The set of possible transitions from this phase.
        /// Each transition can only be activated once (per instance of the phase).
        /// If multiple transitions are triggered simultaneously, they will be processed in the order they appear in this array.
        /// Once the phase has been destroyed, no further transitions will be processed.
        /// </summary>
        public WeaponTransition[] transitions;
    }
}
