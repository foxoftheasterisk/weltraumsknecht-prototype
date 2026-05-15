using Platformer.Mechanics;
using System;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// A condition that checks the phase projectile's movement against the given parameters.
    /// If any parameter fails, the condition is false.
    /// </summary>
    [CreateAssetMenu(fileName = "ProjectileMovementCondition", menuName = "Weapon Conditions/ProjectileMovementCondition")]
    public class ProjectileMovementCondition : TransitionCondition
    {
        public float minSpeed = 0;
        public float maxSpeed = float.PositiveInfinity;

        /// <summary>
        /// The minimum angular velocity (absolute value).
        /// </summary>
        public float minAngular = 0;
        public float maxAngular = float.PositiveInfinity;

        /// <summary>
        /// The direction to compare movement to.
        /// </summary>
        public Vector2 direction = Vector2.zero;
        public float maxAngle = 360;

        internal override bool CheckCondition(ActivePhase phaseState, PlayerController player)
        {
            if(phaseState.linkedProjectile == null)
            {
                Debug.Log("Attempted to check movement of phase with no projectile!");
                return false;
            }

            Rigidbody2D rb;
            if (!phaseState.linkedProjectile.TryGetComponent<Rigidbody2D>(out rb))
            {
                Debug.Log("Attempted to check movement of projectile with no Rigidbody!");
                return false;
            }

            Vector2 movement = rb.linearVelocity;

            if (movement.magnitude < minSpeed || movement.magnitude > maxSpeed)
                return false;

            float angularMag = Mathf.Abs(rb.angularVelocity);

            if (angularMag < minAngular || angularMag > maxAngular)
                return false;

            if (direction.magnitude > 0)
            {
                if (Vector2.Angle(direction, movement) < maxAngle)
                    return false;
            }

            return true;
        }
    }
}
