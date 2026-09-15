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
            enemy.Die();
        }
    }
}