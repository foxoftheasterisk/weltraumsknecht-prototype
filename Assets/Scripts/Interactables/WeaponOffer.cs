using Platformer.Mechanics;
using UnityEngine;
using Weltraumsknecht.Weapons;

public class WeaponOffer : Interactable
{
    //TODO: find group from static? Not sure if that's a good idea actually.
    public WeaponGroup group;
    public SpriteRenderer preview;

    private WeaponDefinition weapon;


    public override void Initialize()
    {
        if(group == null)
        {
            Debug.Log("Weapon group not found, offer not possible.");
            Destroy(this);
        }
        else
        {
            weapon = group.GetRandomWeapon();
            if (weapon.icon != null)
                preview.sprite = weapon.icon;
        }
    }

    public override void Interact(PlayerController player)
    {
        //TODO: allow choosing which slot to use (including replacing)

        if(!player.westSlot.HasWeapon())
        {
            GiveWeapon(player.westSlot, player);
        }
        else if (!player.northSlot.HasWeapon())
        {
            GiveWeapon(player.northSlot, player);
        }
        else if (!player.eastSlot.HasWeapon())
        {
            GiveWeapon(player.eastSlot, player);
        }

        Destroy(preview.gameObject);
        Destroy(this);
        //Likely at some point we'll want not a clean destroy, but an inactive state - but we'll worry about that later.
    }

    private void GiveWeapon(WeaponSlot slot, PlayerController player)
    {
        WeaponInstance instance = new WeaponInstance(weapon);
        slot.weapon = instance;
        instance.Initialize(player);
        //TODO: this feels kinda janky
    }
}
