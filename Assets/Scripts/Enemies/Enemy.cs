using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using static Platformer.Core.Simulation;

namespace Weltraumsknecht.Enemies
{
    /// <summary>
    /// A base class implementing common behavior for enemies
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(MovementAI), typeof(Animator))]
    //[RequireComponent(typeof(AttackAI))]
    public class Enemy : MonoBehaviour
    {
        public AudioClip ouch;

        public float iTimeAfterHit = .2f;
        public bool InIFrames
        {
            get;
            private set;
        } = false;
        public bool InKnockback
        {
            get;
            private set;
        } = false;
        public float knockbackScale = 1;

        public float cooldownAfterHit = 1;

        public float cooldownBetweenAttacks = 5; //This maybe should be per-attack?
        private AttackAI currentAttack = null;
        protected bool inCooldown = false;

        internal Animator animator;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;
        internal Rigidbody2D body;

        internal MovementAI movementAI;
        internal AttackAI[] attacks;

        public bool IsFacingLeft
        {
            get;
            private set;
        } = false;

        public bool IsDying
        {
            get;
            private set;
        } = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
            movementAI = GetComponent<MovementAI>();
            attacks = GetComponents<AttackAI>();
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
            if (!InKnockback && !IsDying)
            {
                if (currentAttack != null)
                {
                    currentAttack.Continue();
                    if (!currentAttack.IsActive)
                    {
                        currentAttack = null;
                        inCooldown = true;
                        Invoke(nameof(EndCooldown), cooldownBetweenAttacks);
                    }
                }
                else
                    Act();
            }
        }

        /// <summary>
        /// Act is called every frame that the enemy is capable of normal movement (i.e., when not suffering knockback, dying, or in the middle of an attack).
        /// </summary>
        protected void Act()
        {
            movementAI.Move();

            if (!inCooldown)
            {
                CheckAttacks();
            }
        }

        public void FaceTowardsPlayer()
        {
            Vector2 playerPos = PlayerController.player.transform.position;
            if (playerPos.x < transform.position.x && !IsFacingLeft)
            {
                Flip();
            }
            else if (playerPos.x > transform.position.x && IsFacingLeft)
            {
                Flip();
            }
        }

        public void Flip()
        {
            //Probably not the best way to do this, but it works for now.
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            IsFacingLeft = !IsFacingLeft;
        }

        protected void CheckAttacks()
        {
            Vector2 playerPos = PlayerController.player.transform.position - transform.position;

            List<AttackAI> possibleAttacks = new List<AttackAI>();
            int priority = 0;
            foreach (AttackAI attack in attacks)
            {
                if (attack.priority <= priority && attack.IsInRange(playerPos))
                {
                    if (attack.priority > priority)
                    {
                        possibleAttacks = new List<AttackAI>();
                    }
                    possibleAttacks.Add(attack);
                }
            }

            if (possibleAttacks.Count > 0)
            {
                StartAttack(possibleAttacks[UnityEngine.Random.Range(0, possibleAttacks.Count)]);
            }
        }

        protected void StartAttack(AttackAI attack)
        {
            currentAttack = attack;
            attack.StartWarmup();
        }

        private void CancelAttack()
        {
            inCooldown = true;
            if (currentAttack.InAttack)
                Invoke(nameof(EndCooldown), cooldownBetweenAttacks);
            else
                Invoke(nameof(EndCooldown), cooldownAfterHit);

            currentAttack.CancelAttack();
            currentAttack = null;
        }

        public void TookDamageFrom(WeaponProjectile projectile)
        {
            Debug.Log("Enemy took damage");
            InIFrames = true;
            Invoke(nameof(EndIFrames), iTimeAfterHit);

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

            InKnockback = true;
            animator.SetBool("inFlinch", true);
            Invoke(nameof(EndKnockback), iTimeAfterHit);

            if (currentAttack != null)
            {
                CancelAttack();
            }
        }

        public void EndIFrames()
        {
            InIFrames = false;
        }

        public void EndKnockback()
        {
            InKnockback = false;
            animator.SetBool("inFlinch", false);
        }

        public void EndCooldown()
        {
            inCooldown = false;
        }

        public bool IsAttacking()
        {
            return currentAttack != null;
        }

        /// <summary>
        /// Disables all offense of this enemy and starts the dying animation.
        /// </summary>
        public void Die()
        {
            IsDying = true;
            if (TryGetComponent<Collider2D>(out Collider2D collider))
            {
                collider.enabled = false;
            }
            if (currentAttack != null)
                currentAttack.CancelAttack();


            _audio.PlayOneShot(ouch);
            animator.SetTrigger("die");
        }

        public void FinishDying()
        {
            Destroy(gameObject);
        }
    }
}