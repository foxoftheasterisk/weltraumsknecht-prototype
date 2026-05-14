using UnityEngine;


/// <summary>
/// A simple listener for contacts with the attached object. Accepts a single delegate.
/// Listens to both collisions and triggers and does not distinguish between them.
/// </summary>
public class ContactListener : MonoBehaviour
{

    public delegate void OnCollision(Collider2D other);
    public OnCollision function;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        function(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        function(other);
    }

    void Start()
    {
        Debug.Log("ContactListener starting");
    }

}
