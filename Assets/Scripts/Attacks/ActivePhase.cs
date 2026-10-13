using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weltraumsknecht.Projectiles;
using static UnityEngine.UI.Selectable;

using Weltraumsknecht.Weapons;

namespace Weltraumsknecht.Attacks
{
    /// <summary>
    /// Holds the data of a currently-running phase.
    /// Managed by WeaponInstance.
    /// </summary>
    //TODO: it should probably manage itself, for best code practices
    internal class ActivePhase
    {
        public AttackPhase Definition
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

        public bool Flipped
        {
            get;
            private set;
        }

        public List<AttackTransition> potentialTransitions;

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

        private readonly Attack manager;

        public ActivePhase(AttackPhase definition, GameObject projectile, Attack _manager, bool flipped)
        {
            Definition = definition;
            linkedProjectile = projectile;
            manager = _manager;
            Flipped = flipped;

            potentialTransitions = new List<AttackTransition>(Definition.transitions);

            if(potentialTransitions.Any(p => p.triggerType == AttackTransition.TriggerType.Contact))
            {
                if (projectile.TryGetComponent<Projectile>(out Projectile proj))
                {
                    proj.AddListener(p => CheckTransitions(
                        AttackTransition.TriggerType.Contact,
                        manager.GenerateEvent(this,
                            AttackTransition.TriggerType.Contact,
                            new CollisionInfo()
                            {
                                collidingObject = p
                            }))); //This is very messy, should probably be exported.
                }
                else
                {
                    Debug.Log("Could not find Projectile on object to add Contact phase trigger!");
                }
                //TODO: consider adding this listener to *each* object. Probably a good thing to do in the cases it matters.
            }
        }

        public void AdvanceTime(float deltaTime)
        {
            TimeInPhase += deltaTime;
        }

        public void CheckTransitions(AttackTransition.TriggerType triggerType, AttackEvent e)
        {
            Debug.Log("Checking transitions " + triggerType);

            List<AttackTransition> activating = new List<AttackTransition>();
            foreach (AttackTransition transition in potentialTransitions)
            {
                if (transition.triggerType == triggerType && transition.ShouldAdvance(e))
                    activating.Add(transition);
            }

            foreach(AttackTransition transition in activating)
            {
                manager.AdvancePhase(this, transition);
                if (transition.destroyLastPhase)
                    return;
                else
                    potentialTransitions.Remove(transition);
                
            }
        }

    }

}
