using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weltraumsknecht.Weapons.WeaponDefinition;


using Weltraumsknecht.Weapons;

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
                if(activating.Count > 0)
                {
                    active.Add(new(phase, activating));
                }
            }
        }

        foreach(ActivePhase phase in inactive)
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
            if(transitions.Count > 0)
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
        foreach(WeaponTransition transition in transitions)
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
            StartPhase(transition.nextPhase, lastPhase.linkedProjectile);
        }

        if (transition.destroyLastPhase)
        {
            activePhases.Remove(lastPhase);
            if(lastPhase.linkedProjectile != null)
                GameObject.Destroy(lastPhase.linkedProjectile);
        }
    }

    private void StartPhase(WeaponPhase phase, GameObject lastPhaseObject = null)
    {
        if (!phase.isWarmup)
        {
            CooldownRemaining = definition.cooldown;
        }

        if (phase.projectilePrefab == null)
        {
            activePhases.Add(new ActivePhase(phase, null, this));
            return;
        }

        GameObject projectile;
        ProjectileProperties? phaseProps = null;
        if (phase.useProperties)
            phaseProps = phase.initialProperties;

        switch (phase.locale)
        {
            case WeaponPhase.ProjectileLocale.Melee:
            case WeaponPhase.ProjectileLocale.Ranged:
                projectile = CreateProjectile(phase.projectilePrefab, phase.locale == WeaponPhase.ProjectileLocale.Melee, phase.initialProperties);
                break;
            case WeaponPhase.ProjectileLocale.Remote:
                if (lastPhaseObject == null)
                    throw new MissingReferenceException("Tried to start remote phase with no parent!");
                projectile = CreateProjectile(phase.projectilePrefab, false, phase.initialProperties, lastPhaseObject);
                break;
            case WeaponPhase.ProjectileLocale.Replace:
                if (lastPhaseObject == null)
                    throw new MissingReferenceException("Tried to start replace phase with no parent!");
                projectile = CreateProjectile(phase.projectilePrefab, false, phase.initialProperties, lastPhaseObject, true);
                break;
            default:
                throw new NotImplementedException("Undefined projectile locale: " + phase.locale);
        }

        activePhases.Add(new ActivePhase(phase, projectile, this));
    }

    /// <summary>
    ///Creates a given projectile (or multiple projectiles in one prefab)
    ///If melee is true, the projectile is created as a child of the parent (and therefore will move with them);
    ///if false, the projectile is created at the parent's location, but not as a child.
    ///If no parent is passed, the player will be used.
    ///(Do not pass in the player, as this will make projectile flipping inaccurate.)
    ///Returns the created projectile.
    /// </summary>
    internal GameObject CreateProjectile(GameObject prefab, bool melee = false, ProjectileProperties? properties = null, GameObject parent = null, bool inheritProperties = false)
    {
        GameObject projectile;
        bool flip = false;

        if (parent == null)
        {
            parent = Player.gameObject;
            flip = Player.IsFacingLeft();
        }
        else if (parent.TryGetComponent<Rigidbody2D>(out Rigidbody2D parentrb))
            flip = parentrb.linearVelocityX < 0;

        if (melee)
        {
            projectile = GameObject.Instantiate(prefab, parent.transform);
        }
        else
        {
            Vector3 relativePosition = prefab.transform.position;
            if (flip)
                relativePosition.x *= -1;
            Vector3 position = parent.transform.position + relativePosition;
            projectile = GameObject.Instantiate(prefab, position, prefab.transform.rotation);
        }

        if (properties != null)
        {
            if (!projectile.TryGetComponent<Rigidbody2D>(out _))
            {
                Debug.Log("Tried to apply phase properties to projectile with no rigidbody. Continuing without properties.");
            }
            else
            {
                StartWithPropertiesBehaviour behaviour = projectile.AddComponent<StartWithPropertiesBehaviour>();
                behaviour.properties = (ProjectileProperties)properties;
            }
        }

        if (flip)
        {
            Vector3 scale = projectile.transform.localScale;
            scale.x *= -1;
            projectile.transform.localScale = scale;

            Vector3 rotation = projectile.transform.localEulerAngles;
            rotation.z *= -1;
            projectile.transform.localEulerAngles = rotation;

            if (melee)
            {
                Vector3 position = projectile.transform.localPosition;
                position.x *= -1;
                projectile.transform.localPosition = position;
            }

            foreach (FlippableBehaviour flippable in projectile.GetComponents<FlippableBehaviour>())
            {
                flippable.Flip();
            }
        }

        WeaponProjectile[] wps = projectile.GetComponentsInChildren<WeaponProjectile>();
        foreach (WeaponProjectile wp in wps)
        {
            wp.Create(this, melee);
        }

        if (inheritProperties)
        {
            projectile.transform.localEulerAngles += parent.transform.localEulerAngles;
            if (projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D projectileRB) && parent.TryGetComponent<Rigidbody2D>(out Rigidbody2D parentRB)
                && projectileRB.bodyType != RigidbodyType2D.Static && parentRB.bodyType != RigidbodyType2D.Static)
            {
                projectileRB.linearVelocity += parentRB.linearVelocity;
                projectileRB.angularVelocity += parentRB.angularVelocity;
            }
        }

        return projectile;
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

