using Platformer.Mechanics;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// Carries information about events that inform whether TransitionConditions should fire.
    /// Making it a class allows us to add more information to certain events while still having the same signature.
    /// </summary>
    public class WeaponEvent
    {
        internal ActivePhase CurrentPhase
        { get; private set; }
        internal PlayerController Player
        { get; private set; }

        internal WeaponEvent(ActivePhase currentPhase, PlayerController player)
        {
            this.CurrentPhase = currentPhase;
            this.Player = player;
        }
    }

}
