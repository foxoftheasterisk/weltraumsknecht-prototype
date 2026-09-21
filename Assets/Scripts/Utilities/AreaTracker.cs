using NUnit.Framework;
using Platformer.Mechanics;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static ProjectileUtils;

/// <summary>
/// Tracks all GameObjects of a specific type in the area defined by this object's Collider2D.
/// Only works if the collider is set to Trigger.
/// 
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class AreaTracker : MonoBehaviour
{
    public TargetType type;
    private HashSet<GameObject> objectSet;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (objectSet == null)
            Initialize();
    }

    private void Initialize()
    {
        objectSet = new();
        //Collider2D collider = GetComponent<Collider2D>();

        //Get initial collisions, somehow?
        //collider.GetContacts probably, but i don't understand how to use that.
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (objectSet == null)
            Debug.Log("AreaTracker reached OnTriggerEnter2D before initializing!");

        GameObject other = collision.gameObject;
        if (type.Matches(other))
            objectSet.Add(other);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectSet.Remove(collision.gameObject);
    }

    public IList<GameObject> GetObjectsInArea()
    {
        if (objectSet == null)
            Initialize();

        return objectSet.AsReadOnlyList();
    }
}
