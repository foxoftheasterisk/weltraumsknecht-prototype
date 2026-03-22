using UnityEngine;
using Platformer.Mechanics;

public abstract class Weapon : ScriptableObject
{
    public bool blocksWeapons = false;
    public bool blocksMovement = false;
    public bool blocksFacing = false;
    
    public float cooldown;
    protected float cooldownRemaining;
    
    public abstract void ButtonPressed();
    
    public abstract void ButtonReleased();
    
    [HideInInspector]
    public PlayerController player;
    
    public bool CanFire()
    {
        return !IsActive() && cooldownRemaining <= 0;
    }
    
    protected virtual void Fire()
    {
        cooldownRemaining = cooldown;
    }
    
    protected abstract bool IsActive();
    
    ///Creates a given projectile (or multiple projectiles in one prefab)
    ///If melee is true, the projectile is created as a child of the player (and therefore will move with them);
    ///if false, the projectile is created at the player's location, but not as a child.
    ///Returns the created projectile.
    protected GameObject CreateProjectile(GameObject prefab, bool melee = false)
    {
        GameObject projectile;
        bool flip = player.IsFacingLeft();
        
        if (melee)
        {
            projectile = Instantiate(prefab, player.transform);
        }
        else
        {
            Vector3 position = player.transform.position + prefab.transform.position;
            projectile = Instantiate(prefab, position, Quaternion.identity);
        }
        
        if (flip)
        {
            Vector3 scale = projectile.transform.localScale;
            scale.x *= -1;
            projectile.transform.localScale = scale;
            
            Vector3 rotation = projectile.transform.localEulerAngles;
            rotation.z *= -1;
            projectile.transform.localEulerAngles = rotation;
        }
        
        WeaponProjectile[] wps = projectile.GetComponentsInChildren<WeaponProjectile>();
        foreach (WeaponProjectile wp in wps)
        {
            wp.Create(player);
            
            //a weaponprojectile requires a rigidbody, so this should be safe
            Rigidbody2D rb = wp.GetComponent<Rigidbody2D>();
            if (rb.bodyType != RigidbodyType2D.Static)
            {
                if (flip)
                {
                    rb.linearVelocity = new Vector2(wp.initialVelocity.x * -1, wp.initialVelocity.y);
                    rb.angularVelocity = wp.rotateVelocity * -1;
                }
                else
                {
                    rb.linearVelocity = wp.initialVelocity;
                    rb.angularVelocity = wp.rotateVelocity;
                }
            }
        }
        
        return projectile;
    }
    
    public virtual void Update()
    {
        if (!IsActive() && cooldownRemaining > 0)
            cooldownRemaining -= Time.deltaTime;
    }
    
    public virtual bool IsBlockingFacing()
    {
        return blocksFacing && IsActive();
    }
    
    public virtual bool IsBlockingMovement()
    {
        return blocksMovement && IsActive();
    }
    
    public virtual bool IsBlockingWeapons()
    {
        return blocksWeapons && IsActive();
    }
}
