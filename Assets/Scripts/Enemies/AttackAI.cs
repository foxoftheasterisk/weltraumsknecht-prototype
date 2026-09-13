using System;
using UnityEngine;

namespace Weltraumsknecht.Enemies
{

    [RequireComponent(typeof(Animator))]
    public abstract class AttackAI : MonoBehaviour
    {
        public bool IsActive
        { get; protected set; } = false;

        public int priority = 0;
        public string animationTriggerName;

        private const string animationWarmupName = "warmupCompletion";

        public float warmupTime = .3f;
        private float elapsedTime;

        private bool inAttack;

        protected Animator animator;

        void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public abstract bool IsInRange(Vector2 playerPosition);

        public virtual void StartWarmup()
        {
            IsActive = true;
            inAttack = false;
            elapsedTime = 0;
            animator.SetTrigger(animationTriggerName);
            animator.SetFloat(animationWarmupName, 0);
        }

        public void Continue()
        {
            if (inAttack)
            {
                ContinueAttack();
            }
            else
            {
                elapsedTime += Time.deltaTime;
                animator.SetFloat(animationWarmupName, elapsedTime / warmupTime);
                if(elapsedTime >= warmupTime)
                {
                    inAttack = true;
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
            inAttack = false;
        }
    }
}