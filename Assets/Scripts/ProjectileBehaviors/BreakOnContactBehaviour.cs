using UnityEngine;

///A simple script that causes the attached object to be destroyed after any collision.
///(Because Destroy occurs at the end of the frame, other collision effects still apply.)
///Currently only works with collisions, so does nothing on projectiles set to trigger.
///(TODO: fix that.)
public class BreakOnContactBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() { }

    // Update is called once per frame
    void Update(){ }
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Projectile breaking from contact with " + collision.gameObject.name);
        Destroy(gameObject);
    }
    
    /*
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Projectile breaking from trigger with " + other.gameObject.name);
        Destroy(gameObject);
    }
    //*/
}
