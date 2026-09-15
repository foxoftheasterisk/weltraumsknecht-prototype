using Platformer.Mechanics;
using UnityEngine;

public class ProjectileUtils
{
    /// <summary>
    ///Creates a given projectile (or multiple projectiles in one prefab)
    ///If melee is true, the projectile is created as a child of the parent (and therefore will move with them);
    ///if false, the projectile is created at the parent's location, but not as a child.
    ///Returns the created projectile.
    /// </summary>
    public static GameObject CreateProjectile(GameObject prefab, GameObject parent, bool flip, bool melee = false, ProjectileProperties? properties = null, bool inheritProperties = false)
    {
        GameObject projectile;

        if (melee)
        {
            projectile = GameObject.Instantiate(prefab, parent.transform);
        }
        else
        {
            Vector3 relativePosition = prefab.transform.position;
            if (flip)
                relativePosition.x *= -1;
            Vector3 position = parent.transform.position + relativePosition;
            projectile = GameObject.Instantiate(prefab, position, prefab.transform.rotation);
        }

        if (properties != null)
        {
            if (!projectile.TryGetComponent<Rigidbody2D>(out _))
            {
                Debug.Log("Tried to apply phase properties to projectile with no rigidbody. Continuing without properties.");
            }
            else
            {
                StartWithPropertiesBehaviour behaviour = projectile.AddComponent<StartWithPropertiesBehaviour>();
                behaviour.properties = (ProjectileProperties)properties;
            }
        }

        if (flip)
        {
            Vector3 scale = projectile.transform.localScale;
            scale.x *= -1;
            projectile.transform.localScale = scale;

            Vector3 rotation = projectile.transform.localEulerAngles;
            rotation.z *= -1;
            projectile.transform.localEulerAngles = rotation;

            if (melee)
            {
                Vector3 position = projectile.transform.localPosition;
                position.x *= -1;
                projectile.transform.localPosition = position;
            }

            foreach (FlippableBehaviour flippable in projectile.GetComponents<FlippableBehaviour>())
            {
                flippable.Flip();
            }
        }

        if (inheritProperties)
        {
            projectile.transform.localEulerAngles += parent.transform.localEulerAngles;
            if (projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D projectileRB) && parent.TryGetComponent<Rigidbody2D>(out Rigidbody2D parentRB)
                && projectileRB.bodyType != RigidbodyType2D.Static && parentRB.bodyType != RigidbodyType2D.Static)
            {
                projectileRB.linearVelocity += parentRB.linearVelocity;
                projectileRB.angularVelocity += parentRB.angularVelocity;
            }
        }

        return projectile;
    }
}
