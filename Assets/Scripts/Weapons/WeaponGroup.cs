using UnityEngine;
using Weltraumsknecht.Weapons;


[CreateAssetMenu(fileName = "WeaponGroup", menuName = "Scriptable Objects/WeaponGroup")]
public class WeaponGroup : ScriptableObject
{

    public WeaponDefinition[] weapons;

    //TODO: some handling to prevent offering a weapon that the player already has
    //That may not be something that should be handled in *this* class though.

    public WeaponDefinition GetRandomWeapon()
    {
        return weapons[Random.Range(0, weapons.Length)];
    }
}
