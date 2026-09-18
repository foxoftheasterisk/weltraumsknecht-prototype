using UnityEngine;
using Platformer.Mechanics;
using Weltraumsknecht.Enemies;


namespace Weltraumsknecht.Projectiles
{
    ///A simple projectile that only deals damage when moving faster than a given speed.
    ///Can be set to crit or not.
    [AddComponentMenu("Projectiles/Weapon Projectiles/Only Damage While Moving Projectile")]
    public class OnlyDamagesWhileMovingProjectile : WeaponProjectile
    {
        public bool crits;
        public float speedRequired = 0.5f;

        override public int GetDamage()
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb.linearVelocity.magnitude >= speedRequired)
            {
                if (crits)
                    return weapon.GetDamage(true);
                else
                    return weapon.GetDamage(false);
            }
            else
                return 0;
        }

        override public void CollidedWithEnemy(Enemy enemy, bool killed) { }
    }
}