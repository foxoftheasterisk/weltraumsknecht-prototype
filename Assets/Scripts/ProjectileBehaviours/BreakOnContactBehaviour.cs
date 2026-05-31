using UnityEngine;

///A simple script that causes the attached object to be destroyed after collisions.
///It can also have a number of non-destroying collisions beforehand.
///(Because Destroy occurs at the end of the frame, other collision effects still apply.)
///Currently only works with collisions, so does nothing on projectiles set to trigger.
///(TODO: fix that.)
[AddComponentMenu("Projectile Behaviours/Break On Contact")]
public class BreakOnContactBehaviour : MonoBehaviour
{
    public int nonDestroyingCollisions = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update(){ }
    
    public void OnCollisionEnter2D(Collision2D collision)
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
    
    /*
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile breaking from trigger with " + other.gameObject.name);
        Destroy(gameObject);
    }
    //*/
}
