using UnityEngine;

///A simple script that causes the attached object to expire after a given time.
public class ExpiringBehaviour : MonoBehaviour
{
    public float timeUntilExpire = 10;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, timeUntilExpire);
    }

    // Update is called once per frame
    void Update()
    { }
}
