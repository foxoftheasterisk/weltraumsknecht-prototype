using UnityEngine;

namespace Weltraumsknecht.Enemies
{
    [AddComponentMenu("Enemies/Movement/Flying")]
    [RequireComponent(typeof(Rigidbody2D))]
    public class FlyingAI : MonoBehaviour, MovementAI
    {
        /// <summary>
        /// The target position, relative to the player.
        /// Y is literal, but X is treated as absolute distance.
        /// </summary>
        public Vector2 targetPosition;

        public float maxSpeed;
        public float acceleration;

        public float knockbackScale = 1;

        //I'm... reasonably certain this should always be false. If not, I can change it.
        public bool OnlyFlipsWhenAttacking => false;

        protected Rigidbody2D body;

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        public void Move(Vector2 relativePlayerPosition)
        {
            Vector2 targetChange = relativePlayerPosition + targetPosition;

            Vector2 velocity = body.linearVelocity;
            float deltaV = Time.deltaTime * acceleration;

            velocity = Vector2.MoveTowards(velocity, targetChange, deltaV); //not sure THAT'S right but we'll go with it for now
            velocity = Vector2.ClampMagnitude(velocity, maxSpeed); //might get awkward if a non-volitional velocity is faster than MaxSpeed

            body.linearVelocity = velocity;
        }

        public void StopMovement()
        {
            body.linearVelocity = Vector2.zero;
        }

        public void SufferKnockback(Vector2 suggestedKnockback)
        {
            body.linearVelocity = suggestedKnockback * knockbackScale;
        }
    }
}