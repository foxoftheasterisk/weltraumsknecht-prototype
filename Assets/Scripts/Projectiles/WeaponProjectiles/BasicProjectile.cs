using UnityEngine;
using Platformer.Mechanics;

namespace Weltraumsknecht.Projectiles
{

    /// <summary>
    /// A basic projectile that either always or never crits.
    /// </summary>
    [AddComponentMenu("Projectiles/Weapon Projectiles/Basic Projectile")]
    public class BasicProjectile : WeaponProjectile
    {
        public bool crits;

        override public int GetDamage()
        {
            return weapon.GetDamage(crits);
        }

    }
}
