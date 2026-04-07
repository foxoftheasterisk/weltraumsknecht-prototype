using UnityEngine;
using Platformer.Mechanics;

///A weapon that performs a chain of projectile-creating actions with successive inputs.
///Each step in the combo replaces the previous step (the previous step's projectiles are destroyed).
///(Currently a bit simplistic.)
[CreateAssetMenu(fileName = "ComboWeapon", menuName = "Scriptable Objects/ComboWeapon")]
public class ComboWeapon : Weapon
{   
    public ComboStep[] steps;
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
    public class ComboStep
    {
        public GameObject projectilePrefab;
        public ProjectileLocale locale;
        public float nextStepStart;
        public float nextStepEnd;
    }
    
    public override void ButtonPressed()
    {
        if (CanFire())
        {
            Fire();
        }
        else if (IsActive() && 
                 steps.Length > currentStep && 
                 timeInStep > steps[currentStep].nextStepStart && 
                 (timeInStep < steps[currentStep].nextStepEnd || steps[currentStep].nextStepEnd == 0))
        {
            AdvanceStep();
        }
    }
    
    public override void ButtonReleased() {}
    
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
                                      
        Destroy(lastStep);
    }
    
    protected override bool IsActive()
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
        }
    }
}
