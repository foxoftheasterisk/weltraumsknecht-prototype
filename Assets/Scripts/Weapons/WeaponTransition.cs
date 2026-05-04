using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// Defines when and how a weapon changes phases.
    /// Each transition can only activate once per active phase.
    /// A single copy of each Transition exists per game, so Transitions should not contain any temporary data;
    /// any required data should be obtained from the ActivePhase.
    /// </summary>
    [System.Serializable]
    public abstract class WeaponTransition
    {
        public bool destroyLastPhase = true;

        /// <summary>
        /// The number of phases to advance. Usually one. Can be negative.
        /// If outside the range of existing phases, the transition will do nothing.
        /// </summary>
        public int advancePhases = 1;

        /// <summary>
        /// Controls when the weapon should check if the phase is ready to advance.
        /// </summary>
        public enum TriggerType
        {
            Update,
            ButtonPress,
            ButtonRelease,
            Inactivate
        }
        public TriggerType triggerType;

        abstract internal bool ShouldAdvance(ActivePhase phaseState);
    }
}