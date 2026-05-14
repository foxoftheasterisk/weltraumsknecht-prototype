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
        if (IsActive())
        {
            List<Tuple<ActivePhase, List<WeaponTransition>>> inactive = new();
            List<Tuple<ActivePhase, List<WeaponTransition>>> active = new();
            foreach (ActivePhase phase in activePhases)
            {
                if (!phase.IsActive())
                {
                    inactive.Add(new(phase, CheckTransitions(phase, WeaponTransition.TriggerType.Inactivate)));
                }
                else
                {
                    phase.AdvanceTime(Time.deltaTime);
                    List<WeaponTransition> activating = CheckTransitions(phase, WeaponTransition.TriggerType.Update);
                    if(activating.Count > 0)
                    {
                        active.Add(new(phase, activating));
                    }
                }
            }

            foreach(Tuple<ActivePhase, List<WeaponTransition>> inactivePair in inactive)
            {
                if (inactivePair.Item2.Count > 0)
                {
                    Debug.Log("Inactive phase transition");
                    ProcessTransitions(inactivePair.Item1, inactivePair.Item2);
                }
                activePhases.Remove(inactivePair.Item1);
            }

            foreach (Tuple<ActivePhase, List<WeaponTransition>> activePair in active)
            {
                ProcessTransitions(activePair.Item1, activePair.Item2);
            }

        }
        else if (CooldownRemaining > 0 && definition.cooldownType == CooldownType.Time)
            CooldownRemaining -= Time.deltaTime;
    }

    public void ButtonPressed()
    {
        if (IsActive())
        {
            ProcessAllTransitions(WeaponTransition.TriggerType.ButtonPress);
        }
        else if (CanFire())
        {
            Fire();
        }
    }

    public void ButtonReleased()
    {
        if (IsActive())
        {
            ProcessAllTransitions(WeaponTransition.TriggerType.ButtonRelease);
        }
    }

    //TODO: move transition checking to ActivePhase
    private void ProcessAllTransitions(WeaponTransition.TriggerType triggerType)
    {
        List<Tuple<ActivePhase, List<WeaponTransition>>> activatingTransitions = new();

        foreach (ActivePhase phase in activePhases)
        {
            List<WeaponTransition> transitions = CheckTransitions(phase, triggerType);
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

    private List<WeaponTransition> CheckTransitions(ActivePhase phase, WeaponTransition.TriggerType triggerType)
    {
        List<WeaponTransition> activating = new List<WeaponTransition>();
        foreach (WeaponTransition transition in phase.potentialTransitions)
        {
            if (transition.triggerType == triggerType && transition.ShouldAdvance(phase, Player))
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

        switch (phase.locale)
        {
            case WeaponPhase.ProjectileLocale.Melee:
            case WeaponPhase.ProjectileLocale.Ranged:
                projectile = CreateProjectile(phase.projectilePrefab, phase.locale == WeaponPhase.ProjectileLocale.Melee);
                break;
            case WeaponPhase.ProjectileLocale.Remote:
                if (lastPhaseObject == null)
                    throw new MissingReferenceException("Tried to start remote phase with no parent!");
                projectile = CreateProjectile(phase.projectilePrefab, false, lastPhaseObject);
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
    ///(Do not pass in the player, as this will prevent projectile flipping.)
    ///Returns the created projectile.
    /// </summary>
    internal GameObject CreateProjectile(GameObject prefab, bool melee = false, GameObject parent = null)
    {
        GameObject projectile;
        bool flip = false;

        if (parent == null)
        {
            parent = Player.gameObject;
            flip = Player.IsFacingLeft();
        }

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

            //a weaponprojectile requires a rigidbody, so this should be safe
            Rigidbody2D rb = wp.GetComponent<Rigidbody2D>();
            if (rb.bodyType != RigidbodyType2D.Static)
            {
                if (flip)
                {
                    rb.linearVelocity = new Vector2(wp.initialVelocity.x * -1, wp.initialVelocity.y);
                    rb.angularVelocity = wp.rotateVelocity * -1;
                }
                else
                {
                    rb.linearVelocity = wp.initialVelocity;
                    rb.angularVelocity = wp.rotateVelocity;
                }
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

