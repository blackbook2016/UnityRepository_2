// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Modes
{
    /// <summary>
    /// LookAt camera - static camera looking from one point to another with ability to change between targets
    /// </summary>
    [RequireComponent(typeof(Config.LookAtConfig))]
    public class LookAtCameraMode : CameraMode
    {
        /// <summary>
        /// callback when the look at interpolation is finished
        /// </summary>
        public delegate void OnLookAtFinished();

        private Vector3 newTarget;
        private Vector3 newPos;
        private Vector3 oldPos;
        private Vector3 oldTarget;
        private Quaternion oldRot;
        private Quaternion newRot;
        private float targetTimeoutMax;
        private float targetTimeout;
        private OnLookAtFinished finishedCallback;

        public override Type Type
        {
            get { return Type.LookAt; }
        }

        public override void Init()
        {
            base.Init();

            UnityCamera.transform.LookAt(cameraTarget);

            config = GetComponent<Config.LookAtConfig>();

            targetTimeout = -1.0f;
            targetTimeoutMax = 1.0f;
        }

        public override void OnActivate()
        {
            ApplyCurrentCamera();
        }

        public void RegisterFinishCallback(OnLookAtFinished callback)
        {
            finishedCallback += callback;
        }

        public void UnregisterFinishCallback(OnLookAtFinished callback)
        {
            finishedCallback -= callback;
        }

        /// <summary>
        /// apply current camera settings to LookAt so the camera doesn't change
        /// </summary>
        public void ApplyCurrentCamera()
        {
            var ray = new Ray(UnityCamera.transform.position, UnityCamera.transform.forward);
            Vector3 hitPoint = ray.origin + ray.direction * 100.0f;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue))
            {
                hitPoint = hit.point;
            }

            cameraTarget = hitPoint;
            targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
            UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
        }

        /// <summary>
        /// smooth look at point from current camera position in timeout [s]
        /// </summary>
        /// <param name="point">target position</param>
        /// <param name="timeout">time to smooth look</param>
        public void LookAt(Vector3 point, float timeout)
        {
            LookAt(UnityCamera.transform.position, point, timeout);
        }

        /// <summary>
        /// smooth look at point from new position to new target in timeout [s]
        /// </summary>
        /// <param name="from">new position of camera</param>
        /// <param name="point">target position</param>
        /// <param name="timeout">time to smooth look</param>
        public void LookAt(Vector3 from, Vector3 point, float timeout)
        {
            oldPos = UnityCamera.transform.position;
            oldTarget = cameraTarget;
            oldRot = UnityCamera.transform.rotation;
            newPos = from;
            newTarget = point;

            if (timeout < 0.0f)
            {
                timeout = 0.0f;
            }

            newRot = Quaternion.LookRotation(point - from);
            targetTimeoutMax = timeout;
            targetTimeout = timeout;
        }

        /// <summary>
        /// smooth look at current point from new position in timeout [s]
        /// </summary>
        /// <param name="from">new position of camera</param>
        /// <param name="timeout">time to smooth look</param>
        public void LookFrom(Vector3 from, float timeout)
        {
            LookAt(from, cameraTarget, timeout);
        }

        private void UpdateLookAt()
        {
            if (targetTimeout >= 0.0f)
            {
                targetTimeout -= Time.deltaTime;

                float t;

                if (targetTimeoutMax < Mathf.Epsilon)
                {
                    t = 1.0f;
                }
                else
                {
                    t = 1.0f - targetTimeout/targetTimeoutMax;
                }

                var p0 = Utils.Interpolation.LerpS(oldPos, newPos, t);
                UnityCamera.transform.position = p0;

                if (config.GetBool("InterpolateTarget"))
                {
                    cameraTarget = Utils.Interpolation.LerpS(oldTarget, newTarget, t);
                    UnityCamera.transform.LookAt(cameraTarget);
                }
                else
                {
                    var r0 = Quaternion.Slerp(oldRot, newRot, Utils.Interpolation.LerpS(0, 1, t));
                    UnityCamera.transform.rotation = r0;
                }

                if (targetTimeout < 0.0f)
                {
                    if (finishedCallback != null)
                    {
                        finishedCallback();
                    }
                }
            }
        }

        private void UpdateFOV()
        {
            UnityCamera.fieldOfView = config.GetFloat("FOV");
        }

        public override void PostUpdate()
        {
            //
            // update field of view
            //
            UpdateFOV();

            //
            // update look at interpolation (changing between targets)
            //
            UpdateLookAt();
        }
    }
}
