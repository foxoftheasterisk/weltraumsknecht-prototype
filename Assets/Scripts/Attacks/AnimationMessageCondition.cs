using System;
using UnityEngine;

namespace Weltraumsknecht.Attacks
{

    /// <summary>
    /// A condition that checks that the animation message given matches the held string.
    /// Only works in animation message context.
    /// </summary>
    [CreateAssetMenu(fileName = "AnimationMessageCondition", menuName = "Weapon Conditions/AnimationMessageCondition")]
    public class AnimationMessageCondition : TransitionCondition
    {
        public string message;

        internal override bool CheckCondition(AttackEvent e)
        {
            if (e.animationInfo is null)
                throw new ArgumentException("AnimationMessageCondition needs an AnimationInfo to work!");

            return message.Equals(e.animationInfo?.message);
        }
    }
}