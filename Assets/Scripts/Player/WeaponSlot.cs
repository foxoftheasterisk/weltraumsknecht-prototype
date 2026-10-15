using UnityEngine;
using Platformer.Mechanics;
using Weltraumsknecht.Weapons;
using UnityEngine.InputSystem;

namespace Weltraumsknecht.Player
{

    ///A very simple container for a Weapon. Exists so that the same weapon slot can be referred to in multiple contexts.
    [CreateAssetMenu(fileName = "WeaponSlot", menuName = "Scriptable Objects/WeaponSlot")]
    public class WeaponSlot : ScriptableObject
    {
        public WeaponInstance weapon;
        PlayerAttackController player;

        internal InputAction button;


        public bool HasWeapon()
        {
            if (weapon == null)
                return false;

            //TODO: remove this kludge
            if (weapon.definition == null)
            {
                weapon = null;
                return false;
            }

            return true;
        }
    }
}
