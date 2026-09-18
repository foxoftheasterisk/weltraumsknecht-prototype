using UnityEngine;
using Platformer.Mechanics;

/// <summary>
/// A basic projectile that either always or never crits.
/// </summary>
[AddComponentMenu("Weapon Projectiles/Basic Projectile")]
public class BasicProjectile : WeaponProjectile
{
    public bool crits;

    override public int GetDamage()
    {
        return weapon.GetDamage(crits);
    }

}
