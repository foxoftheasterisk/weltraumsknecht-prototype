using Platformer.Gameplay;
using Platformer.Mechanics;
using UnityEngine;
using Weltraumsknecht.Enemies;
using Weltraumsknecht.Weapons;

using static Platformer.Core.Simulation;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        InteractWith(collision.collider);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        InteractWith(other);
    }

    protected void InteractWith(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            ProjectilePlayerCollision ev = Schedule<ProjectilePlayerCollision>();
            ev.projectile = this;
            ev.player = player;
        }
        else
        {
            //CollidedWithOther(other.gameObject);
        }
    }
}
