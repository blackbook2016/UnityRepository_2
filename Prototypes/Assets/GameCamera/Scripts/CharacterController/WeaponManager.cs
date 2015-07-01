// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// helper for gun manipulation
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        public Light FlashLight;
        public GunSFX GunSfx;

        public static WeaponManager Instance
        {
            get { return instance; }
        }

        private static WeaponManager instance;

        private void Awake()
        {
            instance = this;

            FlashLight.intensity = 0.0f;
        }

        public void StartShootSFX(Transform parent)
        {
            FlashLight.transform.parent = parent;
            FlashLight.transform.localPosition = Vector3.zero;
            FlashLight.intensity = 1.0f;

            GunSfx.transform.parent = parent;
            GunSfx.transform.localPosition = Vector3.zero;
            GunSfx.transform.forward = CameraManager.Instance.UnityCamera.transform.forward;
            GunSfx.Emit();
        }

        public void StopShootSFX()
        {
            if (FlashLight.intensity > 0.0f)
            {
                FlashLight.intensity = 0.0f;
            }
        }
    }
}
