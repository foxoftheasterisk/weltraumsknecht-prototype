using Platformer.Mechanics;
using UnityEngine;

namespace Weltraumsknecht.Weapons
{

    /// <summary>
    /// A simple class that checks if a condition is true.
    /// Basically a predicate that can be used with Unity's editor.
    /// </summary>
    public abstract class TransitionCondition : ScriptableObject
    {
        abstract internal bool CheckCondition(WeaponEvent e);
    }
}
