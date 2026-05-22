using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StartWithPropertiesBehaviour : FlippableBehaviour
{
    public ProjectileProperties properties;

    public override void Flip()
    {
        properties.Flip();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position += (Vector3)properties.displace;

        Vector3 rotation = transform.localEulerAngles;
        rotation.z += properties.rotateMod;
        transform.localEulerAngles = rotation;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity += properties.initialVelocity;
        rb.angularVelocity += properties.initialRotateVelocity;
    }

    // Update is called once per frame
    void Update() { }
}
