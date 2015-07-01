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
    /// orbit camera with similar functionality to Unity Scene window camera
    /// features:
    /// * zoom
    /// * pan
    /// * rotate around target
    /// </summary>
    [RequireComponent(typeof(Config.OrbitConfig))]
    public class OrbitCameraMode : CameraMode
    {
        private Vector2 lastPanPosition;
        private bool panValid;
        private Vector3 newTarget;
        private float interpolateTime;

        public override Type Type
        {
            get { return Type.Orbit; }
        }

        public override void OnActivate()
        {
            base.OnActivate();

            if (Target)
            {
                cameraTarget = Target.position;
                newTarget = Target.position;
                interpolateTime = -0.1f;
                UnityCamera.transform.LookAt(cameraTarget);
                targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
                panValid = false;
                RotateCamera(Vector2.one * 1.0f);
                lastPanPosition = Vector2.zero;
            }
            else
            {
                var ray = new Ray(UnityCamera.transform.position, UnityCamera.transform.forward);
                Vector3 hitPoint = ray.origin + ray.direction * 100.0f;

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, float.MaxValue))
                {
                    hitPoint = hit.point;
                }

                newTarget = hitPoint;
                cameraTarget = newTarget;
                targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
                RotateCamera(Vector2.zero * 0.01f);
                lastPanPosition = Vector2.zero;
            }
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
                newTarget = Target.position;
                interpolateTime = 0.1f;
                UnityCamera.transform.LookAt(cameraTarget);
                targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
                panValid = false;
                RotateCamera(Vector2.zero*0.01f);
                lastPanPosition = Vector2.zero;
            }
        }

        public override void Init()
        {
            base.Init();

            UnityCamera.transform.LookAt(cameraTarget);

            config = GetComponent<Config.OrbitConfig>();
        }

        /// <summary>
        /// zoom camera by amount
        /// </summary>
        /// <param name="amount">zoom amount, positive value is zoom in, negative value is zoom out
        /// for example: amout = 0.1f</param>
        public void ZoomCamera(float amount)
        {
            if (config.GetBool("DisableZoom"))
            {
                return;
            }

            var zoom = amount * config.GetFloat("ZoomSpeed");

            if (Math.Abs(zoom) > Mathf.Epsilon)
            {
                if (UnityCamera.orthographic)
                {
                    var zoomFactor = GetZoomFactor();

                    zoom *= zoomFactor;

                    UnityCamera.orthographicSize -= zoom;
                    if (UnityCamera.orthographicSize < 0.01f)
                    {
                        UnityCamera.orthographicSize = 0.01f;
                    }
                }
                else
                {
                    float zoomFactor = GetZoomFactor();

                    if (zoomFactor < 0.01f)
                    {
                        zoomFactor = 0.01f;
                    }

                    zoom *= zoomFactor;

                    var zoomAdd = UnityCamera.transform.forward*zoom;
                    var futurePos = UnityCamera.transform.position + zoomAdd;
                    var plane = new Plane(UnityCamera.transform.forward, cameraTarget);

                    if (!plane.GetSide(futurePos))
                    {
                        UnityCamera.transform.position = futurePos;
                    }
                }

                targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
            }
        }

        /// <summary>
        /// pan camera to new screen position
        /// NOTE: this method requires to be called every frame since pan needs at least two positions to be working from -> to
        /// Example usage:
        /// if (Input.GetMouseButton(2))
        /// {
        ///     PanCamera(Input.mousePosition);
        /// }
        /// </summary>
        /// <param name="mousePosition">mouse position of rotation</param>
        public void PanCamera(Vector2 mousePosition)
        {
            if (config.GetBool("DisablePan"))
            {
                return;
            }

            if (panValid)
            {
                var mouseDiff = mousePosition - lastPanPosition;
                lastPanPosition = mousePosition;
                mouseDiff *= 0.01f * config.GetFloat("PanSpeed") * GetZoomFactor();

                var panDiff = -UnityCamera.transform.up * mouseDiff.y + -UnityCamera.transform.right * mouseDiff.x;

                UnityCamera.transform.position += panDiff;
                cameraTarget += panDiff;
            }
            else
            {
                lastPanPosition = mousePosition;
                panValid = true;
            }
        }

        /// <summary>
        /// pan camera with movement vector (useful for gamepad movement)
        /// </summary>
        /// <param name="move"></param>
        public void PanCameraWithMove(Vector2 move)
        {
            if (move.sqrMagnitude <= Mathf.Epsilon || config.GetBool("DisablePan"))
            {
                return;
            }

            move *= 0.1f * config.GetFloat("PanSpeed") * GetZoomFactor();

            var panDiff = UnityCamera.transform.up * move.y + UnityCamera.transform.right * move.x;

            UnityCamera.transform.position += panDiff;
            cameraTarget += panDiff;
        }

        /// <summary>
        /// rotate camera to new screen position
        /// NOTE: this method requires to be called every frame since rotate needs at least two positions to be working from -> to
        /// Example usage:
        /// if (Input.GetMouseButton(2))
        /// {
        ///     RotateCamera(Input.mousePosition);
        /// }
        /// </summary>
        /// <param name="mousePosition">mouse position of rotation</param>
        public void RotateCamera(Vector2 mousePosition)
        {
            if (config.GetBool("DisableRotation"))
            {
                return;
            }

            // don't mix pan with rotation
            if (!panValid)
            {
                var axisRight = UnityCamera.transform.right;
                UnityCamera.transform.RotateAround(cameraTarget, axisRight, config.GetFloat("RotationSpeedY") * -mousePosition.y);

                var limitX = config.GetFloat("RotationYMax");
                var limitY = config.GetFloat("RotationYMin");
                var limitMax = config.GetFloatMax("RotationYMax");
                var limitMin = config.GetFloatMin("RotationYMin");

                // clamp y rotation
                if (limitX < limitMax || limitY > limitMin)
                {
                    float rotX, rotY;
                    Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);
                    var yAngle = -rotY*Mathf.Rad2Deg;

                    var clampAngle = false;
                    var clampY = 0.0f;

                    if (limitX < limitMax && yAngle > limitX)
                    {
                        clampY = -limitX*Mathf.Deg2Rad;
                        clampAngle = true;
                    }
                    if (limitY > limitMin && yAngle < limitY)
                    {
                        clampY = -limitY*Mathf.Deg2Rad;
                        clampAngle = true;
                    }

                    if (clampAngle)
                    {
                        Vector3 dir;
                        Utils.Math.ToCartesian(rotX, clampY, out dir);
                        UnityCamera.transform.forward = dir;
                        UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
                    }
                }

                var axisUp = Vector3.up;
                UnityCamera.transform.RotateAround(cameraTarget, axisUp, config.GetFloat("RotationSpeedX") * mousePosition.x);

                var euler = UnityCamera.transform.eulerAngles;
                UnityCamera.transform.rotation = Quaternion.Euler(euler);
                UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
            }
        }

        /// <summary>
        /// reset camera position to current mouse position
        /// </summary>
        public void ResetCamera(Vector2 position)
        {
            // run raycast in direction of camera
            var ray = UnityCamera.ScreenPointToRay(position);

            Vector3 hitPoint = ray.origin + ray.direction*100.0f;

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue))
            {
                hitPoint = hit.point;
            }

            newTarget = hitPoint;
            interpolateTime = config.GetFloat("TargetInterpolation");
        }

        private void InterpolateTarget()
        {
            interpolateTime -= Time.deltaTime;

            cameraTarget = Vector3.Lerp(cameraTarget, newTarget, Time.deltaTime*10.0f);
//            UnityCamera.transform.LookAt(cameraTarget);
            UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward*targetDistance;
            targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
        }

        public override void PostUpdate()
        {
            if (disableInput)
            {
                return;
            }

            if (interpolateTime >= 0.0f)
            {
                InterpolateTarget();
                return;
            }

            if (InputManager)
            {
                //
                // setup FOV
                //
                UpdateFOV();

                //
                // update pan
                //
                if (InputManager.GetInput(InputType.Pan).Valid)
                {
                    PanCamera((Vector2)InputManager.GetInput(InputType.Pan).Value);
                }
                else
                {
                    if (InputManager.GetInput(InputType.Move).Valid)
                    {
                        PanCameraWithMove((Vector2) InputManager.GetInput(InputType.Move).Value);
                    }

                    panValid = false;
                }

                //
                // update zoom
                //
                if (InputManager.GetInput(InputType.Zoom).Valid)
                {
                    ZoomCamera((float)InputManager.GetInput(InputType.Zoom).Value);
                }

                //
                // update rotation
                //
                if (InputManager.GetInput(InputType.Rotate).Valid)
                {
                    RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
                }

                //
                // reset camera
                //
                var reset = InputManager.GetInput(InputType.Reset);
                if (reset.Valid && (bool)reset.Value)
                {
                    ResetCamera(UnityEngine.Input.mousePosition);
                }
            }
        }
    }
}
