using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using Weltraumsknecht.Enemies;
using Weltraumsknecht.Weapons;
using Weltraumsknecht.Player;

using static Platformer.Core.Simulation;

namespace Weltraumsknecht.Projectiles
{

    [AddComponentMenu("Projectiles/Enemy Projectile")]
    public class EnemyProjectile : Projectile
    {
        public int damage = 1;

        protected override void InteractWith(Collider2D other)
        {
            if (other.TryGetComponent<PlayerMovementController>(out PlayerMovementController player))
            {
                ProjectilePlayerCollision ev = Schedule<ProjectilePlayerCollision>();
                ev.projectile = this;
                ev.player = player;
            }
            else
            {
                CollidedWith(other.gameObject);
            }
        }
    }

}
