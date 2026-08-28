using Platformer.Mechanics;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{
    internal class WeaponCollisionEvent : WeaponEvent
    {
        internal Collider2D CollidingObject
        { get; private set; }

        internal WeaponCollisionEvent(ActivePhase currentPhase, PlayerController player, Collider2D collidingObject) : base(currentPhase, player)
        {
            CollidingObject = collidingObject;
        }
    }
}
