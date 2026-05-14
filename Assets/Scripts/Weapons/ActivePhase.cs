using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.UI.Selectable;

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

        private WeaponInstance parent;

        public ActivePhase(WeaponPhase definition, GameObject projectile, WeaponInstance _parent)
        {
            Definition = definition;
            linkedProjectile = projectile;
            parent = _parent;

            potentialTransitions = new List<WeaponTransition>(Definition.transitions);

            if(potentialTransitions.Any(p => p.triggerType == WeaponTransition.TriggerType.Contact))
            {
                ContactListener listener = projectile.AddComponent<ContactListener>();
                listener.function = p => CheckTransitions(WeaponTransition.TriggerType.Contact);
            }
        }

        public void AdvanceTime(float deltaTime)
        {
            TimeInPhase += deltaTime;
        }

        public void CheckTransitions(WeaponTransition.TriggerType triggerType)
        {
            Debug.Log("Checking transitions " + triggerType);

            List<WeaponTransition> activating = new List<WeaponTransition>();
            foreach (WeaponTransition transition in potentialTransitions)
            {
                if (transition.triggerType == triggerType && transition.ShouldAdvance(this, parent.Player))
                    activating.Add(transition);
            }

            foreach(WeaponTransition transition in activating)
            {
                parent.AdvancePhase(this, transition);
                if (transition.destroyLastPhase)
                    return;
                else
                    potentialTransitions.Remove(transition);
                
            }
        }

    }

}
