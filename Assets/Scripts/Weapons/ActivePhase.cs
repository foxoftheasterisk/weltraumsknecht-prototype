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
        public WeaponPhase definition;

        public GameObject linkedProjectile;

        public float timeInPhase;

        public List<WeaponTransition> potentialTransitions;

        public bool isActive()
        {
            if (linkedProjectile == null)
                return false;
            return linkedProjectile.activeInHierarchy;
        }
    }

}
