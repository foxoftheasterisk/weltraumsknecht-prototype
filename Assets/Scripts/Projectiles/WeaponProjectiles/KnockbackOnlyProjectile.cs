using UnityEngine;
using Platformer.Mechanics;

namespace Weltraumsknecht.Projectiles
{
    ///A basic projectile that does no damage and has no special interaction with enemies.
    ///Still causes knockback.
    [AddComponentMenu("Projectiles/Weapon Projectiles/Knockback Only Projectile")]
    public class KnockbackOnlyProjectile : WeaponProjectile
    {
        override public int GetDamage()
        {
            return 0;
        }
    }
}