using Platformer.Mechanics;
using UnityEngine;


namespace Weltraumsknecht.Enemies
{
    [AddComponentMenu("Enemies/Movement/Immobile")]
    public class ImmobileAI : MonoBehaviour, MovementAI
    {
        public bool OnlyFlipsWhenAttacking
        { get; set; }

        public void Move(Vector2 relativePlayerPosition)
        {
            //Do nothing, since the enemy is immobile.
        }

        public void StopMovement()
        {
            //Do nothing, there is no movement to stop.
        }

        public void SufferKnockback(Vector2 suggestedKnockback)
        {
            //Do nothing, the enemy cannot be moved.
        }
    }
}
