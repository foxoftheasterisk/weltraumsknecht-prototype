using UnityEngine;
using Platformer.Mechanics;

[CreateAssetMenu(fileName = "ChargingWeapon", menuName = "Scriptable Objects/ChargingWeapon")]
public class ChargingWeapon : Weapon
{
    public GameObject unchargedPrefab;
    public GameObject chargedPrefab;
    
    public float chargeTime = 5;
    private bool charging = false;
    private float charge;
    
    public float chargedCooldownModifier;
    
    public override void ButtonPressed()
    {
        if (CanFire() && !charging)
        {
            charging = true;
            charge = 0;
        }
    }
    
    public override void ButtonReleased()
    {
        if (charging)
        {
            charging = false;
            Fire();
        }
    }
    
    protected override void Fire()
    {
        base.Fire();
        if (charge >= chargeTime)
        {
            cooldownRemaining += chargedCooldownModifier;
            CreateProjectile(chargedPrefab);
        }
        else
        {
            CreateProjectile(unchargedPrefab);
        }
    }
    
    public override bool IsActive()
    {
        return charging;
    }
    
    public override void Update()
    {
        base.Update();
        if (charging)
        {
            charge += Time.deltaTime;
        }
    }
}
