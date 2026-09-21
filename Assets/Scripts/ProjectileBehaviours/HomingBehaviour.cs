using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProjectileUtils;

[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Projectile Behaviours/Homing")]
public class HomingBehaviour : MonoBehaviour
{
    public float targetSpeed = 7; //The intended speed for the attached projectile
    public float acceleration = 3; //How quickly to change the attached projectile's actual speed to its intended, in units per second.
    public float rotateMax = 360; //Angle in degrees that this can turn in one second

    public TargetType targetType;
    public TargetSwitchMode targetSwitchMode;
    public AreaTracker targetFindArea;
    public float targetKeepRange = float.PositiveInfinity;

    public enum TargetSwitchMode
    {
        Never, IfLost, Anytime
        //"Anytime" is probably horrible performance-wise
    }

    private Rigidbody2D target;

    private Rigidbody2D projectile;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        projectile = GetComponent<Rigidbody2D>();
        if (projectile == null)
        {
            Debug.Log("HomingBehaviour detected no Rigidbody2D!");
            Destroy(this);
        }

        FindTarget();
    }

    // Update is called once per frame
    void Update()
    {
        //Lose target if out of range
        if(target != null && Vector2.Distance(projectile.position, target.position) > targetKeepRange)
        {
            target = null;
        }

        //Find new target if applicable
        if (targetSwitchMode == TargetSwitchMode.Anytime || (target == null && targetSwitchMode == TargetSwitchMode.IfLost))
            FindTarget();

        //If no target, does nothing.
        if (target == null)
            return;

        float deltaTime = Time.deltaTime;

        //First manage speed
        Vector2 vel = projectile.linearVelocity;
        float speed = vel.magnitude;
        float deltaSpeed = acceleration * deltaTime; //not quite standard usage of delta but w/e
        if (speed < deltaSpeed)
        {
            //this is not the most elegant way to do this, but eh, it works
            Vector2 targetPos = target.position;
            Vector2 direction = (targetPos - projectile.position).normalized;

            Vector2 deltaV = direction * deltaSpeed;
            vel = Vector2.ClampMagnitude(vel, targetSpeed - deltaSpeed);
            vel = vel + deltaV;

            projectile.linearVelocity = vel;
            return;
        }
        else if (speed != targetSpeed)
        {
            if (speed > targetSpeed)
            {
                speed = Mathf.Max(speed - deltaSpeed, targetSpeed);
            } 
            else
            {
                speed = Mathf.Min(speed + deltaSpeed, targetSpeed);
            }

            vel = vel.normalized * speed;
        }

        //then manage rotation
        {
            Vector2 targetPos = target.position;
            Vector2 direction = (targetPos - projectile.position).normalized;
            float angle = Vector2.SignedAngle(vel, direction);

            float deltaRotate = deltaTime * rotateMax;

            if (Mathf.Abs(angle) < deltaRotate)
            {
                vel = direction * vel.magnitude;
            }
            else
            {
                if (angle < 0)
                {
                    deltaRotate = -deltaRotate;
                }

                float radianDeltaRotate = Mathf.Deg2Rad * deltaRotate;

                //now the tricky part
                float newX = vel.x * Mathf.Cos(radianDeltaRotate) - vel.y * Mathf.Sin(radianDeltaRotate);
                float newY = vel.x * Mathf.Sin(radianDeltaRotate) + vel.y * Mathf.Cos(radianDeltaRotate);

                vel = new Vector2(newX, newY);
            }
        }

        projectile.linearVelocity = vel;
    }

    private void FindTarget()
    {
        if(targetFindArea != null)
        {
            TargetClosestObject(targetFindArea.GetObjectsInArea());
            //all should be of an appropriate type
        }
        else
        {
            switch (targetType)
            {
                case TargetType.Player:
                    target = PlayerController.player.GetComponent<Rigidbody2D>(); //no handling for null, which could be a problem but ehh.
                    break;
                default:
                    throw new NotImplementedException("HomingBehaviour.FindTarget has no handling for " + targetType.ToString() + " without a bounding area!");
            }
        }
    }

    private void TargetClosestObject(IList<GameObject> potentialTargets)
    {
        Rigidbody2D closest = null;
        float closestDistance = float.PositiveInfinity;
        foreach (GameObject potentialTarget in potentialTargets)
        {
            if (potentialTarget.TryGetComponent(out Rigidbody2D body))
            {
                if (closest == null || Vector2.Distance(projectile.position, body.position) < closestDistance)
                {
                    closest = body;
                    closestDistance = Vector2.Distance(body.position, projectile.position);
                }
            }
        }

        if (closestDistance < targetKeepRange)
            target = closest;
    }

    public void AssignTarget(Rigidbody2D target)
    {
        this.target = target;
    }
}
