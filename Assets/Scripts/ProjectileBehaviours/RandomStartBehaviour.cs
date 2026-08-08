using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;

/// <summary>
/// A script that randomizes the initial values for an attached projectile, then destroys itself.
/// All values are relative to any currently existing values on the projectile.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Projectile Behaviours/Random Start")]
public class RandomStartBehaviour : FlippableBehaviour
{
    [System.Serializable]
    public struct RandomRange
    {
        public float min;
        public float max;
        public bool randomizeSign;

        public float Generate()
        {
            float result = Random.Range(min, max);
            if (randomizeSign && Random.Range(-1, 1) < 0)
                result *= -1;

            return result;
        }

        /// <summary>
        /// Flips the range's signs. (For example, a range of 1 to 3 would become -3 to -1.)
        /// </summary>
        public void Invert()
        {
            if (randomizeSign)
                return; //There's no point inverting if the sign is randomized anyway

            float newMax = min * -1;
            min = max * -1;
            max = newMax;
        }
    }

    public RandomRange displaceX;
    public RandomRange displaceY;

    public RandomRange rotate;

    public RandomRange velocityX;
    public RandomRange velocityY;

    public RandomRange angularVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 displace = new Vector2(displaceX.Generate(), displaceY.Generate());
        transform.position += (Vector3)displace;

        Vector3 rotation = transform.localEulerAngles;
        rotation.z += rotate.Generate();
        transform.localEulerAngles = rotation;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 velocity = new Vector2(velocityX.Generate(), velocityY.Generate());
        rb.linearVelocity += velocity;
        rb.angularVelocity += angularVelocity.Generate();

        Destroy(this);
    }

    public override void Flip()
    {
        displaceX.Invert();
        rotate.Invert();
        velocityX.Invert();
        angularVelocity.Invert();
    }
}
