using System;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;

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

    public float initialSpeed;

    /// <summary>
    /// Angle at which the projectile should travel.
    /// Defined as degrees from forward, where positive is up and negative is down. Should be in the range -180 to 180.
    /// </summary>
    public float initialAngle;
    public float initialRotateVelocity;
}
