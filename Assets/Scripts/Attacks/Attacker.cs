using System;
using System.ComponentModel;
using UnityEngine;
using Weltraumsknecht.Enemies;
using Weltraumsknecht.Projectiles;

namespace Weltraumsknecht.Attacks
{
    /// <summary>
    /// Shared base for entities that can host attacks (i.e. enemies and player)
    /// </summary>
    [RequireComponent(typeof(Animator), typeof(ContactListener), typeof(Rigidbody2D))]
    public abstract class Attacker : MonoBehaviour
    {
        protected ActiveAttack currentAttack;

        protected Animator animator;

        private Rigidbody2D body;
        private RigidbodyType2D bodyType;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            GetComponent<ContactListener>().function = AttackerCollision;
            body = GetComponent<Rigidbody2D>();
            bodyType = body.bodyType;
        }

        public abstract bool IsFacingLeft();

        /// <summary>
        /// Called by an attack when its first non-warmup phase is started. Generally used to start cooldowns.
        /// </summary>
        internal abstract void EndWarmup();

        /// <summary>
        /// Called by an attack when its last performed phase ends. This frees the Attacker to perform other actions.
        /// </summary>
        /// <returns></returns>
        internal virtual void EndAttack()
        {
            currentAttack = null;
        }

        protected virtual void StartAttack(AttackPhaseDefinition initialPhase, WeaponInfo? weaponInfo = null)
        {
            if (currentAttack is not null)
                throw new InvalidOperationException("Cannot start an attack while Attacker is already in an attack!");

            currentAttack = new(this, initialPhase, weaponInfo);
        }

        /// <summary>
        /// Cancels performance of the current attack. This ends all performed phases, but non-performed phases (such as already fired projectiles) may continue.
        /// </summary>
        protected void CancelAttack()
        {
            currentAttack.CancelAttack();
            EndAttack();
        }

        public virtual bool IsFacingBlocked()
        {
            if (currentAttack is null)
                return false;

            return currentAttack.IsBlockingFacing();
        }

        public virtual bool IsMovementBlocked()
        {
            if (currentAttack is null)
                return false;

            return currentAttack.IsBlockingMovement();
        }

        public virtual bool IsAttacking()
        {
            return currentAttack is not null;
        }

        internal void TriggerAnimation(string triggerName)
        {
            animator.SetTrigger(triggerName);
        }

        public void AnimationMessage(string message)
        {
            //TODO: make parameter do something
            currentAttack?.InvokeTrigger(AttackTransition.TriggerType.AnimationMessage, animationInfo : new() { message = message });
        }

        public void AttackerCollision(Collider2D other)
        {
            currentAttack?.InvokeTrigger(AttackTransition.TriggerType.AttackerContact, collisionInfo: new() { collidingObject = other.gameObject });
        }

        public void AttachTo(GameObject guide)
        {
            transform.SetParent(guide.transform);
            body.bodyType = RigidbodyType2D.Kinematic;
        }

        public void DetachFrom(GameObject guide)
        {
            transform.SetParent(guide.transform.parent);
            body.bodyType = bodyType;
        }
    }
}