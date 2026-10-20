using UnityEngine;

namespace Weltraumsknecht.Enemies
{
    public interface MovementAI
    {
        public bool OnlyFlipsWhenAttacking
        { get; }

        public void Move(Vector2 relativePlayerPosition);

        public void StopMovement();

        public void SufferKnockback(Vector2 suggestedKnockback);
    }
}
