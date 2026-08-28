using Platformer.Mechanics;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// A simple transition that advances from any triggers that fall within the correct time range.
    /// </summary>
    [CreateAssetMenu(fileName = "TimeRangeCondition", menuName = "Weapon Conditions/TimeRangeCondition")]
    public class TimeRangeCondition : TransitionCondition
    {
        //The minimum amount of time elapsed.
        public float minimumTime = 0;

        //The maximum amount of time elapsed.
        public float maximumTime = float.PositiveInfinity;

        internal override bool CheckCondition(WeaponEvent e)
        {
            ActivePhase phaseState = e.CurrentPhase;
            return (phaseState.TimeInPhase >= minimumTime && phaseState.TimeInPhase <= maximumTime);
        }
    }
}
