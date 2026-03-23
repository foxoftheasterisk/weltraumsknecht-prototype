using UnityEngine;
using Platformer.Mechanics;

///A basic projectile that never crits and has no interaction with enemies.
public class NeverCritsProjectile : WeaponProjectile
{
    override public int GetDamage()
    {
        return weapon.GetDamage(false);
    }
    
    override public void CollidedWithEnemy(EnemyController enemy, bool killed) { }
}
