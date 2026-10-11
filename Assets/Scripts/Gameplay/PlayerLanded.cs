using Platformer.Core;
using Platformer.Mechanics;

using Weltraumsknecht.Player;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player character lands after being airborne.
    /// </summary>
    /// <typeparam name="PlayerLanded"></typeparam>
    public class PlayerLanded : Simulation.Event<PlayerLanded>
    {
        public PlayerMovementController player;

        public override void Execute()
        {
            player.Land();
        }
    }
}