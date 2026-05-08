using System.Collections.Generic;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{
    /// <summary>
    /// Holds the data of a currently-running phase.
    /// Managed by WeaponInstance.
    /// </summary>
    //TODO: it should probably manage itself, for best code practices
    internal class ActivePhase
    {
        public WeaponPhase Definition
        {
            get;
            private set;
        }

        public GameObject linkedProjectile;

        public float TimeInPhase
        {
            get;
            private set;
        } = 0;

        public List<WeaponTransition> potentialTransitions;

        public bool IsActive()
        {
            if (Definition.linkedToProjectile)
            {
                if (linkedProjectile == null)
                    return false;
                return linkedProjectile.activeInHierarchy;
            }
            return true;
        }

        public ActivePhase(WeaponPhase definition, GameObject projectile)
        {
            Definition = definition;
            linkedProjectile = projectile;
            potentialTransitions = new List<WeaponTransition>(Definition.transitions);
        }

        public void AdvanceTime(float deltaTime)
        {
            TimeInPhase += deltaTime;
        }

    }

}
