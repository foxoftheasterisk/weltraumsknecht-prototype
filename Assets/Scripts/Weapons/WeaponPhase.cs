using UnityEngine;

namespace Weltraumsknecht.Weapons
{
    /// <summary>
    /// A phase in a weapon's activation.
    /// Most weapons have at least a warmup phase and a projectile phase.
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponPhase", menuName = "Scriptable Objects/WeaponPhase")]
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
        /// If false, initialProperties will be ignored.
        /// </summary>
        public bool useProperties = false;
        //TODO: figure out if there's a better way to do this (like having a nullable) that works in editor.
        public ProjectileProperties initialProperties;

        /// <summary>
        /// Defines a created projectile's behavior.
        /// A Melee projectile is centered on the player, and follows them as they move;
        /// a Ranged projectile is created at the player's location, but then moves independently;
        /// a Remote projectile is created at the previous phase's center, and moves independently.
        /// Replace will create the projectile at the previous phase's location and apply the same rotation and velocities 
        /// (adding them to those defined by the new phase).
        /// (Note that Replace does not automatically remove the last phase, that's controlled by DestroyLastPhase).
        /// </summary>
        public enum ProjectileLocale
        {
            Melee,
            Ranged,
            Remote,
            Replace
        }
        public ProjectileLocale locale;

        public GameObject projectilePrefab;

        /// <summary>
        /// If true, this phase will become inactive when its projectile does not exist or is not active.
        /// Use false for phases without projectile-creating effects, such as warmups.
        /// </summary>
        public bool linkedToProjectile = true;

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
