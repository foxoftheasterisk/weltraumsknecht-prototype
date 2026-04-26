using UnityEngine;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;

///A projectile that crits after the first time it collides with an object.
public class CritAfterBounceProjectile : WeaponProjectile
{
    private bool hasBounced = false;
    
    override protected void InteractWith(Collider2D other)
    {
        base.InteractWith(other);

        hasBounced = true;
    }
    
    override public int GetDamage()
    {
        return weapon.GetDamage(hasBounced);
    }
}
