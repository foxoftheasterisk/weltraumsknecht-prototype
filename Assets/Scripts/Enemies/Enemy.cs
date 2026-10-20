using System;
using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using static Platformer.Core.Simulation;
using Weltraumsknecht.Projectiles;
using Weltraumsknecht.Attacks;
using Weltraumsknecht.Player;

namespace Weltraumsknecht.Enemies
{
    /// <summary>
    /// A base class implementing common behavior for enemies
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(MovementAI), typeof(Animator))]
    [AddComponentMenu("Enemies/Enemy")]
    public class Enemy : Attacker
    {
        public AudioClip ouch;

        public float iTimeAfterHit = .1f;
        public float flinchTimeAfterHit = .8f;
        public bool InIFrames
        {
            get;
            private set;
        } = false;

        public bool InFlinch
        {
            get;
            private set;
        } = false;

        public float cooldownAfterHit = 1;

        public float cooldownBetweenAttacks = 5; //This maybe should be per-attack?
        protected bool inCooldown = false;

        internal Animator animator_enemy;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;
        internal Rigidbody2D body;

        internal MovementAI movementAI;
        public EnemyAttack[] attacks;

        private bool wasFacingLeft;

        public bool IsDying
        {
            get;
            private set;
        } = false;

        override protected void Awake()
        {
            base.Awake();

            animator_enemy = GetComponent<Animator>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
            movementAI = GetComponent<MovementAI>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerMovementController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
            }
        }

        void Update()
        {
            if (!InFlinch && !IsDying)
            {
                Act();
            }
        }

        /// <summary>
        /// Act is called every frame that the enemy is capable of normal movement (i.e., when not suffering knockback or dying).
        /// </summary>
        protected void Act()
        {
            Vector2 playerPos = PlayerMovementController.player.transform.position - transform.position;

            if (IsMovementBlocked())
                movementAI.StopMovement();
            else
                movementAI.Move(playerPos);

            if (!IsFacingBlocked())
                FaceTowardsPlayer();

            if (!IsAttacking() && !inCooldown)
            {
                CheckAttacks(playerPos);
            }
        }

        public void FaceTowardsPlayer()
        {
            Vector2 playerPos = PlayerMovementController.player.transform.position;
            if (playerPos.x < transform.position.x && !IsFacingLeft())
            {
                Flip();
            }
            else if (playerPos.x > transform.position.x && IsFacingLeft())
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
            wasFacingLeft = !IsFacingLeft();
        }

        protected void CheckAttacks(Vector2 playerPos)
        {
            List<EnemyAttack> possibleAttacks = new();
            int priority = 0;
            foreach (EnemyAttack attack in attacks)
            {
                if (attack.priority >= priority && attack.IsInRange(playerPos))
                {
                    if (attack.priority > priority)
                    {
                        possibleAttacks = new();
                    }
                    possibleAttacks.Add(attack);
                }
            }

            if (possibleAttacks.Count > 0)
            {
                StartAttack(possibleAttacks[UnityEngine.Random.Range(0, possibleAttacks.Count)].initialPhase);
            }
        }

        public void TookDamageFrom(WeaponProjectile projectile)
        {
            Debug.Log("Enemy took damage");
            InIFrames = true;
            Invoke(nameof(EndIFrames), iTimeAfterHit);

            movementAI.SufferKnockback(projectile.GetKnockback(body.position));
            Flinch();
        }

        /// <summary>
        /// Flinch is called when damage is inflicted on the enemy from a weapon projectile.
        /// It cancels all attacks currently being made and prevents the enemy from starting new attacks until the flinch period ends.
        /// </summary>
        /// <param name="knockback"></param>
        protected void Flinch()
        {
            InFlinch = true;
            animator_enemy.SetBool("inFlinch", true);
            Invoke(nameof(EndFlinch), flinchTimeAfterHit);

            if (currentAttack != null)
            {
                CancelAttack();
            }
        }

        public void EndIFrames()
        {
            InIFrames = false;
        }

        public void EndFlinch()
        {
            InFlinch = false;
            animator_enemy.SetBool("inFlinch", false);
        }

        public void EndCooldown()
        {
            inCooldown = false;
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
            animator_enemy.SetBool("dying", true);
        }

        public void FinishDying()
        {
            Destroy(gameObject);
        }

        public override bool IsFacingLeft()
        {
            return wasFacingLeft;
        }

        internal override void EndWarmup()
        {
            inCooldown = true;
            Invoke(nameof(EndCooldown), cooldownBetweenAttacks);
        }

        public override bool IsFacingBlocked()
        {
            if (!IsAttacking() && movementAI.OnlyFlipsWhenAttacking)
                return true;
            return base.IsFacingBlocked();
        }

        protected override void StartAttack(AttackPhaseDefinition initialPhase, WeaponInfo? weaponInfo = null)
        {
            FaceTowardsPlayer(); //All enemies should face towards player when starting an attack, I think?
            base.StartAttack(initialPhase, weaponInfo);
        }
    }
}