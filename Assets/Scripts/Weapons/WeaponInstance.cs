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

        [DoNotSerialize]
        public float CooldownRemaining
        {
            get;
            private set;
        }

        [HideInInspector, DoNotSerialize]
        public PlayerAttackController Player
        {
            get;
            private set;
        }//TODO: I don't think this is needed anymore, but I'm not sure.

        public WeaponInstance(WeaponDefinition _definition)
        {
            definition = _definition;
        }

        public void Initialize(PlayerAttackController player)
        {
            Player = player;
        }

        public void Update()
        {
            if (CooldownRemaining > 0 && definition.cooldownType == CooldownType.Time)
                CooldownRemaining -= Time.deltaTime;
        }

        {
        }

        public bool CanFire()
        {
        }

        public int GetDamage(bool crit)
        {
            if (crit)
                return definition.critDamage;
            else
                return definition.standardDamage;
        }

        {
        }
    }
}
