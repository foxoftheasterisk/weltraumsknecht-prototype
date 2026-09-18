using NUnit.Framework;
using Platformer.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;
using Weltraumsknecht.Enemies;

namespace Weltraumsknecht.Projectiles
{

    /// <summary>
    /// Base class for both WeaponProjectiles and EnemyProjectiles.
    /// </summary>
    
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        private List<Action<GameObject>> listeners = new();

        public void OnCollisionEnter2D(Collision2D collision)
        {
            InteractWith(collision.collider);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            InteractWith(other);
        }

        protected virtual void InteractWith(Collider2D other)
        {
            CollidedWith(other.gameObject);
        }

        public void AddListener(Action<GameObject> listener)
        {
            listeners.Add(listener);
        }

        public void RemoveListener(Action<GameObject> listener)
        {
            listeners.Remove(listener);
        }

        /// <summary>
        /// CollidedWith is called after a collision.
        /// Handling that requires properties of the projectile (e.g. damage) should be done first,
        /// as a common usage of CollidedWith is to destroy the projectile.
        /// </summary>
        /// <param name="other">The GameObject that was collided with.</param>
        public virtual void CollidedWith(GameObject other)
        {
            foreach (Action<GameObject> listener in listeners)
            {
                listener.Invoke(other);
            }
        }
    }
}
