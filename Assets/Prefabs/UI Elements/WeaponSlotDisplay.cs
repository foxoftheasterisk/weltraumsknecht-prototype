using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotDisplay : MonoBehaviour
{
    public WeaponSlot slot;
    public Image display;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (slot != null && slot.HasWeapon())
        {
            display.overrideSprite = slot.weapon.icon;
            //If the override sprite is null, will display its default sprite, which is exactly the behavior we want.
            
            display.color = Color.white;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
