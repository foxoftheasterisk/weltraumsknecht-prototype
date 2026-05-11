using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotDisplay : MonoBehaviour
{
    public WeaponSlot slot;
    public Image display;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (slot == null)
        {
            Debug.Log("Weapon Slot Display not linked to a slot! " + this.name);
            Destroy(this);
            return;
        }
        
        if (slot.HasWeapon())
        {
            display.overrideSprite = slot.weapon.definition.icon;
            //If the override sprite is null, will display its default sprite, which is exactly the behavior we want.
            
            display.color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(slot.HasWeapon())
        {
            //TODO: make this triggered instead of a constant replacement
            display.overrideSprite = slot.weapon.definition.icon;

            WeaponInstance weapon = slot.weapon;
            if (!weapon.CanFire())
            {
                if (weapon.IsActive())
                {
                    display.color = Color.yellow;
                }
                else
                {
                    display.color = Color.gray;
                }
            }
            else
            {
                display.color = Color.white;
            }
        }
    }
}
