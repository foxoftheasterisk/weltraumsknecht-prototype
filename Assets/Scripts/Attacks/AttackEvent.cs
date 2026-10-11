using Platformer.Mechanics;
using UnityEngine;

using Weltraumsknecht.Weapons;

namespace Weltraumsknecht.Attacks
{

    /// <summary>
    /// Carries information about events that inform whether TransitionConditions should fire.
    /// Some of these only apply in some cases.
    /// </summary>
    internal struct AttackEvent
    {
        internal AttackTransition.TriggerType eventTriggerType;
        internal PhaseInfo phaseInfo;
        internal WeaponInfo? weaponInfo;
        internal CollisionInfo? collisionInfo;
    }

    internal struct PhaseInfo
    {
        internal ActivePhase TriggeringPhase;
    }

    internal struct WeaponInfo
    {
        internal PlayerMovementController Player;
        internal WeaponInstance WeaponInstance;
        internal WeaponDefinition WeaponDefinition;
    }

    internal struct CollisionInfo
    {
        internal GameObject collidingObject;
    }

}
