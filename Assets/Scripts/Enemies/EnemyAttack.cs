using System;
using UnityEngine;
using Weltraumsknecht.Attacks;

namespace Weltraumsknecht.Enemies
{

    [CreateAssetMenu(fileName = "EnemyAttack", menuName = "Scriptable Objects/EnemyAttack")]
    public class EnemyAttack : ScriptableObject
    {
        public Range range;
        public int priority = 0;

        public AttackPhaseDefinition initialPhase;

        /// <summary>
        /// Given a RELATIVE player position, tests to see if the player is in range of this attack.
        /// </summary>
        /// <param name="playerPosition">The position of the player, relative to the attacker.</param>
        /// <returns>True if the player is in range, false otherwise.</returns>
        public bool IsInRange(Vector2 playerPosition)
        {
            return range.Test(playerPosition);
        }
    }

    
    [Serializable]
    public struct Range
    {
        /// <summary>
        /// The direction to test against, expressed as a Vector2.
        /// Horizontal directions will use their absolute value to test.
        /// If the vector is 0, 0 direction will be ignored.
        /// </summary>
        public Vector2 direction;

        /// <summary>
        /// How much the actual angle can vary from the direction given.
        /// </summary>
        public int tolerance;

        /// <summary>
        /// The shortest distance away a point can be and remain in range.
        /// </summary>
        public float minDistance;

        /// <summary>
        /// The farthest distance away a point can be and still be in range.
        /// </summary>
        public float maxDistance;

        internal readonly bool Test(Vector2 relativePosition)
        {
            if (direction != Vector2.zero)
            {
                Vector2 absPos = new Vector2(Math.Abs(relativePosition.x), relativePosition.y);
                Vector2 absDir = new Vector2(Math.Abs(direction.x), direction.y);

                if (Vector2.Angle(absDir, absPos) > tolerance)
                    return false;
            }

            float dist = relativePosition.magnitude;
            return dist <= maxDistance && dist >= minDistance;
        }
    }
    
}