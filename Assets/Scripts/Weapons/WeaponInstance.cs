using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weltraumsknecht.Weapons.WeaponDefinition;

using Weltraumsknecht.Attacks;
using Weltraumsknecht.Player;

namespace Weltraumsknecht.Weapons
{

    [Serializable]
    public class WeaponInstance
    {
        [SerializeField]
        public WeaponDefinition definition;

        private AttackManager currentAttack;

        [DoNotSerialize]
        public float CooldownRemaining
        {
            get;
            private set;
        }
        //we will probably eventually want a public ReduceCooldown method

        [HideInInspector, DoNotSerialize]
        public PlayerMovementController Player
        {
            get;
            private set;
        }

        public WeaponInstance(WeaponDefinition _definition)
        {
            definition = _definition;
        }

        public void Initialize(PlayerMovementController player)
        {
            Player = player;
        }

        public void Update()
        {
            //call AttackManager's Update?

            if (CooldownRemaining > 0 && definition.cooldownType == CooldownType.Time)
                CooldownRemaining -= Time.deltaTime;
        }

        public void ButtonPressed()
        {
            if (currentAttack != null)
            {
                currentAttack.InvokeTrigger(AttackTransition.TriggerType.ButtonPress);
            }
            else if (CanFire())
            {
                Fire();
            }
        }

        public void ButtonReleased()
        {
            if (currentAttack != null)
            {
                currentAttack.InvokeTrigger(AttackTransition.TriggerType.ButtonRelease);
            }
        }

        public bool CanFire()
        {
            return !IsActive() && CooldownRemaining <= 0;
        }

        protected virtual void Fire()
        {
            currentAttack = new(definition.initialPhase);
        }

        public bool IsActive()
        {
            if (currentAttack == null)
                return false;
            else
                return currentAttack.IsActive();
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
            return currentAttack.IsBlockingFacing();
        }

        public virtual bool IsBlockingMovement()
        {
            if (!IsActive())
                return false;
            return currentAttack.IsBlockingMovement();
        }

        public virtual bool IsBlockingWeapons()
        {
            if (!IsActive())
                return false;
            return currentAttack.IsBlockingAttacks();
        }
    }
}
