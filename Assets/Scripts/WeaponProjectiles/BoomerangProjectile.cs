using UnityEngine;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;


public class BoomerangProjectile : WeaponProjectile
{
    public bool returning = false;
    public float returnSpeed = 7;
    public float timeBeforeReturn = 3;
    
    private Rigidbody2D player;
    private Rigidbody2D rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Start()
    {
        base.Start();
        returning = false;
        Invoke("TimeOut", timeBeforeReturn);
        
        rb = GetComponent<Rigidbody2D>();
        
        player = weapon.player.GetComponent<Rigidbody2D>();
        if (player == null)
        {
            Debug.Log("Cannot find player Rigidbody2D for BoomerangProjectile");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    override public void Update()
    {
        if (returning)
        {
            Vector2 playerPos = player.position;
            Vector2 direction = playerPos - rb.position;
            Vector2 vel = Vector2.Normalize(direction) * returnSpeed;
            rb.linearVelocity = vel;
        }
    }
    
    override protected void InteractWith(Collider2D other)
    {
        base.InteractWith(other);
        
        if (returning && other.gameObject == player.gameObject)
        {
            Destroy(gameObject);
        }
    }
    
    override public int GetDamage()
    {
        return weapon.GetDamage(returning);
    }
    
    override public void CollidedWithEnemy(EnemyController enemy, bool killed)
    {
        returning = true;
    }
    
    private void TimeOut()
    {
        returning = true;
    }
}
