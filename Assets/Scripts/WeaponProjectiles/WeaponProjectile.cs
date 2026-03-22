using UnityEngine;
using Platformer.Mechanics;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public abstract class WeaponProjectile : MonoBehaviour
{
    protected PlayerController player;
    public Vector2 initialVelocity;
    public float rotateVelocity;
    public int standardDamage = 9;
    public int critDamage = 18;
    
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
    public void Create(PlayerController player)
    {
        this.player = player;
        
        WeaponProjectile[] children = GetComponentsInChildren<WeaponProjectile>();
        foreach (WeaponProjectile child in children)
        {
            if(child != this)
                child.Create(player);
        }
    }

    // Update is called once per frame
    virtual public void Update() 
    {
        //...will this slow it down too much
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
        if(Vector2.Distance(rb.position, prb.position) > 100)
            Destroy(gameObject);
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
    
    public PlayerController GetPlayer()
    {
        return player;
    }
    
    protected virtual void InteractWith(Collider2D other)
    {
        var enemy = other.gameObject.GetComponent<EnemyController>();
        if (enemy != null)
        {
            var ev = Schedule<ProjectileEnemyCollision>();
            ev.projectile = this;
            ev.enemy = enemy;
        }
    }
    
    
    public abstract int GetDamage();
    
    public abstract void CollidedWithEnemy(EnemyController enemy, bool killed);
}
