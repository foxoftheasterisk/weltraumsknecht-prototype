using System;
using UnityEngine;

namespace Weltraumsknecht.Enemies
{

    public abstract class AttackAI : MonoBehaviour
    {
        public bool IsAttacking
        { get; protected set; } = false;

        public int priority = 0;

        public abstract bool IsInRange(Vector2 playerPosition);

        public virtual void StartAttack()
        {
            IsAttacking = true;
        }

        public virtual void CancelAttack()
        {
            IsAttacking = false;
        }

        public abstract void ContinueAttack(); //This one might not be necessary? Could use update instead.
    }
}