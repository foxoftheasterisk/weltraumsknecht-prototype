using UnityEngine;
using Platformer.Mechanics;

///Behavior for an object that only exists as a container for other objects.
///Destroys the attached GameObject when it has no children.
[AddComponentMenu("Projectile Behaviours/Container")]
public class ContainerBehaviour : MonoBehaviour
{

    // Update is called once per frame
    public void Update()
    {
        if(transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
