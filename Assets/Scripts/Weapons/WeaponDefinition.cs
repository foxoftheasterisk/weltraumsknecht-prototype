using NUnit.Framework;
using Platformer.Mechanics;
using System.Collections.Generic;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{
    public class WeaponDefinition : ScriptableObject
    {
        public WeaponPhase initialPhase;
    
        public int standardDamage = 10;
        public int critDamage = 20;
        public float knockbackFactor = 1;
        //possibly these should be determined per-phase

        public enum CooldownType
        {
            Time,
            Hits
        };
        public CooldownType cooldownType = CooldownType.Time;
        public float cooldown;
    
        public Sprite icon;
    }
}
