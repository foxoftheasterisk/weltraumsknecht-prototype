using UnityEngine;
using Platformer.Mechanics;
using Weltraumsknecht.Weapons;

///A very simple container for a Weapon. Exists so that the same weapon slot can be referred to in multiple contexts.
[CreateAssetMenu(fileName = "WeaponSlot", menuName = "Scriptable Objects/WeaponSlot")]
public class WeaponSlot : ScriptableObject
{
    public Weapon weapon;
    PlayerController player;
    //TODO: separate into WeaponData and WeaponInstance
    
    public bool HasWeapon()
    {
        return (weapon != null);
    }
}
