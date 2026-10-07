using Platformer.Mechanics;
using UnityEngine;


namespace Weltraumsknecht.Enemies
{
    [AddComponentMenu("Enemies/Movement/Immobile")]
    public class ImmobileAI : MovementAI
    {
        public override void Move(Vector2 relativePlayerPosition)
        {
            //Do nothing, since the enemy is immobile.
        }
    }
}
