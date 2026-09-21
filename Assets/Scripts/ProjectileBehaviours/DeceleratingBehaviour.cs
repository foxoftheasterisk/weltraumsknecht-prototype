using UnityEngine;


/// <summary>
/// A simple behaviour that applies a constant deceleration per second every frame.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Projectile Behaviours/Decelerating")]
public class DeceleratingBehaviour : MonoBehaviour
{
    /// <summary>
    /// The deceleration, in units per second.
    /// </summary>
    public float deceleration;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vel = rb.linearVelocity;
        float speed = vel.magnitude;
        float deltaSpeed = deceleration * Time.deltaTime;
        speed = Mathf.Max(speed - deltaSpeed, 0);
        vel = Vector2.ClampMagnitude(vel, speed);
        rb.linearVelocity = vel;
    }
}
