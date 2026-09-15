using UnityEngine;

namespace Weltraumsknecht.Enemies
{
    /// <summary>
    /// A basic attack that consists of firing one projectile or several identical projectiles from the enemy's position.
    /// ...Eventually. Right now it just fires one.
    /// </summary>
    public class ProjectileAttackAI : AttackAI
    {

        public GameObject projectile;

        protected override void StartAttack()
        {
            ProjectileUtils.CreateProjectile(projectile, gameObject, enemy.IsFacingLeft);
            EndAttack();
        }

        protected override void ContinueAttack()
        {
            //throw new System.NotImplementedException();
        }
    }
}
