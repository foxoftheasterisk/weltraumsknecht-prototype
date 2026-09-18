using UnityEngine;
using Platformer.Mechanics;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using System;

using Weltraumsknecht.Weapons;
using Weltraumsknecht.Enemies;
using Weltraumsknecht.Projectiles;

public abstract class WeaponProjectile : Projectile
{
    protected WeaponInstance weapon;
    private bool melee;
    
    //Create is called by the Weapon that created this projectile, in order to pass along parameters
    public void Create(WeaponInstance weapon, bool melee)
    {
        this.weapon = weapon;
        this.melee = melee;
    }

    // Update is called once per frame
    virtual public void Update() 
    {
        //...will this slow it down too much
        //doesn't seem to so far. Probably fine as long as we don't do projectile spam.
        /*
        if (!melee)
        {
            PlayerController player = weapon.Player;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
            if(Vector2.Distance(rb.position, prb.position) > 100)
                Destroy(gameObject);
        }
        //*/
        //This should no longer be necessary...
    }
    
    public WeaponInstance GetWeapon()
    {
        return weapon;
    }
    
    protected override void InteractWith(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            ProjectileEnemyCollision ev = Schedule<ProjectileEnemyCollision>();
            ev.projectile = this;
            ev.enemy = enemy;
        }
        else
        {
            CollidedWithOther(other.gameObject);
        }
    }
    
    
    public abstract int GetDamage();
    
    ///Gets the raw knockback velocity. Enemies may still have to modify this based on position (e.g. if grounded).
    public virtual Vector2 GetKnockback(Vector2 enemyPosition)
    {
        //starting with a naive algorithm
        Rigidbody2D actor;
        if (melee) {
            actor = weapon.Player.GetComponent<Rigidbody2D>();
        }
        else {
            actor = GetComponent<Rigidbody2D>();
        }
        
        Vector2 center = actor.position;
        Vector2 fromCenter = enemyPosition - center;
        
        Vector2 velocity = actor.linearVelocity;
        
        Vector2 direction = fromCenter + velocity;
        direction.Normalize();
        
        return direction * weapon.definition.knockbackFactor;
        
    }
    
    public virtual void CollidedWithEnemy(Enemy enemy, bool killed) 
    {
        //CollidedWith(enemy.gameObject);
    }

    public virtual void CollidedWithOther(GameObject other) 
    {
        CollidedWith(other);
    }
}
