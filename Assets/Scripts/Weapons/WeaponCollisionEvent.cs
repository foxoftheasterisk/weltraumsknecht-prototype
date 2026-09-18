using Platformer.Mechanics;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{
    internal class WeaponCollisionEvent : WeaponEvent
    {
        internal GameObject CollidingObject
        { get; private set; }

        internal WeaponCollisionEvent(ActivePhase currentPhase, PlayerController player, GameObject collidingObject) : base(currentPhase, player)
        {
            CollidingObject = collidingObject;
        }
    }
}
