using UnityEngine;
using UnityEngine.Serialization;
using Weltraumsknecht.Weapons;

namespace Weltraumsknecht.Attacks
{
    /// <summary>
    /// A phase in an attack's activation.
    /// Most attacks have at least a warmup phase and a projectile phase.
    /// </summary>
    [CreateAssetMenu(fileName = "AttackPhaseDefinition", menuName = "Scriptable Objects/AttackPhase")]
    public class AttackPhaseDefinition : ScriptableObject
    {
        /// <summary>
        /// A weapon's cooldown is not triggered until it enters a non-warmup phase.
        /// (Thus, if the weapon's activation is interrupted during its warmup, the cooldown is not triggered.)
        /// </summary>
        public bool isWarmup;

        /// <summary>
        /// A phase that is performed means that it is linked to actions by the Attacker; thus, it may be canceled if the Attacker can no longer perform the attack.
        /// A phase that is not performed is generally a ranged projectile. It will continue even if the Attacker cancels the attack or dies.
        /// </summary>
        public bool isPerformed;
        //TODO: determine this automatically?
        //Warmups, melee projectiles, animations, and movements are all performed.
        //Also, all phases that block movement, block facing, or have a move speed penalty are performed.

        public bool blocksMovement;
        public float moveSpeedPenalty = 0; //1 for no volitional movement, 0 for no penalty
        public bool blocksFacing;

        /// <summary>
        /// The set of possible transitions from this phase.
        /// Each transition can only be activated once (per instance of the phase).
        /// If multiple transitions are triggered simultaneously, they will be processed in the order they appear in this array.
        /// Once the phase has been destroyed, no further transitions will be processed.
        /// </summary>
        public AttackTransition[] transitions;

        // --- PHASE EFFECTS ---
        
        //(Wondering if I should make these be, like, a List of effects instead of just all the possible effects.
        //Thing is, I don't think we'd want to allow the same effect more than once in a phase?)

        #region CreateProjectile

        [FormerlySerializedAs("linkedToProjectile")]
        public bool createProjectile = false;

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

        #endregion

        #region Animation

        public bool animation = false;

        public string animationTriggerName;

        #endregion

        #region MoveAttacker

        //TODO: this

        #endregion

        //TODO: Transition trigger?
    }
}
