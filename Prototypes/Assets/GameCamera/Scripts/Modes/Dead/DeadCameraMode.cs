// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Modes
{
    /// <summary>
    /// Dead camera - simple camera mode rotating around target (dead character)
    /// </summary>
    [RequireComponent(typeof(Config.DeadConfig))]
    public class DeadCameraMode : CameraMode
    {
        private float rotX;
        private float rotY;
        private float angle;

        public override Type Type
        {
            get { return Type.Dead; }
        }

        public override void Init()
        {
            base.Init();

            Utils.Debug.Assert(collision != null, "Missing collision system componnent!");

            UnityCamera.transform.LookAt(cameraTarget);

            config = GetComponent<Config.DeadConfig>();

            Utils.Debug.Assert(collision);
        }

        public override void OnActivate()
        {
            base.OnActivate();

            targetDistance = (cameraTarget - UnityCamera.transform.position).magnitude;
        }

        /// <summary>
        /// rotate camera around player
        /// </summary>
        private void RotateCamera()
        {
            Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);

            angle = config.GetFloat("RotationSpeed")*Time.deltaTime;

            rotY = -config.GetFloat("Angle")*Mathf.Deg2Rad;
            rotX += angle;
        }

        private void UpdateFOV()
        {
            UnityCamera.fieldOfView = config.GetFloat("FOV");
        }

        private Vector3 GetOffsetPos()
        {
            var cfgOffset = config.GetVector3("TargetOffset");

            var camForwardXZ = Utils.Math.VectorXZ(UnityCamera.transform.forward);
            var camRightXZ = Utils.Math.VectorXZ(UnityCamera.transform.right);
            var camUp = Vector3.up;

            return camForwardXZ * cfgOffset.z +
                   camRightXZ * cfgOffset.x +
                   camUp * cfgOffset.y;
        }

        public override void PostUpdate()
        {
            //
            // update field of view
            //
            UpdateFOV();

            //
            // rotate camera around player
            //
            RotateCamera();

            //
            // calculate camera Collision
            //
            if (config.GetBool("Collision"))
            {
                UpdateCollision();
            }

            UpdateDir();
        }

        private void UpdateDir()
        {
            Vector3 dir;
            Utils.Math.ToCartesian(rotX, rotY, out dir);
            UnityCamera.transform.forward = dir;

            cameraTarget = Target.position + GetOffsetPos();
            UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
        }

        private void UpdateCollision()
        {
            var distance = config.GetFloat("Distance");

            float collisionDistance, collisionTargetDist;
            collision.ProcessCollision(cameraTarget, GetTargetHeadPos(), UnityCamera.transform.forward, distance, out collisionTargetDist, out collisionDistance);

            // interpolate camera distance
            targetDistance = Utils.Interpolation.Lerp(targetDistance, collisionDistance, targetDistance > collisionDistance ? collision.GetClipSpeed() : collision.GetReturnSpeed());
        }
    }
}
