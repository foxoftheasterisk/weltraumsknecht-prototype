using UnityEngine;
using UnityEngine.InputSystem;
using Weltraumsknecht.Attacks;

namespace Weltraumsknecht.Player
{

    [RequireComponent(typeof(PlayerMovementController))]
    public class PlayerAttackController : Attacker
    {
        public PlayerMovementController MovementController
        { get; private set; }

        private InputAction m_WestWeaponAction;
        private InputAction m_NorthWeaponAction;
        private InputAction m_EastWeaponAction;
        private InputAction m_KickAction;

        public WeaponSlot westSlot;
        public WeaponSlot northSlot;
        public WeaponSlot eastSlot;
        public WeaponSlot kickSlot;

        public override bool IsFacingLeft()
        {
            return MovementController.IsFacingLeft();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            MovementController = GetComponent<PlayerMovementController>();

            //TODO: make weapons clear only on game reset
            if (westSlot.HasWeapon())
                westSlot.weapon = null;
            if (northSlot.HasWeapon())
                northSlot.weapon = null;
            if (eastSlot.HasWeapon())
                eastSlot.weapon = null;

            if (kickSlot.HasWeapon())
                kickSlot.weapon.Initialize(this);

            m_WestWeaponAction = InputSystem.actions.FindAction("Player/WeaponWest");
            m_NorthWeaponAction = InputSystem.actions.FindAction("Player/WeaponNorth");
            m_EastWeaponAction = InputSystem.actions.FindAction("Player/WeaponEast");
            m_KickAction = InputSystem.actions.FindAction("Player/Kick");

            m_WestWeaponAction.Enable();
            m_NorthWeaponAction.Enable();
            m_EastWeaponAction.Enable();
            m_KickAction.Enable();

        }

        // Update is called once per frame
        void Update()
        {
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
            //good news, past me! it's probably changing

            UpdateWeapons();
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

        internal bool IsFacingBlocked()
        {
            return MovementController.IsDashing() || (westSlot.HasWeapon() && westSlot.weapon.IsBlockingFacing())
                    || (northSlot.HasWeapon() && northSlot.weapon.IsBlockingFacing())
                    || (eastSlot.HasWeapon() && eastSlot.weapon.IsBlockingFacing())
                    || (kickSlot.HasWeapon() && kickSlot.weapon.IsBlockingFacing());
        }

        internal bool IsMovementBlocked()
        {
            return (westSlot.HasWeapon() && westSlot.weapon.IsBlockingMovement())
                    || (northSlot.HasWeapon() && northSlot.weapon.IsBlockingMovement())
                    || (eastSlot.HasWeapon() && eastSlot.weapon.IsBlockingMovement())
                    || (kickSlot.HasWeapon() && kickSlot.weapon.IsBlockingMovement());
        }

        internal bool AreWeaponsBlocked()
        {
            return (westSlot.HasWeapon() && westSlot.weapon.IsBlockingWeapons())
                    || (northSlot.HasWeapon() && northSlot.weapon.IsBlockingWeapons())
                    || (eastSlot.HasWeapon() && eastSlot.weapon.IsBlockingWeapons())
                    || (kickSlot.HasWeapon() && kickSlot.weapon.IsBlockingWeapons());
        }

        internal bool AttemptDashCancel()
        {
            //TODO: this
            return false;
        }
    }
}
