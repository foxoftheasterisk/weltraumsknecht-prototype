using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

using Weltraumsknecht.Enemies;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when a weapon projectile collides with an Enemy.
    /// </summary>
    public class ProjectileEnemyCollision : Simulation.Event<ProjectileEnemyCollision>
    {
        public Enemy enemy;
        public WeaponProjectile projectile;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        //TODO: debounce these collisions
        //(Probably not in this class. But maybe!)

        public override bool Precondition()
        {
            return !enemy.InIFrames;
        }

        public override void Execute()
        {

            var enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.Damage(projectile.GetDamage());
                if (!enemyHealth.IsAlive)
                {
                    Schedule<EnemyDeath>().enemy = enemy;
                    projectile.CollidedWithEnemy(enemy, true);
                }
                else
                {
                    enemy.TookDamageFrom(projectile);
                    projectile.CollidedWithEnemy(enemy, false);
                }
            }
            else
            {
                //there shouldn't be enemies without health, but... a handler anyway.
                Schedule<EnemyDeath>().enemy = enemy;
                projectile.CollidedWithEnemy(enemy, true);
            }
            
        }
    }
}