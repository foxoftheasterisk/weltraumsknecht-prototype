using UnityEngine;
using Platformer.Mechanics;

///A simple weapon that creates a single projectile then waits for it to be destroyed before starting its cooldown.
///Supports different projectiles for air and ground; however, if airPrefab is null, will always use projectilePrefab.
///Setting only airPrefab allows creating a weapon that works only in the air.
/// (However, at the moment this still resets the cooldown on the ground. TODO: fix.)
[CreateAssetMenu(fileName = "SingleProjectileWeapon", menuName = "Scriptable Objects/SingleProjectileWeapon")]
public class SingleProjectileWeapon : Weapon
{
    public GameObject projectilePrefab;
    public GameObject airPrefab = null;
    private GameObject projectile;
    public bool melee = false;
    
    
    public override void ButtonPressed()
    {
        if (CanFire())
            Fire();
    }
    
    public override void ButtonReleased() {}
    
    protected override void Fire()
    {
        base.Fire();
        if(player.IsAirborne() && airPrefab != null)
        {
            projectile = CreateProjectile(airPrefab, melee);
        }
        else if (projectilePrefab != null)
        {
            projectile = CreateProjectile(projectilePrefab, melee);
        }
    }
    
    protected override bool IsActive()
    {
        if (projectile == null)
            return false;
        return projectile.activeInHierarchy;
    }
}
