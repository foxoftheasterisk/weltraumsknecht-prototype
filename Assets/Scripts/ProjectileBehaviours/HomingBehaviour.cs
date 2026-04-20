using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingBehaviour : MonoBehaviour
{
    public float targetSpeed = 7; //The intended speed for the attached projectile
    public float acceleration = 3; //How quickly to change the attached projectile's actual speed to its intended.
    public float rotateMax = 360; //Angle in degrees that this can turn in one second

    public Rigidbody2D target;

    private Rigidbody2D projectile;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectile = GetComponent<Rigidbody2D>();
        if (projectile == null)
        {
            Debug.Log("HomingBehaviour detected no Rigidbody2D!");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        //First manage speed
        Vector2 vel = projectile.linearVelocity;
        float speed = vel.magnitude;
        float deltaSpeed = acceleration * deltaTime; //not quite standard usage of delta but w/e
        if (speed < deltaSpeed)
        {
            //this is not the most elegant way to do this, but eh, it works
            Vector2 targetPos = target.position;
            Vector2 direction = (targetPos - projectile.position).normalized;

            Vector2 deltaV = direction * deltaSpeed;
            vel = Vector2.ClampMagnitude(vel, targetSpeed - deltaSpeed);
            vel = vel + deltaV;

            projectile.linearVelocity = vel;
            return;
        }
        else if (speed != targetSpeed)
        {
            if (speed > targetSpeed)
            {
                speed = Mathf.Max(speed - deltaSpeed, targetSpeed);
            } 
            else
            {
                speed = Mathf.Min(speed + deltaSpeed, targetSpeed);
            }

            vel = vel.normalized * speed;
        }

        //then manage rotation
        {
            Vector2 targetPos = target.position;
            Vector2 direction = (targetPos - projectile.position).normalized;
            float angle = Vector2.SignedAngle(vel, direction);

            float deltaRotate = deltaTime * rotateMax;

            if (Mathf.Abs(angle) < deltaRotate)
            {
                vel = direction * vel.magnitude;
            }
            else
            {
                if (angle < 0)
                {
                    deltaRotate = -deltaRotate;
                }

                float radianDeltaRotate = Mathf.Deg2Rad * deltaRotate;

                //now the tricky part
                float newX = vel.x * Mathf.Cos(radianDeltaRotate) - vel.y * Mathf.Sin(radianDeltaRotate);
                float newY = vel.x * Mathf.Sin(radianDeltaRotate) + vel.y * Mathf.Cos(radianDeltaRotate);

                vel = new Vector2(newX, newY);
            }
        }

        projectile.linearVelocity = vel;
    }
}
