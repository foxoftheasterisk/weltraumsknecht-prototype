using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// A simple transition that advances from any triggers that fall within the correct time range.
    /// </summary>
    public class TimeRangeTransition : WeaponTransition
    {
        //The minimum amount of time elapsed.
        public float minimumTime = 0;

        //The maximum amount of time elapsed.
        public float maximumTime = float.PositiveInfinity;

        internal override bool ShouldAdvance(ActivePhase phaseState)
        {
            return (phaseState.TimeInPhase >= minimumTime && phaseState.TimeInPhase <= maximumTime);
        }
    }
}
