using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A script that randomizes the initial values for an attached projectile, then destroys itself.
/// Will overwrite any existing velocity settings on the attached object.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class RandomStartBehaviour : MonoBehaviour
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
    }

    public RandomRange displaceX;
    public RandomRange displaceY;

    public RandomRange velocityX;
    public RandomRange velocityY;

    public RandomRange angularVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector2 displace = new Vector2(displaceX.Generate(), displaceY.Generate());
        transform.position += (Vector3)displace;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 velocity = new Vector2(velocityX.Generate(), velocityY.Generate());
        rb.linearVelocity = velocity;
        rb.angularVelocity = angularVelocity.Generate();

        Destroy(this);
    }
}
