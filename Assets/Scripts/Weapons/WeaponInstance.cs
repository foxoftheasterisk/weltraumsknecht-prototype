using Platformer.Mechanics;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Weltraumsknecht.Weapons.WeaponDefinition;

namespace Weltraumsknecht.Weapons
{

    //Not sure if this should be a MonoBehaviour actually
    public class WeaponInstance : MonoBehaviour
    {
        public WeaponDefinition Definition
        {
            get;
            private set;
        }

        private List<ActivePhase> activePhases;

        public float CooldownRemaining
        {
            get;
            private set;
        }
        //we will probably eventually want a public ReduceCooldown method

        [HideInInspector]
        public PlayerController player;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            activePhases = new List<ActivePhase>();
        }

        // Update is called once per frame
        void Update()
        {
            if (IsActive())
            {
                List<ActivePhase> inactive = new List<ActivePhase>();
                foreach (ActivePhase phase in activePhases)
                {
                    if (!phase.isActive())
                    {
                        inactive.Add(phase);
                    }
                    else
                    {
                        phase.timeInPhase += Time.deltaTime;
                        phase.checkTransitions();
                    }
                }

            }
            else if (cooldownRemaining > 0 && cooldownType == CooldownType.Time)
                cooldownRemaining -= Time.deltaTime;
        }




        public abstract void ButtonPressed();

        public abstract void ButtonReleased();



        public bool CanFire()
        {
            return !IsActive() && cooldownRemaining <= 0;
        }

        protected virtual void Fire()
        {
            cooldownRemaining = cooldown;
        }

        public bool IsActive()
        {
            foreach (ActivePhase phase in activePhases)
            {
                if (phase.isActive() && phase.definition.activeLink)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///Creates a given projectile (or multiple projectiles in one prefab)
        ///If melee is true, the projectile is created as a child of the parent (and therefore will move with them);
        ///if false, the projectile is created at the parent's location, but not as a child.
        ///If no parent is passed, the player will be used.
        ///(Do not pass in the player, as this will prevent projectile flipping.)
        ///Returns the created projectile.
        /// </summary>
        internal GameObject CreateProjectile(GameObject prefab, bool melee = false, GameObject parent = null)
        {
            GameObject projectile;
            bool flip = false;

            if (parent == null)
            {
                parent = player.gameObject;
                flip = player.IsFacingLeft();
            }

            if (melee)
            {
                projectile = Instantiate(prefab, parent.transform);
            }
            else
            {
                Vector3 relativePosition = prefab.transform.position;
                if (flip)
                    relativePosition.x *= -1;
                Vector3 position = parent.transform.position + relativePosition;
                projectile = Instantiate(prefab, position, Quaternion.identity);
            }

            if (flip)
            {
                Vector3 scale = projectile.transform.localScale;
                scale.x *= -1;
                projectile.transform.localScale = scale;

                Vector3 rotation = projectile.transform.localEulerAngles;
                rotation.z *= -1;
                projectile.transform.localEulerAngles = rotation;

                if (melee)
                {
                    Vector3 position = projectile.transform.localPosition;
                    position.x *= -1;
                    projectile.transform.localPosition = position;
                }
            }

            WeaponProjectile[] wps = projectile.GetComponentsInChildren<WeaponProjectile>();
            foreach (WeaponProjectile wp in wps)
            {
                wp.Create(this, melee);

                //a weaponprojectile requires a rigidbody, so this should be safe
                Rigidbody2D rb = wp.GetComponent<Rigidbody2D>();
                if (rb.bodyType != RigidbodyType2D.Static)
                {
                    if (flip)
                    {
                        rb.linearVelocity = new Vector2(wp.initialVelocity.x * -1, wp.initialVelocity.y);
                        rb.angularVelocity = wp.rotateVelocity * -1;
                    }
                    else
                    {
                        rb.linearVelocity = wp.initialVelocity;
                        rb.angularVelocity = wp.rotateVelocity;
                    }
                }
            }

            return projectile;
        }

        public int GetDamage(bool crit)
        {
            if (crit)
                return critDamage;
            else
                return standardDamage;
        }

        public virtual bool IsBlockingFacing()
        {
            return blocksFacing && IsActive();
        }

        public virtual bool IsBlockingMovement()
        {
            return blocksMovement && IsActive();
        }

        public virtual bool IsBlockingWeapons()
        {
            return blocksWeapons && IsActive();
        }
    }
}
