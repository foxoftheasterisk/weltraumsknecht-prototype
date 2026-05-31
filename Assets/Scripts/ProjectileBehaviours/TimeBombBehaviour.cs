using UnityEngine;

///A simple script which destroys the object and creates an explosion (or other object) at its location
///after a certain amount of time.
[AddComponentMenu("Projectile Behaviours/Time Bomb")]
public class TimeBombBehaviour : MonoBehaviour
{
    public float timeUntilExplode = 10;
    public GameObject explosionPrefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Explode", timeUntilExplode);
    }

    // Update is called once per frame
    void Update() { }
    
    private void Explode()
    {    
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        WeaponProjectile ewp = explosion.GetComponent<WeaponProjectile>();
        if (ewp != null)
        {
            WeaponProjectile wp = GetComponent<WeaponProjectile>();
            //we SHOULDN'T have non-weapon projectiles creating weapon projectiles,
            //but if we do get that situation, we don't want it to cause a crash, so.
            if (wp != null)
            {
                ewp.Create(wp.GetWeapon(), false);
            }
        }
        
        Destroy(gameObject);
    }
    
}
