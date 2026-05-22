using System;
using UnityEngine;

/// <summary>
/// A definition of the starting properties of a projectile.
/// </summary>
[Serializable]
public struct ProjectileProperties
{

    /// <summary>
    /// Amount of displacement from the prefab's original location (relative to the parent object).
    /// </summary>
    public Vector2 displace;
    /// <summary>
    /// Amount of rotation (Z) from the prefab's orientation (relative to the parent).
    /// </summary>
    public float rotateMod;

    public Vector2 initialVelocity;
    public float initialRotateVelocity;

    public void Flip()
    {
        displace.x *= -1;
        rotateMod *= -1;
        initialVelocity.x *= -1;
        initialRotateVelocity *= -1;
    }
}
