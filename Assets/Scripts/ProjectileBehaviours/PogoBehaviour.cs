using UnityEngine;
using Platformer.Mechanics;

///A script that applies a bounce effect to the parent gameObject's KinematicObject when the attached projectile
/// collides with any other object.
/// Will not operate correctly if its transform does not have a parent object 
/// or if that object is not a KinematicObject
/// After causing this effect once, the script destroys itself.
/// (Unknown behavior if it collides with two objects in the same frame.)
///Currently only works with collisions, so does nothing on projectiles set to trigger.
///(TODO: fix that.)
[AddComponentMenu("Projectile Behaviours/Pogo")]
public class PogoBehaviour : MonoBehaviour
{
    private KinematicObject objectToBounce;
    public float bounceMagnitude = 4;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        KinematicObject parentKO;
        Transform current = transform;
        do {
            if (current.parent == null)
            {
                Debug.Log("No parent object; destroying PogoBehavior");
                Destroy(this);
                return;
            }

            parentKO = current.parent.GetComponent<KinematicObject>();
            current = current.parent;
        }
        while (parentKO == null);
        
        Debug.Log("Parent identified: " + parentKO.name);
        objectToBounce = parentKO;
    }

    // Update is called once per frame
    void Update(){ }
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        objectToBounce.Bounce(bounceMagnitude);
        Debug.Log("Applying bounce to object: " + objectToBounce.name);
        Destroy(this);
    }
    
    /*
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile breaking from trigger with " + other.gameObject.name);
        Destroy(gameObject);
    }
    //*/
}
