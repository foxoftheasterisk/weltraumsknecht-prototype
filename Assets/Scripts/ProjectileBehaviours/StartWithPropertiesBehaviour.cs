using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Projectile Behaviours/Starting Properties")]
public class StartWithPropertiesBehaviour : FlippableBehaviour
{
    public ProjectileProperties properties;
    private bool flip = false;

    public override void Flip()
    {
        flip = !flip;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 displace = (Vector3)properties.displace;
        if (flip)
            displace.x *= -1;
        transform.position += displace;

        Vector3 rotation = transform.localEulerAngles;
        rotation.z += flip ? properties.rotateMod * -1 : properties.rotateMod;
        transform.localEulerAngles = rotation;

        float radians = Mathf.Deg2Rad * properties.initialAngle;
        Vector2 initialVelocity = new Vector2((float)Mathf.Cos(radians), (float)Mathf.Sin(radians));
        if(flip)
            initialVelocity.x *= -1;
        initialVelocity *= properties.initialSpeed;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity += initialVelocity;
        rb.angularVelocity += flip ? properties.initialRotateVelocity * -1 : properties.initialRotateVelocity;

        Destroy(this);
    }
}
