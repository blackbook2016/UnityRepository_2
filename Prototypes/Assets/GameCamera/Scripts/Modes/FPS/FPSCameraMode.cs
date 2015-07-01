// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using RG_GameCamera.Input;
using UnityEngine;

namespace RG_GameCamera.Modes
{
    /// <summary>
    /// simple FPS camera
    /// </summary>
    [RequireComponent(typeof(Config.FPSConfig))]
    public class FPSCameraMode : CameraMode
    {
        private float rotX;
        private float rotY;
        private bool targetHide;
        private float activateTimeout;

        public override Type Type
        {
            get { return Type.FPS; }
        }

        public override void OnActivate()
        {
            base.OnActivate();

            if (Target)
            {
                cameraTarget = Target.position;
                UnityCamera.transform.position = GetEyePos();
                UnityCamera.transform.LookAt(GetEyePos() + Target.forward);
                RotateCamera(Vector2.zero);
                targetHide = false;
                activateTimeout = 1.0f;
            }
            else
            {
                Utils.Debug.Assert(false);
            }
        }

        public override void OnDeactivate()
        {
            // show target
            ShowTarget(true);
        }

        private Vector3 GetEyePos()
        {
            if (config.IsVector3("TargetOffset"))
            {
                return Target.transform.position + config.GetVector3("TargetOffset");
            }

            return Target.transform.position;
        }

        private void UpdateFOV()
        {
            UnityCamera.fieldOfView = config.GetFloat("FOV");
        }

        /// <summary>
        /// update new camera target
        /// </summary>
        public override void SetCameraTarget(Transform target)
        {
            base.SetCameraTarget(target);

            if (target)
            {
                cameraTarget = Target.position;
                UnityCamera.transform.position = GetEyePos();
                UnityCamera.transform.LookAt(GetEyePos() + Target.forward);
                RotateCamera(Vector2.zero);
            }
        }

        public override void Init()
        {
            base.Init();

            config = GetComponent<Config.FPSConfig>();

            cameraTarget = Target.position;
            UnityCamera.transform.position = GetEyePos();

            if (config.IsFloat("RotationSpeedY"))
            {
                RotateCamera(Vector2.zero);
            }
        }

        /// <summary>
        /// rotate camera to new screen position
        /// </summary>
        public void RotateCamera(Vector2 mousePosition)
        {
            Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);

            rotY += config.GetFloat("RotationSpeedY") * mousePosition.y * 0.01f;
            rotX += config.GetFloat("RotationSpeedX") * mousePosition.x * 0.01f;

            // limit vertical angle
            var yAngle = -rotY * Mathf.Rad2Deg;
            var limitMax = config.GetFloat("RotationYMax");
            var limitMin = config.GetFloat("RotationYMin");

            if (yAngle > limitMax)
            {
                rotY = -limitMax * Mathf.Deg2Rad;
            }
            if (yAngle < limitMin)
            {
                rotY = -limitMin * Mathf.Deg2Rad;
            }
        }

        private void UpdateDir()
        {
            Vector3 dir;
            Utils.Math.ToCartesian(rotX, rotY, out dir);

            UnityCamera.transform.forward = dir;
            UnityCamera.transform.position = GetEyePos();

            activateTimeout -= Time.deltaTime;

            if (activateTimeout > 0.0f)
            {
                UnityCamera.transform.LookAt(GetEyePos() + Target.forward);
            }
        }

        private void UpdateTargetVisibility()
        {
            var hideTarget = config.GetBool("HideTarget");

            if (hideTarget != targetHide)
            {
                targetHide = hideTarget;
                ShowTarget(!targetHide);
            }
        }

        private void ShowTarget(bool show)
        {
            Utils.Debug.SetVisible(Target.gameObject, show, true);
        }

		/// <summary>
		/// reset camera to its original settings
		/// </summary>
		public override void Reset()
		{
			activateTimeout = 0.1f;
		}

        public override void PostUpdate()
        {
            if (disableInput)
            {
                return;
            }

            if (InputManager)
            {
                //
                // update target (character) visibility
                //
                if (activateTimeout < 0.0f)
                {
                    UpdateTargetVisibility();
                }

                //
                // setup FOV
                //
                UpdateFOV();

                //
                // update rotation
                //
                if (InputManager.GetInput(InputType.Rotate).Valid)
                {
                    RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
                }

                //
                // apply rotations to camera forward dir
                //
                UpdateDir();
            }
        }
    }
}
