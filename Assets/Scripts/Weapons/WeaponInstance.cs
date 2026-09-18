using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weltraumsknecht.Weapons.WeaponDefinition;

using Weltraumsknecht.Projectiles;

namespace Weltraumsknecht.Weapons
{

    [Serializable]
    public class WeaponInstance
    {
        [SerializeField]
        public WeaponDefinition definition;

        private List<ActivePhase> activePhases;

        [DoNotSerialize]
        public float CooldownRemaining
        {
            get;
            private set;
        }
        //we will probably eventually want a public ReduceCooldown method

        [HideInInspector, DoNotSerialize]
        public PlayerController Player
        {
            get;
            private set;
        }

        public WeaponInstance(WeaponDefinition _definition)
        {
            definition = _definition;
        }

        public void Initialize(PlayerController player)
        {
            activePhases = new List<ActivePhase>();
            Player = player;
        }

        public void Update()
        {
            List<ActivePhase> inactive = new();
            List<Tuple<ActivePhase, List<WeaponTransition>>> active = new();
            foreach (ActivePhase phase in activePhases)
            {
                if (!phase.IsActive())
                {
                    inactive.Add(phase);
                }
                else
                {
                    phase.AdvanceTime(Time.deltaTime);
                    List<WeaponTransition> activating = CheckTransitions(phase, WeaponTransition.TriggerType.Update, new WeaponEvent(phase, Player));
                    if (activating.Count > 0)
                    {
                        active.Add(new(phase, activating));
                    }
                }
            }

            foreach (ActivePhase phase in inactive)
            {
                activePhases.Remove(phase);
            }

            foreach (Tuple<ActivePhase, List<WeaponTransition>> activePair in active)
            {
                ProcessTransitions(activePair.Item1, activePair.Item2);
            }

            if (CooldownRemaining > 0 && definition.cooldownType == CooldownType.Time)
                CooldownRemaining -= Time.deltaTime;
        }

        public void ButtonPressed()
        {
            ProcessAllTransitions(WeaponTransition.TriggerType.ButtonPress);

            if (CanFire())
            {
                Fire();
            }
        }

        public void ButtonReleased()
        {
            ProcessAllTransitions(WeaponTransition.TriggerType.ButtonRelease);

        }

        //TODO: move transition checking to ActivePhase
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

        public bool CanFire()
        {
            return !IsActive() && CooldownRemaining <= 0;
        }

        protected virtual void Fire()
        {
            StartPhase(definition.initialPhase);
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
                CooldownRemaining = definition.cooldown;
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

        public virtual bool IsBlockingWeapons()
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
    }
}
