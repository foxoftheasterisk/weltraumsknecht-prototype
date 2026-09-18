using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using Weltraumsknecht.Enemies;
using Weltraumsknecht.Weapons;

using static Platformer.Core.Simulation;

namespace Weltraumsknecht.Projectiles
{

    public class EnemyProjectile : Projectile
    {
        public int damage = 1;

        protected override void InteractWith(Collider2D other)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player))
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
