using Platformer.Mechanics;
using System;
using UnityEngine;

using static ProjectileUtils;

using Weltraumsknecht.Enemies;
using Weltraumsknecht.Weapons;

namespace Weltraumsknecht.Attacks
{

    /// <summary>
    /// A simple transition that advances from any triggers that fall within the correct time range.
    /// </summary>
    [CreateAssetMenu(fileName = "CollideWithObjectTypeCondition", menuName = "Weapon Conditions/CollideWithObjectTypeCondition")]
    public class CollideWithObjectTypeCondition : TransitionCondition
    {

        public TargetType type;

        internal override bool CheckCondition(AttackEvent e)
        {
            if (e.collisionInfo is null)
            {
                throw new ArgumentNullException("CollideWithObjectTypeCondition used where collisionInfo is null!");
            }
            return type.Matches(e.collisionInfo?.collidingObject);
        }
    }
}
