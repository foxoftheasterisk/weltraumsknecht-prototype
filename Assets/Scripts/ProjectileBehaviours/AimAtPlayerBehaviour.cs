using Platformer.Mechanics;
using System.Linq;
using UnityEngine;

/// <summary>
/// A behaviour that redirects any StartWithPropertiesBehaviour(s) on the object such that 0 degrees is aimed at the player,
/// then destroys itself.
/// Has no effect if the attached object has no StartWithPropertiesBehaviour.
/// Assumes that the starting properties are already flipped toward the player.
/// </summary>
[AddComponentMenu("Projectile Behaviours/Aim At Player")]
public class AimAtPlayerBehaviour : StartingPropertiesAlterer
{

    void OnEnable()
    {
        Debug.Log("Aim starting");

        Vector2 pos = transform.position;
        Vector2 playerPos = PlayerController.player.transform.position;

        Vector2 target = playerPos - pos;
        if (target.x < 0)
            target.x *= -1;

        
        float angle = Vector2.SignedAngle(Vector2.right, target);
        Alterations = new()
        {
            displace = Vector2.zero,
            rotateMod = angle,
            initialSpeed = 0,
            initialAngle = angle,
            initialRotateVelocity = 0
        };
    }


}
