using UnityEngine;
using Platformer.Mechanics;

///A simple projectile that only deals damage when moving faster than a given speed.
///Can be set to crit or not.
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
    
    override public void CollidedWithEnemy(EnemyController enemy, bool killed) { }
}
