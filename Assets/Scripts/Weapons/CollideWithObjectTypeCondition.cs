using Platformer.Mechanics;
using System;
using UnityEngine;

using static ProjectileUtils;

using Weltraumsknecht.Enemies;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// A simple transition that advances from any triggers that fall within the correct time range.
    /// </summary>
    [CreateAssetMenu(fileName = "CollideWithObjectTypeCondition", menuName = "Weapon Conditions/CollideWithObjectTypeCondition")]
    public class CollideWithObjectTypeCondition : TransitionCondition
    {

        public TargetType type;

        internal override bool CheckCondition(WeaponEvent e)
        {
            if (e is not WeaponCollisionEvent)
            {
                throw new System.Exception("CollideWithObjectTypeCondition used in a non-collision context!");
            }
            WeaponCollisionEvent collisionEvent = (WeaponCollisionEvent) e;

            return type.Matches(collisionEvent.CollidingObject);
        }
    }
}
