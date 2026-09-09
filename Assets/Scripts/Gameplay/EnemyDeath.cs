using Platformer.Core;
using Platformer.Mechanics;
using UnityEngine;
using Weltraumsknecht.Enemies;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of  0.
    /// </summary>
    /// <typeparam name="EnemyDeath"></typeparam>
    public class EnemyDeath : Simulation.Event<EnemyDeath>
    {
        public Enemy enemy;

        public override void Execute()
        {
            
            if (enemy.TryGetComponent<Collider2D>(out Collider2D collider))
            {
                collider.enabled = false;
            }
            enemy.control.enabled = false;
            if (enemy._audio && enemy.ouch)
                enemy._audio.PlayOneShot(enemy.ouch);
        }
    }
}