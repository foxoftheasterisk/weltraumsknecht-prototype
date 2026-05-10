using Platformer.Mechanics;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    /// <summary>
    /// A method that performs any initial steps the Interactable needs (such as randomly choosing an offer)
    /// Currently occurs on Start, but this may not always be the case.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Performed when the player pushes the Interact button while in the Interactable's zone.
    /// </summary>
    /// <param name="player"></param>
    public abstract void Interact(PlayerController player);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
