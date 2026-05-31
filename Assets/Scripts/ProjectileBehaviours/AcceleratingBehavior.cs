using UnityEngine;


/// <summary>
/// A simple behaviour that applies a constant acceleration per second every frame.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Projectile Behaviours/Accelerating")]
public class AcceleratingBehavior : FlippableBehaviour
{
    /// <summary>
    /// The acceleration, in units per second.
    /// </summary>
    public Vector2 acceleration;
    private Rigidbody2D rb;


    public override void Flip()
    {
        acceleration.x *= -1;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity += acceleration * Time.deltaTime;
    }
}
