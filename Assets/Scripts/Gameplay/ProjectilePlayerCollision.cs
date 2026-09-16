using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

using Weltraumsknecht.Enemies;

namespace Platformer.Gameplay
{

    /// <summary>
    /// Fired when an enemy projectile collides with the Player.
    /// </summary>
    public class ProjectilePlayerCollision : Simulation.Event<ProjectileEnemyCollision>
    {
        public PlayerController player;
        public EnemyProjectile projectile;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {

            var playerHealth = player.GetComponent<Health>();
            
            //Until the player actually gets a Health stat, all projectiles instakill.
            Schedule<PlayerDeath>();
            /*
            if (playerHealth != null)
            {
                playerHealth.Damage(projectile.damage);
                //TODO: allow armor to exist?

                if (!playerHealth.IsAlive)
                {
                    Schedule<PlayerDeath>();
                }
                else
                {
                    //player.TookDamageFrom(projectile);
                }
            }
            else
            {
                
            }
            */
        }
    }
}