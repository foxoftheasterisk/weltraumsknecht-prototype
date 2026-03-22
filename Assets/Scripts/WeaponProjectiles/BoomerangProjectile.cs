using UnityEngine;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;


public class BoomerangProjectile : WeaponProjectile
{
    public bool returning = false;
    public float returnSpeed = 7;
    public float timeBeforeReturn = 3;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Start()
    {
        base.Start();
        returning = false;
        Invoke("TimeOut", timeBeforeReturn);
    }

    // Update is called once per frame
    override public void Update()
    {
        if (returning)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Vector2 playerPos = player.GetComponent<Rigidbody2D>().position;
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
        if (returning)
            return critDamage;
        else
            return standardDamage;
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
