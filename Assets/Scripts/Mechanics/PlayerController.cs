using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;
using UnityEngine.InputSystem;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 7;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 7;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        /*internal new*/ public Collider2D collider2d;
        /*internal new*/ public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;
        
        private bool dashExpended = false;
        public float dashSpeed = 7;
        public float maxDashDistance = 1000;
        public float minDashDistance = 50;
        private bool stopDash;
        private float dashDistance = 0;

        bool jump;
        Vector2 move; //requested move left/right. May not match actual movement.
        private bool wasFacingLeft = false;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        private InputAction m_MoveAction;
        private InputAction m_JumpAction;
        private InputAction m_DashLeftAction;
        private InputAction m_DashRightAction;
        
        private InputAction m_WestWeaponAction;
        private InputAction m_NorthWeaponAction;
        private InputAction m_EastWeaponAction;
        private InputAction m_KickAction;

        public Bounds Bounds => collider2d.bounds;
        
        public WeaponSlot westSlot;
        public WeaponSlot northSlot;
        public WeaponSlot eastSlot;
        public WeaponSlot kickSlot;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            if (westSlot.HasWeapon())
                westSlot.weapon.player = this;
            if (northSlot.HasWeapon())
                northSlot.weapon.player = this;
            if (eastSlot.HasWeapon())
                eastSlot.weapon.player = this;
            if (kickSlot.HasWeapon())
                kickSlot.weapon.player = this;

            m_MoveAction = InputSystem.actions.FindAction("Player/Move");
            m_JumpAction = InputSystem.actions.FindAction("Player/Jump");
            m_DashLeftAction = InputSystem.actions.FindAction("Player/Dash Left");
            m_DashRightAction = InputSystem.actions.FindAction("Player/Dash Right");
            
            m_MoveAction.Enable();
            m_JumpAction.Enable();
            m_DashLeftAction.Enable();
            m_DashRightAction.Enable();
            
            m_WestWeaponAction = InputSystem.actions.FindAction("Player/Weapon1");
            m_NorthWeaponAction = InputSystem.actions.FindAction("Player/Weapon2");
            m_EastWeaponAction = InputSystem.actions.FindAction("Player/Weapon3");
            m_KickAction = InputSystem.actions.FindAction("Player/Kick");
            
            m_WestWeaponAction.Enable();
            m_NorthWeaponAction.Enable();
            m_EastWeaponAction.Enable();
            m_KickAction.Enable();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {   
                move.x = m_MoveAction.ReadValue<Vector2>().x;
                if (!dashExpended && (m_DashLeftAction.WasPressedThisFrame() 
                        || m_DashRightAction.WasPressedThisFrame()))
                    StartDash();
                else if ((jumpState == JumpState.DashingLeft && m_DashLeftAction.WasReleasedThisFrame())
                        || (jumpState == JumpState.DashingRight && m_DashRightAction.WasReleasedThisFrame()))
                {
                    stopDash = true;
                    jumpState = JumpState.InFlight;
                }
                else if (jumpState == JumpState.Grounded && m_JumpAction.WasPressedThisFrame())
                    jumpState = JumpState.PrepareToJump;
                else if (m_JumpAction.WasReleasedThisFrame())
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
                
                bool weaponsBlocked = AreWeaponsBlocked();
                
                //still need to feed through input for the weapon that's blocking other weapons
                if (westSlot.HasWeapon() && (!weaponsBlocked || westSlot.weapon.IsBlockingWeapons()))
                {
                    if (m_WestWeaponAction.WasPressedThisFrame())
                        westSlot.weapon.ButtonPressed();
                    if (m_WestWeaponAction.WasReleasedThisFrame())
                        westSlot.weapon.ButtonReleased();
                }
                
                if (northSlot.HasWeapon() && (!weaponsBlocked || northSlot.weapon.IsBlockingWeapons()))
                {
                    if (m_NorthWeaponAction.WasPressedThisFrame())
                        northSlot.weapon.ButtonPressed();
                    if (m_NorthWeaponAction.WasReleasedThisFrame())
                        northSlot.weapon.ButtonReleased();
                }
                
                if (eastSlot.HasWeapon() && (!weaponsBlocked || eastSlot.weapon.IsBlockingWeapons()))
                {
                    if (m_EastWeaponAction.WasPressedThisFrame())
                        eastSlot.weapon.ButtonPressed();
                    if (m_EastWeaponAction.WasReleasedThisFrame())
                        eastSlot.weapon.ButtonReleased();
                }
                
                if (kickSlot.HasWeapon() && (!weaponsBlocked || kickSlot.weapon.IsBlockingWeapons()))
                {
                    if (m_KickAction.WasPressedThisFrame())
                        kickSlot.weapon.ButtonPressed();
                    if (m_KickAction.WasReleasedThisFrame())
                        kickSlot.weapon.ButtonReleased();
                }
                //this block seems awkward, but I'm not really sure how else to do it...
            }
            else
            {
                move.x = 0;
            }
            
            if(IsDashing() || IsMovementBlocked())
            {
                gravityModifier = 0;
                body.gravityScale = 0;
                //how many fucking ways to turn off gravity are there?? and also which one actually works???
            }
            else
            {
                gravityModifier = 1; //I assume...
                body.gravityScale = 1;
            }
            
            wasFacingLeft = IsFacingLeft();
            spriteRenderer.flipX = wasFacingLeft;
            UpdateWeapons();
            UpdateJumpState();
            base.Update();
        }
        
        void StartDash()
        {
            if (IsMovementBlocked())
            {
                bool cancelled = AttemptDashCancel();
                if (!cancelled)
                    return;
            }
            
            dashExpended = true;
            dashDistance = 0;
            if(m_DashLeftAction.WasPressedThisFrame())
                jumpState = JumpState.DashingLeft;
            else
                jumpState = JumpState.DashingRight;
        }

        private void UpdateWeapons()
        {
            if (westSlot.HasWeapon())
                westSlot.weapon.Update();
            if (northSlot.HasWeapon())
                northSlot.weapon.Update();
            if (eastSlot.HasWeapon())
                eastSlot.weapon.Update();
            if (kickSlot.HasWeapon())
                kickSlot.weapon.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
                case JumpState.DashingLeft:
                case JumpState.DashingRight:
                    dashDistance += dashSpeed;
                    if (dashDistance > maxDashDistance)
                    {
                        jumpState = JumpState.InFlight;
                        stopDash = true;
                    }
                    break;
            }
        }

        public bool IsFacingLeft()
        {
            if (IsFacingBlocked())
                return wasFacingLeft;
            else if (move.x > 0.01f)
                return false;
            else if (move.x < -0.01f)
                return true;
            else
                return wasFacingLeft;
        }

        public bool IsDashing()
        {
            return jumpState == JumpState.DashingLeft || jumpState == JumpState.DashingRight;
        }

        public bool IsAirborne()
        {
            return !IsGrounded;
        }
        
        private bool IsFacingBlocked()
        {
            return IsDashing() || (westSlot.HasWeapon() && westSlot.weapon.IsBlockingFacing()) 
                    || (northSlot.HasWeapon() && northSlot.weapon.IsBlockingFacing()) 
                    || (eastSlot.HasWeapon() && eastSlot.weapon.IsBlockingFacing()) 
                    || (kickSlot.HasWeapon() && kickSlot.weapon.IsBlockingFacing());
        }
        
        private bool IsMovementBlocked()
        {
            return (westSlot.HasWeapon() && westSlot.weapon.IsBlockingMovement()) 
                    || (northSlot.HasWeapon() && northSlot.weapon.IsBlockingMovement()) 
                    || (eastSlot.HasWeapon() && eastSlot.weapon.IsBlockingMovement()) 
                    || (kickSlot.HasWeapon() && kickSlot.weapon.IsBlockingMovement());
        }
        
        private bool AreWeaponsBlocked()
        {
            return (westSlot.HasWeapon() && westSlot.weapon.IsBlockingWeapons()) 
                    || (northSlot.HasWeapon() && northSlot.weapon.IsBlockingWeapons()) 
                    || (eastSlot.HasWeapon() && eastSlot.weapon.IsBlockingWeapons()) 
                    || (kickSlot.HasWeapon() && kickSlot.weapon.IsBlockingWeapons());
        }

        private bool AttemptDashCancel()
        {
            //TODO: this
            return false;
        }

        protected override void ComputeVelocity()
        {
            if (IsMovementBlocked())
            {
                targetVelocity = Vector2.zero;
                velocity = Vector2.zero;
                return;
            }
            
            if (jumpState == JumpState.DashingLeft)
            {
                targetVelocity = new Vector2(-dashSpeed, 0);
                velocity.y = 0;
                return;
            }
            else if (jumpState == JumpState.DashingRight)
            {
                targetVelocity = new Vector2(dashSpeed, 0);
                velocity.y = 0;
                return;
            }
            else if (stopDash)
            {
                velocity.x = 0;
                stopDash = false;
            }
            
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            DashingLeft,
            DashingRight,
            InFlight,
            Landed
        }
        
        public void Land()
        {
            dashExpended = false;
        }
    }
}