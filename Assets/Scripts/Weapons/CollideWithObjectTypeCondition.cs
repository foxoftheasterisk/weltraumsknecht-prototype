using Platformer.Mechanics;
using System;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// A simple transition that advances from any triggers that fall within the correct time range.
    /// </summary>
    [CreateAssetMenu(fileName = "CollideWithObjectTypeCondition", menuName = "Weapon Conditions/CollideWithObjectTypeCondition")]
    public class CollideWithObjectTypeCondition : TransitionCondition
    {

        public enum ObjectType
        {
            Enemy,
            Background
        }

        public ObjectType type;

        internal override bool CheckCondition(WeaponEvent e)
        {
            if (!(e is WeaponCollisionEvent))
            {
                
                throw new System.Exception("CollideWithObjectTypeCondition used in a non-collision context!");
            }
            WeaponCollisionEvent collisionEvent = (WeaponCollisionEvent) e;

            switch (type)
            {
                case ObjectType.Enemy:
                    return collisionEvent.CollidingObject.TryGetComponent(out EnemyController _);
                case ObjectType.Background:
                    return collisionEvent.CollidingObject.gameObject.layer == LayerMask.NameToLayer("Environment");
                default:
                    throw new NotImplementedException("Object type " + type.ToString() + " is not implemented in CollideWithObjectTypeCondition.");
                    //Should only occur if new values are added to the enum without handling.
            }
        }
    }
}
