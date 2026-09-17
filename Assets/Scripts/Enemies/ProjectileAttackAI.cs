using UnityEngine;
using UnityEngine.UIElements;

namespace Weltraumsknecht.Enemies
{
    /// <summary>
    /// A basic attack that consists of firing one projectile or several identical projectiles from the enemy's position.
    /// </summary>
    public class ProjectileAttackAI : AttackAI
    {

        public GameObject projectile;

        /// <summary>
        /// An array of the initial properties for the projectiles in this attack.
        /// Also determines the number of projectiles.
        /// </summary>
        public ProjectileProperties[] projectileProperties;
        private int projectilesFired;

        public bool melee = false;

        public bool AnimationControlled = true;

        protected override void StartAttack()
        {
            projectilesFired = 0;

            FireNextProjectile();
        }

        protected override void ContinueAttack()
        {
            if(!AnimationControlled)
            {
                throw new System.NotImplementedException("Manually controlled ProjectileAttackAI is not implemented.");
            }
        }

        public void FireNextProjectile()
        {
            if (!InAttack)
                return;
            //This invoke was meant for a *different* attack.

            ProjectileUtils.CreateProjectile(projectile, gameObject, enemy.IsFacingLeft, melee, projectileProperties[projectilesFired]);
            projectilesFired++;
            if (projectilesFired == projectileProperties.Length)
            {
                EndAttack();
            }
        }
    }
}
