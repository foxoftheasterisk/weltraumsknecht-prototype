using UnityEngine;

namespace Weltraumsknecht.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class MovementAI : MonoBehaviour
    {
        public abstract void Move();

        //TODO: Should movement AI handle knockback as well?
    }
}
