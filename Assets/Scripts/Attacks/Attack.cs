using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Weltraumsknecht.Projectiles;
using Weltraumsknecht.Weapons;
using static Weltraumsknecht.Weapons.WeaponDefinition;
using static Weltraumsknecht.Attacks.AttackTransition;
using Platformer.Mechanics;


namespace Weltraumsknecht.Attacks
{
    /// <summary>
    /// A class that manages a single instance of an attack.
    /// This can be a weapon attack or an enemy attack.
    /// </summary>
    // Should this be a MonoBehaviour attached to an Actor?
    // (Or not attached, so that independent phases can continue if the Actor is destroyed?)
    public class Attack
    {
        private List<ActivePhase> activePhases;

        private Attacker attacker;
        private WeaponInfo? weaponInfo;

        private bool warmup = true;
        private bool performing = true;

        public Attack(Attacker attacker, AttackPhaseDefinition initialPhase, WeaponInfo? weaponInfo = null)
        {
            this.attacker = attacker;
            this.weaponInfo = weaponInfo;

            activePhases = new();
            StartPhase(initialPhase);
            AttackManager.Instance.AddAttack(this);
        }

        public void Update()
        {
            List<ActivePhase> inactive = new();
            List<Tuple<ActivePhase, List<AttackTransition>>> transitioning = new();
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
                    List<AttackTransition> activating = CheckTransitions(phase, TriggerType.Update, GenerateEvent(phase, TriggerType.Update));
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

            foreach (Tuple<ActivePhase, List<AttackTransition>> activePair in transitioning)
            {
                ProcessTransitions(activePair.Item1, activePair.Item2);
            }
            
            //Destroy this if no phases are active
        }

        //TODO: move transition checking to ActivePhase?
        public void InvokeTrigger(AttackTransition.TriggerType triggerType)
        {
            List<Tuple<ActivePhase, List<AttackTransition>>> activatingTransitions = new();

            

            foreach (ActivePhase phase in activePhases)
            {
                List<AttackTransition> transitions = CheckTransitions(phase, triggerType, GenerateEvent(phase, triggerType));
                if (transitions.Count > 0)
                {
                    activatingTransitions.Add(new(phase, transitions));
                }
            }

            foreach (Tuple<ActivePhase, List<AttackTransition>> phaseTransitions in activatingTransitions)
            {
                ProcessTransitions(phaseTransitions.Item1, phaseTransitions.Item2);
            }
        }

        internal AttackEvent GenerateEvent(ActivePhase phase, TriggerType triggerType, CollisionInfo? collisionInfo = null)
        {
            return new()
            {
                attacker = attacker,
                eventTriggerType = triggerType,
                phaseInfo = new() { TriggeringPhase = phase },
                weaponInfo = weaponInfo,
                collisionInfo = collisionInfo
            };
        }

        private void ProcessTransitions(ActivePhase phase, List<AttackTransition> transitions)
        {
            foreach (AttackTransition transition in transitions)
            {
                AdvancePhase(phase, transition);
                if (transition.destroyLastPhase)
                    return;
                else
                    phase.potentialTransitions.Remove(transition);
            }
        }

        private List<AttackTransition> CheckTransitions(ActivePhase phase, AttackTransition.TriggerType triggerType, AttackEvent e)
        {
            List<AttackTransition> activating = new List<AttackTransition>();
            foreach (AttackTransition transition in phase.potentialTransitions)
            {
                if (transition.triggerType == triggerType && transition.ShouldAdvance(e))
                    activating.Add(transition);
            }

            return activating;
        }

        /// <summary>
        /// Returns true if any phase not marked as independent is active. This means that the Attacker is still performing the attack.
        /// </summary>
        /// <returns></returns>
        public bool IsPerforming()
        {
            foreach (ActivePhase phase in activePhases)
            {
                if (phase.IsActive() && phase.Definition.activeLink)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if any part of the attack is still active, independent or not.
        /// When this is false, the attack will be destroyed and can no longer recieve updates or messages.
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            foreach (ActivePhase phase in activePhases)
            {
                if (phase.IsActive())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Whether the attack is in warmup, i.e., if no phase not marked as warmup has started.
        /// </summary>
        /// <returns></returns>
        public bool IsWarmup()
        {
            return warmup;
        }

        internal void AdvancePhase(ActivePhase lastPhase, AttackTransition transition)
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
                if (performing && lastPhase.Definition.isPerformed && !IsPerforming())
                    attacker.EndAttack();
            }
        }

        private void StartPhase(AttackPhaseDefinition phase, GameObject lastPhaseObject = null, bool lastPhaseFlipped = false)
        {
            if (warmup && !phase.isWarmup)
            {
                attacker.EndWarmup();
                warmup = false;
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
                case AttackPhaseDefinition.ProjectileLocale.Melee:
                case AttackPhaseDefinition.ProjectileLocale.Ranged:
                    flip = attacker.IsFacingLeft();
                    projectile = ProjectileUtils.CreateProjectile(phase.projectilePrefab, attacker.GameObject(), flip, phase.locale == AttackPhaseDefinition.ProjectileLocale.Melee, phaseProps);
                    break;
                case AttackPhaseDefinition.ProjectileLocale.Remote:
                    if (lastPhaseObject == null)
                        throw new MissingReferenceException("Tried to start remote phase with no parent!");
                    flip = lastPhaseFlipped;
                    projectile = ProjectileUtils.CreateProjectile(phase.projectilePrefab, lastPhaseObject, lastPhaseFlipped, false, phaseProps);
                    break;
                case AttackPhaseDefinition.ProjectileLocale.Replace:
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
                    wp.Create(weaponInfo?.WeaponInstance, phase.locale == AttackPhaseDefinition.ProjectileLocale.Melee);
                }
            }

            activePhases.Add(new ActivePhase(phase, projectile, this, flip));
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
                if (phase.Definition.isPerformed && phase.IsActive())
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
