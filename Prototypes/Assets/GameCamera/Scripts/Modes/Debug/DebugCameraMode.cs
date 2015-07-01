// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using RG_GameCamera.Input;
using RG_GameCamera.Utils;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Modes
{
    /// <summary>
    /// LookAt camera - static camera looking from one point to another with ability to change between targets
    /// </summary>
    [RequireComponent(typeof(Config.DebugConfig))]
    public class DebugCameraMode : CameraMode
    {
        private float rotX;
        private float rotY;

        public override Type Type
        {
            get { return Type.Debug; }
        }

        public override void OnActivate()
        {
            base.OnActivate();

            if (Target)
            {
                cameraTarget = Target.position;
//                UnityCamera.transform.LookAt(Target.position);
                RotateCamera(Vector2.zero);
            }
            else
            {
                Utils.Debug.Assert(false);
            }
        }

        public override void Init()
        {
            base.Init();

            UnityCamera.transform.LookAt(cameraTarget);

            config = GetComponent<Config.DebugConfig>();
        }

        private void UpdateFOV()
        {
            UnityCamera.fieldOfView = config.GetFloat("FOV");
        }

        /// <summary>
        /// rotate camera to new screen position
        /// </summary>
        public void RotateCamera(Vector2 mousePosition)
        {
            Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);

            rotY += config.GetFloat("RotationSpeedY") * mousePosition.y * 0.01f;
            rotX += config.GetFloat("RotationSpeedX") * mousePosition.x * 0.01f;
        }

        public void MoveCamera()
        {
            Vector3 moveDir = Vector3.zero;

            if (UnityInput.GetKey(KeyCode.W))
            {
                moveDir += UnityCamera.transform.forward;
            }

            if (UnityInput.GetKey(KeyCode.S))
            {
                moveDir += -UnityCamera.transform.forward;
            }

            if (UnityInput.GetKey(KeyCode.A))
            {
                moveDir += -UnityCamera.transform.right;
            }

            if (UnityInput.GetKey(KeyCode.D))
            {
                moveDir += UnityCamera.transform.right;
            }

            moveDir.Normalize();

            UnityCamera.transform.position += moveDir * config.GetFloat("MoveSpeed") * Time.deltaTime * 10;
        }

        private void UpdateDir()
        {
            Vector3 dir;
            Utils.Math.ToCartesian(rotX, rotY, out dir);

            UnityCamera.transform.forward = dir;
        }

        public override void PostUpdate()
        {
            //
            // update field of view
            //
            UpdateFOV();

            //
            // update rotation
            //
            if (CursorLocking.IsLocked)
            {
                RotateCamera(new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")));
            }

            //
            // update move
            //
            MoveCamera();

            //
            // apply rotations to camera forward dir
            //
            UpdateDir();
        }
    }
}
