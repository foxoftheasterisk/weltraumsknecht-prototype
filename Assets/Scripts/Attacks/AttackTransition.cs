using Platformer.Mechanics;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weltraumsknecht.Attacks
{

    /// <summary>
    /// Defines when and how a weapon changes phases.
    /// Each transition can only activate once per active phase.
    /// A single copy of each Transition exists per game, so Transitions should not contain any temporary data;
    /// any required data should be obtained from the ActivePhase.
    /// </summary>
    [Serializable]
    public class AttackTransition
    {
        /// <summary>
        /// If true, if this transition activates, no transitions below it in the list will be pinged about this trigger.
        /// (Since the transition is removed after activating, this can be used for sequential phases.)
        /// </summary>
        public bool consumeTrigger = false;

        /// <summary>
        /// The phase to advance to.
        /// Note that phases can repeat or even have multiple active instances at once.
        /// If null, the transition will process but no new phase will start. (This is useful for ending phases on triggers.)
        /// </summary>
        public AttackPhaseDefinition nextPhase;

        /// <summary>
        /// If true, this transition ends the phase it is attached to when it activates.
        /// This also causes any ongoing effects of the phase, such as projectiles or Attacker movement, to be destroyed.
        /// Additionally, it prevents any later transitions from activating.
        /// </summary>
        [FormerlySerializedAs("destroyLastPhase")]
        public bool endLastPhase = true;

        /// <summary>
        /// Controls when the weapon should check if the phase is ready to advance.
        /// Update: Triggered each Update phase (i.e. at all times)
        /// ButtonPress, ButtonRelease: Triggered when the action happens (not constantly while in the down or up state). (TODO: add handling for the other as well.)
        /// ProjectileContact: Triggered when the linked projectile enters a trigger/collision with another object.
        /// AnimationMessage: Triggered "manually" by the Attacker's Animations.
        /// AttackerContact: Triggered when the Attacker enters a trigger or collision with another object.
        /// </summary>
        public enum TriggerType
        {
            Update,
            ButtonPress,
            ButtonRelease,
            ProjectileContact,
            AnimationMessage,
            AttackerContact
        }
        public TriggerType triggerType;

        /// <summary>
        /// The conditions to be fulfilled.
        /// All conditions must be satisfied (AND) for the phase to advance.
        /// If there are no conditions, the phase will always advance when the trigger occurs.
        /// </summary>
        public TransitionCondition[] conditions;

        internal bool ShouldAdvance(AttackEvent e)
        {
            foreach (TransitionCondition condition in conditions)
            {
                if (!condition.CheckCondition(e))
                    return false;
            }
            return true;
        }
    }
}