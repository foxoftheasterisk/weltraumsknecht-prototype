using UnityEngine;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;

///A projectile that crits after the first time it collides with an object.
public class CritAfterBounceProjectile : WeaponProjectile
{
    private bool hasBounced = false;
    
    override public void CollidedWithAny(GameObject other)
    {
        hasBounced = true;
    }
    
    override public int GetDamage()
    {
        return weapon.GetDamage(hasBounced);
    }
}
