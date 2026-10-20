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
    /// Managed by ActiveAttack.
    /// </summary>
    //TODO: it should probably manage itself, for best code practices
    internal class ActivePhase
    {
        public AttackPhaseDefinition Definition
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
            if (Definition.createProjectile)
            {
                if (linkedProjectile == null)
                    return false;
                return linkedProjectile.activeInHierarchy;
            }
            return true;
        }

        private readonly ActiveAttack manager;

        public ActivePhase(AttackPhaseDefinition definition, GameObject projectile, ActiveAttack _manager, bool flipped)
        {
            Definition = definition;
            linkedProjectile = projectile;
            manager = _manager;
            Flipped = flipped;

            potentialTransitions = new List<AttackTransition>(Definition.transitions);

            if(potentialTransitions.Any(p => p.triggerType == AttackTransition.TriggerType.ProjectileContact))
            {
                if (projectile.TryGetComponent<Projectile>(out Projectile proj))
                {
                    proj.AddListener(p => InvokeTrigger(
                        AttackTransition.TriggerType.ProjectileContact,
                        manager.GenerateEvent(this,
                            AttackTransition.TriggerType.ProjectileContact,
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

        public void InvokeTrigger(AttackTransition.TriggerType triggerType, AttackEvent e)
        {
            List<AttackTransition> activating = manager.CheckTransitions(this, triggerType, e);

            manager.ProcessTransitions(this, activating);
        }

    }

}
