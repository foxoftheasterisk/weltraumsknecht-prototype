using UnityEngine;
using Weltraumsknecht.Projectiles;

///A simple script that causes the attached object to be destroyed after collisions.
///It can also have a number of non-destroying collisions beforehand.
///...Pretty sure this should no longer be used, but who knows?
[AddComponentMenu("Projectile Behaviours/Break On Contact")]
[RequireComponent(typeof(Projectile))]
public class BreakOnContactBehaviour : MonoBehaviour
{
    public int nonDestroyingCollisions = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Projectile projectile = GetComponent<Projectile>();
        projectile.AddListener(CollidedWith);
    }
    
    public void CollidedWith(GameObject other)
    {
        if(nonDestroyingCollisions > 0)
        {
            nonDestroyingCollisions--;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

}
