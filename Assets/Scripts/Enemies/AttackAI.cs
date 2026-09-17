using System;
using UnityEngine;

namespace Weltraumsknecht.Enemies
{

    /// <summary>
    /// A base class that defines and interface and implements common code for Attacks.
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(Enemy))]
    public abstract class AttackAI : MonoBehaviour
    {
        public bool IsActive
        { get; protected set; } = false;

        public Range range;
        public int priority = 0;
        public string animationTriggerName;

        private const string animationWarmupName = "warmupCompletion";

        public float warmupTime = 1.5f;
        private float elapsedTime;

        public bool InAttack
        { get; protected set; }

        protected Animator animator;
        protected Enemy enemy;

        void Awake()
        {
            animator = GetComponent<Animator>();
            enemy = GetComponent<Enemy>();
        }

        public bool IsInRange(Vector2 playerPosition)
        {
            return range.Test(playerPosition);
        }

        public virtual void StartWarmup()
        {
            enemy.FaceTowardsPlayer();
            IsActive = true;
            InAttack = false;
            elapsedTime = 0;
            animator.SetTrigger(animationTriggerName);
            animator.SetFloat(animationWarmupName, 0);
        }

        public void Continue()
        {
            if (InAttack)
            {
                ContinueAttack();
            }
            else if(IsActive)
            {
                elapsedTime += Time.deltaTime;
                animator.SetFloat(animationWarmupName, elapsedTime / warmupTime);
                if(elapsedTime >= warmupTime)
                {
                    InAttack = true;
                    StartAttack();
                }
            }
        }

        public virtual void CancelAttack()
        {
            IsActive = false;
        }

        protected abstract void StartAttack();
        protected abstract void ContinueAttack();

        protected void EndAttack()
        {
            IsActive = false;
            InAttack = false;
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
            if(direction != Vector2.zero)
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