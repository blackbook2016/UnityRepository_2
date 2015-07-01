// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using System.Collections;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// base class for animated character controller
    /// </summary>
    [RequireComponent(typeof(AnimationController))]
    [RequireComponent(typeof(WeaponController))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AIController))]
    public class Human : HitEntity
    {
        protected AnimationController animController;
        protected WeaponController weaponController;

        protected virtual void Awake()
        {
            animController = GetComponent<AnimationController>();
            Utils.Debug.Assert(animController);

            weaponController = GetComponent<WeaponController>();
            Utils.Debug.Assert(weaponController);
        }

        /// <summary>
        /// Flag for remote players
        /// True means that this Human/Player is a player from remote machine in multiplayer
        /// False means that this Human/Player is a player on local machine
        /// </summary>
        public bool Remote { get; set; }

        public void Move(AnimationController.Input input)
        {
            animController.Move(input);
        }

        public void Aim(bool status)
        {
            if (status)
            {
                weaponController.Aim();
            }
            else
            {
                weaponController.Hold();
            }
        }

        public void Shoot()
        {
            weaponController.Shoot();
        }

        public void ShootAt(HitEntity entity)
        {
            weaponController.ShootAt(entity);
        }

        public void ShootAt(Vector3 aimVector)
        {
            weaponController.ShootAt(aimVector);
        }
    }
}
