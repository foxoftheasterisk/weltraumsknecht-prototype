using Platformer.Mechanics;
using UnityEngine;
using Weltraumsknecht.Enemies;

public abstract class ImmobileEnemy : Enemy
{
    void Awake()
    {
        knockbackScale = 0;
    }

    protected override void Move()
    {
        //Do nothing, since the enemy is immobile.
        //(Idle animations?)
    }
}
