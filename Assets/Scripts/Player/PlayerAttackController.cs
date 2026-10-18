using System;
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

        public WeaponSlot westSlot;
        public WeaponSlot northSlot;
        public WeaponSlot eastSlot;
        public WeaponSlot kickSlot;

        private WeaponSlot activeSlot = null;

        public override bool IsFacingLeft()
        {
            return MovementController.IsFacingLeft();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected override void Awake()
        {
            base.Awake();

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

            westSlot.button = InputSystem.actions.FindAction("Player/WeaponWest");
            northSlot.button = InputSystem.actions.FindAction("Player/WeaponNorth");
            eastSlot.button = InputSystem.actions.FindAction("Player/WeaponEast");
            kickSlot.button = InputSystem.actions.FindAction("Player/Kick");

            westSlot.button.Enable();
            northSlot.button.Enable();
            eastSlot.button.Enable();
            kickSlot.button.Enable();

        }

        // Update is called once per frame
        void Update()
        {
            bool weaponsBlocked = IsAttacking();

            if(IsAttacking())
            {
                if (activeSlot.button.WasPressedThisFrame())
                    currentAttack.InvokeTrigger(AttackTransition.TriggerType.ButtonPress);
                if (activeSlot.button.WasReleasedThisFrame())
                    currentAttack.InvokeTrigger(AttackTransition.TriggerType.ButtonRelease);
            }
            else
            {
                //Else-ifs so multiple weapons cannot start attacking on the same frame.
                if (SlotShouldAttack(westSlot))
                    StartAttack(westSlot);
                else if (SlotShouldAttack(northSlot))
                    StartAttack(northSlot);
                else if (SlotShouldAttack(eastSlot))
                    StartAttack(eastSlot);
                else if (SlotShouldAttack(kickSlot))
                    StartAttack(kickSlot);
            }

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

        public override bool IsFacingBlocked()
        {
            return base.IsFacingBlocked() || MovementController.IsDashing();
        }

        internal bool AttemptDashCancel()
        {
            //TODO: this
            return false;
        }

        private bool SlotShouldAttack(WeaponSlot slot)
        {
            return slot.button.WasPressedThisFrame() && slot.HasWeapon() && slot.weapon.CanFire();
        }

        private void StartAttack(WeaponSlot slot)
        {
            if (!slot.HasWeapon() || !slot.weapon.CanFire())
                throw new ArgumentException("No ready weapon in the given slot!");

            activeSlot = slot;
            StartAttack(slot.weapon.definition.initialPhase, slot.weapon.GenerateWeaponInfo());
        }

        internal override void EndWarmup()
        {
            if (activeSlot == null || !activeSlot.HasWeapon())
                throw new InvalidOperationException("No active weapon to end warmup of!");

            activeSlot.weapon.StartCooldown();
        }

        internal override void EndAttack()
        {
            base.EndAttack();
            activeSlot = null;
        }
    }
}
