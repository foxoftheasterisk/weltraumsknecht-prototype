using UnityEngine;

namespace Weltraumsknecht.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class MovementAI : MonoBehaviour
    {
        public abstract void Move(Vector2 relativePlayerPosition);

        //TODO: Should movement AI handle knockback as well?

        protected Rigidbody2D body;

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }
    }
}
