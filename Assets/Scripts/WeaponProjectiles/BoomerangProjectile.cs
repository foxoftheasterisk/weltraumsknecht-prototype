using UnityEngine;
using Platformer.Mechanics;
using static Platformer.Core.Simulation;

///A projectile that throws in its initial direction for a time, hangs for a time, then returns to the player.
///Crits when hanging or returning.
///If it hits an object before it begins hanging, it will immediately begin returning.
///Behavior may be strange if acceleration exceeds returnSpeed.
public class BoomerangProjectile : WeaponProjectile
{
    public float returnSpeed = 7;
    public float throwTime = 3;
    public float hangTime = .8f;
    public float acceleration = 3;
    
    private enum TravelState
    {
        Throw,
        Hang,
        Return
    }
    private TravelState state;
    
    private Rigidbody2D player;
    private Rigidbody2D rb;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Start()
    {
        base.Start();
        state = TravelState.Throw;
        Invoke("StartHang", throwTime);
        
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
        if (state == TravelState.Throw)
            return; //In "throw", the physics engine handles movement.
        
        
        
        if (state == TravelState.Hang)
        {
            Vector2 vel = rb.linearVelocity;
            float speed = vel.magnitude;
            float deltaSpeed = acceleration * Time.deltaTime;
            speed = Mathf.Max(speed - deltaSpeed, 0);
            vel = Vector2.ClampMagnitude(vel, speed);
            rb.linearVelocity = vel;
        } 
    }
    
    override public void CollidedWithAny(GameObject other)
    {
        if (state == TravelState.Return && Object.ReferenceEquals(other, player.gameObject))
        {
            Destroy(gameObject);
        }
        else if (!Object.ReferenceEquals(other, player.gameObject))
        {
            if (state == TravelState.Throw)
                StartReturn();
        }
    }
    
    override public int GetDamage()
    {
        return weapon.GetDamage(state != TravelState.Throw);
    }
    
    private void StartHang()
    {
        if (state == TravelState.Throw)
        {
            state = TravelState.Hang;
            Invoke("StartReturn", hangTime);
            //Possibly "hang" should also be a trigger-only mode?
        }
    }
    
    private void StartReturn()
    {
        state = TravelState.Return;
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
        HomingBehaviour hb = gameObject.AddComponent<HomingBehaviour>();

        hb.targetSpeed = returnSpeed;
        hb.acceleration = acceleration;
        hb.target = player;
        //Rotate max will need to be passed through as well
    }
}
