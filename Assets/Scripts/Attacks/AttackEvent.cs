using Platformer.Mechanics;
using UnityEngine;

using Weltraumsknecht.Weapons;
using Weltraumsknecht.Player;

namespace Weltraumsknecht.Attacks
{

    /// <summary>
    /// Carries information about events that inform whether TransitionConditions should fire.
    /// Some of these only apply in some cases.
    /// </summary>
    public struct AttackEvent
    {
        internal Attacker attacker;
        internal AttackTransition.TriggerType eventTriggerType;
        internal PhaseInfo phaseInfo;
        internal WeaponInfo? weaponInfo;
        internal CollisionInfo? collisionInfo;
        internal AnimationInfo? animationInfo;
    }

    public struct PhaseInfo
    {
        internal ActivePhase TriggeringPhase;
    }

    public struct WeaponInfo
    {
        public WeaponInstance WeaponInstance;
        public WeaponDefinition WeaponDefinition;
    }

    public struct CollisionInfo
    {
        internal GameObject collidingObject;
    }

    public struct AnimationInfo
    {
        internal string message;
    }
}
