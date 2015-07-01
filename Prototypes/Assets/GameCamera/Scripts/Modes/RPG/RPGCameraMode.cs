// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.Input;
using UnityEngine;

namespace RG_GameCamera.Modes
{
    /// <summary>
    /// RPG camera (Diablo like)
    /// </summary>
    [RequireComponent(typeof(Config.RPGConfig))]
    public class RPGCameraMode : CameraMode
    {
        public bool dbgRing = false;
        private float rotX;
        private float rotY;
        private float targetZoom;
        private Vector3 targetPos;
        private PositionFilter targetFilter;
        private Vector3 springVelocity;
        private GameObject debugRing;
        private float transitDistance;
        private float activateTimeout;

        public override Type Type
        {
            get { return Type.RPG; }
        }

        public override void Init()
        {
            base.Init();

            UnityCamera.transform.LookAt(cameraTarget);

            config = GetComponent<Config.RPGConfig>();

            Utils.DebugDraw.Enabled = true;

            targetFilter = new PositionFilter(10, 1.0f);
            targetFilter.Reset(Target.position);

            debugRing = Utils.RingPrimitive.Create(3.0f, 3.0f, 0.1f, 50, Color.red);
//            debugRing.GetComponent<MeshRenderer>().castShadows = false;
            Utils.Debug.SetActive(debugRing, dbgRing);

            config.TransitCallback = OnTransitMode;
            config.TransitionStartCallback = OnTransitStartMode;
        }

        public override void OnActivate()
        {
            base.OnActivate();

			config.SetCameraMode(DefaultConfiguration);

            // default distance from ground
            targetDistance = config.GetFloat("Distance");

            cameraTarget = Target.position;
            targetFilter.Reset(Target.position);
            targetPos = Target.position;

            // initialize camera with correct angles
            UpdateYAngle();
            UpdateXAngle(true);
            UpdateDir();

            activateTimeout = 2.0f;
        }

        /// <summary>
        /// transition callback
        /// </summary>
        void OnTransitMode(string newMode, float t)
        {
            var distance = config.GetFloat("Distance");

            targetDistance = Mathf.Lerp(transitDistance, distance, t);
        }

        /// <summary>
        /// transition start callback
        /// </summary>
        void OnTransitStartMode(string oldMode, string newMode)
        {
            transitDistance = targetDistance;
        }

        /// <summary>
        /// set a new camera target, this will just reset the camera position, target is not fixed
        /// </summary>
        public override void SetCameraTarget(Transform target)
        {
            base.SetCameraTarget(target);

            if (target)
            {
                cameraTarget = target.position;
            }
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void RotateCamera(Vector2 mousePosition)
        {
            if (config.GetBool("EnableRotation"))
            {
                if (mousePosition.sqrMagnitude > Mathf.Epsilon)
                {
                    rotX += config.GetFloat("RotationSpeed") * mousePosition.x * 0.01f;
                }
            }
        }

        /// <summary>
        /// update field of view
        /// </summary>
        private void UpdateFOV()
        {
            UnityCamera.fieldOfView = config.GetFloat("FOV");
        }

        /// <summary>
        /// update rotation of Y
        /// </summary>
        private void UpdateYAngle()
        {
            Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);

            float zoomRatio = 0.0f;

            if (UnityCamera.orthographic)
            {
                zoomRatio = (UnityCamera.orthographicSize - config.GetFloat("OrthoMin")) / (config.GetFloat("OrthoMax") - config.GetFloat("OrthoMin"));
            }
            else
            {
                zoomRatio = (targetDistance - config.GetFloat("DistanceMin")) / (config.GetFloat("DistanceMax") - config.GetFloat("DistanceMin"));
            }

            var yAngle = config.GetFloat("AngleZoomMin") * (1.0f - zoomRatio) + config.GetFloat("AngleY") * zoomRatio;
            rotY = Mathf.Lerp(rotY, yAngle * -1.0f * Mathf.Deg2Rad, Time.deltaTime * 50);
        }

        /// <summary>
        /// update rotatin of X
        /// </summary>
        /// <param name="force">update rotation even if the rotation is disabled</param>
        private void UpdateXAngle(bool force)
        {
            if (!config.GetBool("EnableRotation") || force || activateTimeout > 0.0f)
            {
                rotX = config.GetFloat("DefaultAngleX")*-Mathf.Deg2Rad;
//                Utils.Debug.Log("UPdateXAngle: {0}", rotX);
            }
        }

        private void UpdateDir()
        {
            Vector3 dir;
            Utils.Math.ToCartesian(rotX, rotY, out dir);

            UnityCamera.transform.forward = dir;

            UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
        }

        private void UpdateConfig()
        {
        }

        /// <summary>
        /// update collision detection to update transparent object in view
        /// </summary>
        private void UpdateCollision()
        {
            if (collision)
            {
                float colTargetDist, colDist;
                collision.ProcessCollision(cameraTarget, cameraTarget, UnityCamera.transform.forward, targetDistance, out colTargetDist, out colDist);
            }
        }

        private void UpdateZoom()
        {
            Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);

            if (Mathf.Abs(targetZoom) > Mathf.Epsilon)
            {
                var zoom = targetZoom * 20 * Time.deltaTime;

                if (Mathf.Abs(zoom) > Mathf.Abs(targetZoom))
                {
                    zoom = targetZoom;
                }

                Zoom(zoom);
                targetZoom -= zoom;
            }
        }

        /// <summary>
        /// zoom by amount
        /// </summary>
        public void Zoom(float amount)
        {
            if (!config.GetBool("EnableZoom"))
            {
                return;
            }

            var zoom = amount * config.GetFloat("ZoomSpeed");

            if (Mathf.Abs(zoom) > Mathf.Epsilon)
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

                    UnityCamera.orthographicSize = Mathf.Clamp(UnityCamera.orthographicSize, config.GetFloat("OrthoMin"), config.GetFloat("OrthoMax"));
                }
                else
                {
                    var zoomFactor = GetZoomFactor();

                    if (zoomFactor < 0.01f)
                    {
                        zoomFactor = 0.01f;
                    }

                    zoom *= zoomFactor;

                    var zoomAdd = UnityCamera.transform.forward * zoom;
                    var futurePos = UnityCamera.transform.position + zoomAdd;
                    var plane = new Plane(UnityCamera.transform.forward, cameraTarget);

                    if (!plane.GetSide(futurePos))
                    {
                        UnityCamera.transform.position = futurePos;
                    }
                }

                targetDistance = (UnityCamera.transform.position - cameraTarget).magnitude;
                targetDistance = Mathf.Clamp(targetDistance, config.GetFloat("DistanceMin"), config.GetFloat("DistanceMax"));
            }
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

        private void UpdateDistance()
        {
            cameraTarget = targetPos + GetOffsetPos();
        }

		/// <summary>
		/// reset camera to its original settings
		/// </summary>
		public override void Reset()
		{
			activateTimeout = 0.1f;

			targetDistance = config.GetFloat("Distance");
			cameraTarget = Target.position;
			targetFilter.Reset(Target.position);
			targetPos = Target.position;
			targetZoom = 0.0f;

			UpdateYAngle();
			UpdateXAngle(true);
			UpdateDir();
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
                // update camera config
                //
                UpdateConfig();

                //
                // update field of view
                //
                UpdateFOV();

                //
                // update zoom
                //
                if (InputManager.GetInput(InputType.Zoom).Valid)
                {
                    targetZoom = (float) InputManager.GetInput(InputType.Zoom).Value;
                }
                UpdateZoom();

                //
                // update rotation in Y
                //
                UpdateYAngle();

                //
                // update rotation in X
                //
                UpdateXAngle(false);

                //
                // update horiznotal rotation
                //
                if (InputManager.GetInput(InputType.Rotate).Valid)
                {
                    RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
                }

                //
                // update distance from the character
                //
                UpdateDistance();

                //
                // apply rotations to camera forward dir
                //
                UpdateDir();
            }

            activateTimeout -= Time.deltaTime;
        }

        /// <summary>
        /// fixed update - callback from unity
        /// </summary>
        public override void FixedStepUpdate()
        {
            targetFilter.AddSample(Target.position);

            //
            // calculate position in dead zone
            //

            var deadZone = config.GetVector2("DeadZone");

            if (deadZone.sqrMagnitude > Mathf.Epsilon)
            {
                // draw dead zone ring
                Utils.RingPrimitive.Generate(debugRing, deadZone.x, deadZone.y, 0.1f, 50);
                debugRing.transform.position = targetPos + Vector3.up * 2;
                debugRing.transform.forward = Utils.Math.VectorXZ(UnityCamera.transform.forward);
                Utils.Debug.SetActive(debugRing, dbgRing);

                var v0 = (targetFilter.GetValue() - targetPos);
                var distanceFromCenter = v0.magnitude;
                v0 /= distanceFromCenter;

                if (distanceFromCenter > deadZone.x || distanceFromCenter > deadZone.y)
                {
                    var localV0 = UnityCamera.transform.InverseTransformDirection(v0);
                    var angle = Mathf.Atan2(localV0.x, localV0.z);

                    var ellipseV0 = new Vector3(Mathf.Sin(angle), 0.0f, Mathf.Cos(angle));
                    var ellipseP0 = new Vector3(ellipseV0.x * deadZone.x, 0.0f, ellipseV0.z * deadZone.y);
                    var ellipseDistance = ellipseP0.magnitude;

                    if (distanceFromCenter > ellipseDistance)
                    {
                        var newTargetPos = targetPos + v0 * (distanceFromCenter - ellipseDistance);
                        targetPos = Vector3.SmoothDamp(targetPos, newTargetPos, ref springVelocity, config.GetFloat("Spring"));
                    }
                }
            }
            else
            {
                //
                // apply spring effect
                //
                targetPos = Vector3.SmoothDamp(targetPos, targetFilter.GetValue(), ref springVelocity, config.GetFloat("Spring"));
                targetPos.y = targetFilter.GetValue().y;
//                targetPos = Vector3.Lerp(targetPos, targetFilter.GetValue(), Time.deltaTime*config.GetFloat("Spring") * 10);
//                Utils.Debug.Log("TargetPos: {0}, Filter {0}", targetPos, targetFilter.GetValue());
            }

            //
            // update camera collision to update transpart objects in collision with camera
            //
            UpdateCollision();

            UpdateTargetDummy();
        }
    }
}
