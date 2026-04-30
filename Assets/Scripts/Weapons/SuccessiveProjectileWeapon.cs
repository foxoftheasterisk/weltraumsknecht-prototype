using UnityEngine;
using Platformer.Mechanics;

///A weapon that performs a chain of projectile-creating actions with successive inputs.
///Each step in the combo replaces the previous step (the previous step's projectiles are destroyed).
///(Currently a bit simplistic.)
[CreateAssetMenu(fileName = "SuccessiveProjectileWeapon", menuName = "Scriptable Objects/SuccessiveProjectileWeapon")]
public class SuccessiveProjectileWeapon : Weapon
{   
    public TimeStep[] steps;
    private GameObject projectile;
    private float timeInStep;
    private int currentStep = -1;
    
    public enum ProjectileLocale
    {    
        Melee, 
        Ranged, 
        Remote
    }

    [System.Serializable]
    public class TimeStep
    {
        public GameObject projectilePrefab;
        public ProjectileLocale locale;
        public float nextStep;
    }
    
    protected override void Fire()
    {
        base.Fire();
        currentStep = 0;
        timeInStep = 0;
        projectile = CreateProjectile(steps[currentStep].projectilePrefab, 
                                      steps[currentStep].locale == ProjectileLocale.Melee);
    }
    
    protected void AdvanceStep()
    {
        GameObject lastStep = projectile;
        
        currentStep++;
        timeInStep = 0;
        if(steps[currentStep].locale == ProjectileLocale.Remote)
            projectile = CreateProjectile(steps[currentStep].projectilePrefab, false, lastStep);
        else
            projectile = CreateProjectile(steps[currentStep].projectilePrefab, 
                                          steps[currentStep].locale == ProjectileLocale.Melee);
                                      
        //Destroy(lastStep);
    }
    
    public override bool IsActive()
    {
        if (projectile == null)
            return false;
        return projectile.activeInHierarchy;
    }
    
    public override void Update()
    {
        base.Update();
        if (IsActive())
        {
            timeInStep += Time.deltaTime;
            if(timeInStep > steps[currentStep].nextStep)
            {
                AdvanceStep();
            }
        }
    }

    public override void ButtonPressed() 
    {
        if (CanFire())
            Fire();
    }

    public override void ButtonReleased() { }
}
