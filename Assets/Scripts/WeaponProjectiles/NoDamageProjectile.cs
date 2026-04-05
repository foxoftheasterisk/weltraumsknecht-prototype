using UnityEngine;
using Platformer.Mechanics;

///A basic projectile that does no damage and has no special interaction with enemies.
///Still causes knockback.
public class NoDamageProjectile : WeaponProjectile
{
    override public int GetDamage()
    {
        return 0;
    }
    
    override public void CollidedWithEnemy(EnemyController enemy, bool killed) { }
}