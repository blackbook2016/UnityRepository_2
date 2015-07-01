// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using RG_GameCamera.Effects;
using UnityEngine;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// this class is responsible for manipulating gun like showing/hiding, shooting, aiming, running particles, sound effects
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class WeaponController : MonoBehaviour
    {
        public const string MuzzleName = "Muzzle";

        private GameObject weapon;
        private Transform muzzle;
        private bool aiming;
        private AudioSource audioSource;
        private float shootTimeout;
        private float flashTimeout;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            Utils.Debug.Assert(audioSource);
            aiming = true;
        }

        private void Update()
        {
            shootTimeout -= Time.deltaTime;
            flashTimeout -= Time.deltaTime;

            if (flashTimeout < 0.0f)
            {
                if (WeaponManager.Instance)
                {
                    WeaponManager.Instance.StopShootSFX();
                }
            }
        }

        public void InitWeapon(GameObject parent, string name)
        {
            var kids = parent.GetComponentsInChildren<Transform>();

            // find weapon in the hierarchy
            foreach (var kid in kids)
            {
                if (kid.name == name)
                {
                    weapon = kid.gameObject;
                    break;
                }
            }

            if (weapon)
            {
                // find muzzle child
                var childs = weapon.GetComponentsInChildren<Transform>();
                foreach (var child in childs)
                {
                    if (child.name == MuzzleName)
                    {
                        muzzle = child;
                        break;
                    }
                }
            }
        }

        public void Aim()
        {
            if (!aiming)
            {
                aiming = true;
                Utils.Debug.SetActive(weapon, aiming);
            }
        }

        public void Hold()
        {
            if (aiming)
            {
                aiming = false;
                Utils.Debug.SetActive(weapon, aiming);
            }
        }

        public void Shoot()
        {
            if (aiming)
            {
                if (shootTimeout < 0.0f)
                {
                    if (SoundManager.Instance && WeaponManager.Instance && TargetManager.Instance && muzzle)
                    {
                        // play shoot sound
                        SoundManager.Instance.PlayShoot(audioSource);
                        shootTimeout = 0.2f;

                        // blink flash light and show the shot texture
                        WeaponManager.Instance.StartShootSFX(muzzle.transform);
                        flashTimeout = 0.1f;

                        TargetManager.Instance.Shoot(gameObject.GetComponent<Player>(), 10);
                    }

                    var fireKick = EffectManager.Instance.Create<FireKick>();
                    fireKick.Play();
                }
            }
        }

        public void ShootAt(HitEntity entity)
        {
            if (aiming)
            {
                if (shootTimeout < 0.0f)
                {
                    if (SoundManager.Instance && WeaponManager.Instance && TargetManager.Instance && muzzle)
                    {
                        // play shoot sound
                        SoundManager.Instance.PlayShoot(audioSource);
                        shootTimeout = 0.2f;

                        // blink flash light and show the shot texture
                        WeaponManager.Instance.StartShootSFX(muzzle.transform);
                        flashTimeout = 0.1f;
                    }

                    entity.OnHit(gameObject.GetComponent<Player>(), 10, entity.transform.position+Vector3.up);
                }
            }
        }

        public void ShootAt(Vector3 aimVector)
        {
            if (aiming)
            {
                if (shootTimeout < 0.0f)
                {
                    if (SoundManager.Instance && WeaponManager.Instance && TargetManager.Instance && muzzle)
                    {
                        // play shoot sound
                        SoundManager.Instance.PlayShoot(audioSource);
                        shootTimeout = 0.2f;

                        // blink flash light and show the shot texture
                        WeaponManager.Instance.StartShootSFX(muzzle.transform);
                        flashTimeout = 0.1f;

                        TargetManager.Instance.ShootAt(aimVector, gameObject.GetComponent<Player>(), 10);
                    }
                }
            }
        }
    }
}
