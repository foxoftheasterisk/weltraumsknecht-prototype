using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static Platformer.Core.Simulation;

namespace Weltraumsknecht.Enemies
{
    /// <summary>
    /// A base class implementing common behavior for enemies
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D), typeof(Rigidbody2D))]
    public abstract class Enemy : MonoBehaviour
    {
        public AudioClip ouch;
        
        public float iTimeAfterHit = .2f;
        private bool inIFrames = false;
        private bool inKnockback = false;
        public float knockbackScale = 1;

        public float cooldownBetweenAttacks = 5;
        protected bool isAttacking = false;
        protected bool inCooldown = false;

        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;
        internal Rigidbody2D body;

        public Bounds Bounds => _collider.bounds;

        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if (!inKnockback)
            {
                if (isAttacking)
                    ContinueAttack();
                else
                    Act();
            }
        }
        
        /// <summary>
        /// ContinueAttack is called every frame when isAttacking is set to true.
        /// It should advance the attack animation.
        /// Note: Warmup is included.
        /// </summary>
        protected abstract void ContinueAttack();

        /// <summary>
        /// Act is called every frame that the enemy is capable of normal movement (i.e., when not suffering knockback or in the middle of an attack).
        /// </summary>
        protected void Act()
        {
            Move();

            if (!inCooldown && IsInRange(PlayerController.player.transform.position))
            {
                StartAttack();
            }
        }

        protected abstract Boolean IsInRange(Vector2 playerPosition);

        protected abstract void Move();
        protected abstract void StartAttack();
        
        
        public void TookDamageFrom(WeaponProjectile projectile)
        {
            Debug.Log("Enemy took damage");
            inIFrames = true;
            Invoke("EndIFrames", iTimeAfterHit);

            SufferKnockback(projectile.GetKnockback(body.position));
        }

        /// <summary>
        /// SufferKnockback is called when knockback is inflicted on the enemy from a weapon projectile.
        /// The default implementation sets the enemy's velocity to the knockback vector given times the knockback scale.
        /// It also cancels all attacks currently being made.
        /// </summary>
        /// <param name="knockback"></param>
        protected virtual void SufferKnockback(Vector2 knockback)
        {
            body.linearVelocity = knockback * knockbackScale;
            inKnockback = true;
            Invoke("EndKnockback", iTimeAfterHit);

            if (isAttacking)
            {
                isAttacking = false;
                inCooldown = true;

            }
        }

        public void EndIFrames()
        {
            inIFrames = false;
        }

        public void EndKnockback()
        {
            inKnockback = false;
        }
        
        public void EndCooldown()
        {
            inCooldown = false;
        }

        public bool IsInIFrames()
        {
            return inIFrames;
        }

    }
}