using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Weltraumsknecht.Projectiles;
using Weltraumsknecht.Weapons;
using static Weltraumsknecht.Weapons.WeaponDefinition;


namespace Weltraumsknecht.Attacks
{
    /// <summary>
    /// A class that manages a single instance of an attack.
    /// This can be a weapon attack or an enemy attack.
    /// </summary>
    // Should this be a MonoBehaviour attached to an Actor?
    // (Or not attached, so that independent phases can continue if the Actor is destroyed?)
    public class AttackManager
    {
        private List<ActivePhase> activePhases;

        public AttackManager(AttackPhase initialPhase)
        {
            activePhases = new()
            {
                initialPhase.Activate()
            };
        }

        public void Update()
        {
            List<ActivePhase> inactive = new();
            List<Tuple<ActivePhase, List<WeaponTransition>>> transitioning = new();
            foreach (ActivePhase phase in activePhases)
            {
                if (!phase.IsActive())
                {
                    inactive.Add(phase); 
                    //Checks for phases that have become inactive without the use of a trigger.
                    //This should be avoided when possible but there are still some cases for it (e.g. weapons that destroy enemy projectiles).
                }
                else
                {
                    phase.AdvanceTime(Time.deltaTime);
                    List<WeaponTransition> activating = CheckTransitions(phase, WeaponTransition.TriggerType.Update, new WeaponEvent(phase, Player));
                    if (activating.Count > 0)
                    {
                        transitioning.Add(new(phase, activating));
                    }
                }
            }

            foreach (ActivePhase phase in inactive)
            {
                activePhases.Remove(phase);
            }

            foreach (Tuple<ActivePhase, List<WeaponTransition>> activePair in transitioning)
            {
                ProcessTransitions(activePair.Item1, activePair.Item2);
            }
            
            //Destroy this if no phases are active
        }

        public void ButtonPressed()
        {
            ProcessAllTransitions(WeaponTransition.TriggerType.ButtonPress);
        }

        public void ButtonReleased()
        {
            ProcessAllTransitions(WeaponTransition.TriggerType.ButtonRelease);
        }

        //TODO: move transition checking to ActivePhase?
        private void ProcessAllTransitions(WeaponTransition.TriggerType triggerType)
        {
            List<Tuple<ActivePhase, List<WeaponTransition>>> activatingTransitions = new();

            foreach (ActivePhase phase in activePhases)
            {
                List<WeaponTransition> transitions = CheckTransitions(phase, triggerType, new WeaponEvent(phase, Player));
                if (transitions.Count > 0)
                {
                    activatingTransitions.Add(new(phase, transitions));
                }
            }

            foreach (Tuple<ActivePhase, List<WeaponTransition>> phaseTransitions in activatingTransitions)
            {
                ProcessTransitions(phaseTransitions.Item1, phaseTransitions.Item2);
            }
        }

        private void ProcessTransitions(ActivePhase phase, List<WeaponTransition> transitions)
        {
            foreach (WeaponTransition transition in transitions)
            {
                AdvancePhase(phase, transition);
                if (transition.destroyLastPhase)
                    return;
                else
                    phase.potentialTransitions.Remove(transition);
            }
        }

        private List<WeaponTransition> CheckTransitions(ActivePhase phase, WeaponTransition.TriggerType triggerType, WeaponEvent e)
        {
            List<WeaponTransition> activating = new List<WeaponTransition>();
            foreach (WeaponTransition transition in phase.potentialTransitions)
            {
                if (transition.triggerType == triggerType && transition.ShouldAdvance(e))
                    activating.Add(transition);
            }

            return activating;
        }

        public bool IsActive()
        {
            foreach (ActivePhase phase in activePhases)
            {
                if (phase.IsActive() && phase.Definition.activeLink)
                    return true;
            }
            return false;
        }

        internal void AdvancePhase(ActivePhase lastPhase, WeaponTransition transition)
        {
            if (transition.nextPhase != null)
            {
                StartPhase(transition.nextPhase, lastPhase.linkedProjectile, lastPhase.Flipped);
            }

            if (transition.destroyLastPhase)
            {
                activePhases.Remove(lastPhase);
                if (lastPhase.linkedProjectile != null)
                    GameObject.Destroy(lastPhase.linkedProjectile);
            }
        }

        private void StartPhase(WeaponPhase phase, GameObject lastPhaseObject = null, bool lastPhaseFlipped = false)
        {
            if (!phase.isWarmup)
            {
                //TODO: send start cooldown message to actor, track that message has been sent.
            }

            if (phase.projectilePrefab == null)
            {
                activePhases.Add(new ActivePhase(phase, null, this, lastPhaseFlipped));
                return;
            }

            GameObject projectile;
            ProjectileProperties? phaseProps = null;
            if (phase.useProperties)
                phaseProps = phase.initialProperties;

            bool flip;

            switch (phase.locale)
            {
                case WeaponPhase.ProjectileLocale.Melee:
                case WeaponPhase.ProjectileLocale.Ranged:
                    flip = Player.IsFacingLeft();
                    projectile = ProjectileUtils.CreateProjectile(phase.projectilePrefab, Player.GameObject(), flip, phase.locale == WeaponPhase.ProjectileLocale.Melee, phaseProps);
                    break;
                case WeaponPhase.ProjectileLocale.Remote:
                    if (lastPhaseObject == null)
                        throw new MissingReferenceException("Tried to start remote phase with no parent!");
                    flip = lastPhaseFlipped;
                    projectile = ProjectileUtils.CreateProjectile(phase.projectilePrefab, lastPhaseObject, lastPhaseFlipped, false, phaseProps);
                    break;
                case WeaponPhase.ProjectileLocale.Replace:
                    if (lastPhaseObject == null)
                        throw new MissingReferenceException("Tried to start replace phase with no parent!");
                    flip = lastPhaseFlipped;
                    projectile = ProjectileUtils.CreateProjectile(phase.projectilePrefab, lastPhaseObject, lastPhaseFlipped, false, phaseProps, true);
                    break;
                default:
                    throw new NotImplementedException("Undefined projectile locale: " + phase.locale);
            }

            if (projectile != null)
            {
                WeaponProjectile[] wps = projectile.GetComponentsInChildren<WeaponProjectile>();
                foreach (WeaponProjectile wp in wps)
                {
                    wp.Create(this, phase.locale == WeaponPhase.ProjectileLocale.Melee);
                }
            }

            activePhases.Add(new ActivePhase(phase, projectile, this, flip));
        }

        public int GetDamage(bool crit)
        {
            //TODO: pass along to actor? or whatever. get the damage, somehow.
            if (crit)
                return definition.critDamage;
            else
                return definition.standardDamage;
        }

        public virtual bool IsBlockingFacing()
        {
            if (!IsActive())
                return false;
            foreach (ActivePhase phase in activePhases)
            {
                if (phase.Definition.blocksFacing && phase.IsActive())
                    return true;
            }
            return false;
        }

        public virtual bool IsBlockingMovement()
        {
            if (!IsActive())
                return false;
            foreach (ActivePhase phase in activePhases)
            {
                if (phase.Definition.blocksMovement && phase.IsActive())
                    return true;
            }
            return false;
        }

        public virtual bool IsBlockingAttacks()
        {
            if (!IsActive())
                return false;
            foreach (ActivePhase phase in activePhases)
            {
                if (phase.Definition.blocksWeapons && phase.IsActive())
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Ends the attack, canceling any phases that are linked to the actor.
        /// Phases marked as independent may continue.
        /// </summary>
        public void CancelAttack()
        {
            //TODO: this
        }

    }

}
