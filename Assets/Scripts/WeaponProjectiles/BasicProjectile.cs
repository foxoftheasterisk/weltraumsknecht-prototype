using UnityEngine;
using Platformer.Mechanics;

/// <summary>
/// A basic projectile that either always or never crits.
/// </summary>
public class BasicProjectile : WeaponProjectile
{
    public bool crits;

    override public int GetDamage()
    {
        return weapon.GetDamage(crits);
    }

}
