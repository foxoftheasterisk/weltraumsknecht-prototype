using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// Defines when and how a weapon changes phases.
    /// Each transition can only activate once per active phase.
    /// A single copy of each Transition exists per game, so Transitions should not contain any temporary data;
    /// any required data should be obtained from the ActivePhase.
    /// </summary>
    public abstract class WeaponTransition : ScriptableObject
    {
        public bool destroyLastPhase = true;

        /// <summary>
        /// The phase to advance to.
        /// Note that phases can repeat or even have multiple active instances at once.
        /// If null, the transition will process but no new phase will start. (This is useful for ending phases on triggers.)
        /// </summary>
        public WeaponPhase nextPhase;

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