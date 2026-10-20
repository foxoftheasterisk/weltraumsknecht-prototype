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
    public class ActiveAttack
    {
        private List<ActivePhase> activePhases;

        private Attacker attacker;
        private WeaponInfo? weaponInfo;

        private bool warmup = true;
        private bool performing = true;

        public ActiveAttack(Attacker attacker, AttackPhaseDefinition initialPhase, WeaponInfo? weaponInfo = null)
        {
            this.attacker = attacker;
            this.weaponInfo = weaponInfo;

            activePhases = new();
            StartPhase(initialPhase);
            AttackManager.Instance.AddAttack(this);
        }

        public void Update()
        {

            List<ActivePhase> inactive = activePhases.FindAll(p => !p.IsActive());
            foreach (ActivePhase phase in inactive)
            {
                EndPhase(phase);
            }

            //This is one more iteration than is strictly necessary,
            //but the phase lists should be short enough it doesn't really matter.
            foreach(ActivePhase phase in activePhases)
            {
                phase.AdvanceTime(Time.deltaTime);
            }

            InvokeTrigger(TriggerType.Update);
        }

        //TODO: move transition checking to ActivePhase?
        public void InvokeTrigger(AttackTransition.TriggerType triggerType, AnimationInfo? animationInfo = null, CollisionInfo? collisionInfo = null)
        {
            List<Tuple<ActivePhase, List<AttackTransition>>> activatingTransitions = new();

            foreach (ActivePhase phase in activePhases)
            {
                List<AttackTransition> transitions = CheckTransitions(phase, triggerType, GenerateEvent(phase, triggerType, collisionInfo: collisionInfo, animationInfo: animationInfo));
                if (transitions.Count > 0)
                {
                    activatingTransitions.Add(new(phase, transitions));
                    //Have to store these since if we processed the transitions immediately, it could remove the phase and mess up the iteration.
                }
            }

            foreach (Tuple<ActivePhase, List<AttackTransition>> phaseTransitions in activatingTransitions)
            {
                ProcessTransitions(phaseTransitions.Item1, phaseTransitions.Item2);
            }
        }

        internal AttackEvent GenerateEvent(ActivePhase phase, TriggerType triggerType, CollisionInfo? collisionInfo = null, AnimationInfo? animationInfo = null)
        {
            return new()
            {
                attacker = attacker,
                eventTriggerType = triggerType,
                phaseInfo = new() { TriggeringPhase = phase },
                weaponInfo = weaponInfo,
                collisionInfo = collisionInfo,
                animationInfo = animationInfo
            };
        }

        internal void ProcessTransitions(ActivePhase phase, List<AttackTransition> transitions)
        {
            foreach (AttackTransition transition in transitions)
            {
                AdvancePhase(phase, transition);
                if (transition.endLastPhase)
                    return;
                else
                    phase.potentialTransitions.Remove(transition);
            }
        }

        internal List<AttackTransition> CheckTransitions(ActivePhase phase, AttackTransition.TriggerType triggerType, AttackEvent e)
        {
            List<AttackTransition> activating = new();
            foreach (AttackTransition transition in phase.potentialTransitions)
            {
                if (transition.triggerType == triggerType && transition.ShouldAdvance(e))
                {
                    activating.Add(transition);
                    if (transition.consumeTrigger)
                        break;
                }
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
                if (phase.IsActive() && phase.Definition.isPerformed)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if any phase of the attack is still active, independent or not.
        /// When this is false, the attack will be destroyed and can no longer recieve updates or messages.
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return activePhases.Count != 0;
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

            if (transition.endLastPhase)
            {
                EndPhase(lastPhase);
            }
        }

        private void StartPhase(AttackPhaseDefinition phase, GameObject lastPhaseObject = null, bool lastPhaseFlipped = false)
        {
            if (warmup && !phase.isWarmup)
            {
                attacker.EndWarmup();
                warmup = false;
            }

            //TODO: move the rest of this method into ActivePhase

            if (phase.animation)
                attacker.TriggerAnimation(phase.animationTriggerName);

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

        private void EndPhase(ActivePhase phase)
        {
            if (!activePhases.Contains(phase))
                return; //This phase has already been ended

            activePhases.Remove(phase);

            if (phase.linkedProjectile != null)
                GameObject.Destroy(phase.linkedProjectile);

            if (performing && !IsPerforming())
                attacker.EndAttack();
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
        /// Ends the attack, canceling any phases that are performed by the actor.
        /// Non-performed phases may continue.
        /// </summary>
        public void CancelAttack()
        {
            List<ActivePhase> performing = activePhases.FindAll(p => p.Definition.isPerformed);
            foreach (ActivePhase phase in performing)
                EndPhase(phase);
        }

    }

}
