using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// Defines when and how a weapon changes phases.
    /// Each transition can only activate once per active phase.
    /// </summary>
    [System.Serializable]
    public abstract class WeaponTransition
    {
        public bool destroyLastPhase;

        /// <summary>
        /// The number of phases to advance. Usually one. Can be negative.
        /// If outside the range of existing phases, the transition will do nothing.
        /// </summary>
        public int advancePhases = 1;

        abstract internal bool ShouldAdvance(ActivePhase phaseState);
    }
}