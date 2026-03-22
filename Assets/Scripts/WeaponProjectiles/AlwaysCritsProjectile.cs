using UnityEngine;
using Platformer.Mechanics;

///A basic projectile that always crits and has no interaction with enemies.
public class AlwaysCritsProjectile : WeaponProjectile
{
    override public int GetDamage()
    {
        return critDamage;
    }
    
    override public void CollidedWithEnemy(EnemyController enemy, bool killed) { }
}
