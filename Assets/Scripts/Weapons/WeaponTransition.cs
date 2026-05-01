using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// Defines when and how a weapon changes phases.
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



    }
}