using UnityEngine;
using Platformer.Mechanics;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using System;

using Weltraumsknecht.Weapons;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class WeaponProjectile : MonoBehaviour
{
    protected WeaponInstance weapon;
    public Vector2 initialVelocity;
    public float rotateVelocity;
    
    private bool melee;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    virtual public void Start()
    { 
        /*
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb.bodyType != RigidbodyType2D.Static)
        {
            if (player.IsFacingLeft())
            {
                rb.linearVelocity = new Vector2(initialVelocity.x * -1, initialVelocity.y);
                rb.angularVelocity = rotateVelocity * -1;
            }
            else
            {
                rb.linearVelocity = initialVelocity;
                rb.angularVelocity = rotateVelocity;
            }
        }
        //*/
    }
    
    //Create is called by the Weapon that created this projectile, in order to pass along parameters
    public void Create(WeaponInstance weapon, bool melee)
    {
        this.weapon = weapon;
        this.melee = melee;
        
        WeaponProjectile[] children = GetComponentsInChildren<WeaponProjectile>();
        foreach (WeaponProjectile child in children)
        {
            if(child != this)
                child.Create(weapon, melee);
        }
    }

    // Update is called once per frame
    virtual public void Update() 
    {
        //...will this slow it down too much
        //doesn't seem to so far. Probably fine as long as we don't do projectile spam.
        if (!melee)
        {
            PlayerController player = weapon.player;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
            if(Vector2.Distance(rb.position, prb.position) > 100)
                Destroy(gameObject);
        }
        //*/
    }
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        InteractWith(collision.collider);
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        InteractWith(other);
    }
    
    public WeaponInstance GetWeapon()
    {
        return weapon;
    }
    
    protected void InteractWith(Collider2D other)
    {
        var enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            var ev = Schedule<ProjectileEnemyCollision>();
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
            actor = weapon.player.GetComponent<Rigidbody2D>();
        }
        else {
            actor = GetComponent<Rigidbody2D>();
        }
        
        Vector2 center = actor.position;
        Vector2 fromCenter = enemyPosition - center;
        
        Vector2 velocity = actor.linearVelocity;
        
        Vector2 direction = fromCenter + velocity;
        direction.Normalize();
        
        return direction * weapon.Definition.knockbackFactor;
        
    }
    
    public virtual void CollidedWithEnemy(EnemyController enemy, bool killed) 
    {
        CollidedWithAny(enemy.gameObject);
    }

    public virtual void CollidedWithOther(GameObject other) 
    {
        CollidedWithAny(other);
    }

    public virtual void CollidedWithAny(GameObject other) { }
}
