// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.Input;
using RG_GameCamera.Input.Mobile;
using UnityEngine;

namespace RG_GameCamera.Modes
{
    /// <summary>
    /// RTS - real-time strategy camera (Starcraft like)
    /// </summary>
    [RequireComponent(typeof(Config.RTSConfig))]
    public class RTSCameraMode : CameraMode
    {
        private float rotX;
        private float rotY;
        private float targetZoom;
        private Plane groundPlane;
        private bool panning;
        private Vector3 panMousePosition;
        private Vector3 panCameraTarget;
        private Vector3 panCameraPos;
        private float activateTimeout;
        private Vector3 dragVelocity;
        private float dragSlowdown;
        private bool updatePanTarget;

        public override Type Type
        {
            get { return Type.RTS; }
        }

        public override void Init()
        {
            base.Init();

            UnityCamera.transform.LookAt(cameraTarget);

            config = GetComponent<Config.RTSConfig>();

            Utils.DebugDraw.Enabled = true;
        }

        public override void OnActivate()
        {
            base.OnActivate();

			config.SetCameraMode(DefaultConfiguration);

            // default distance from ground
            targetDistance = config.GetFloat("Distance");

            // offset from the ground
            groundPlane = new Plane(Vector3.up, config.GetFloat("GroundOffset"));

            if (Target)
            {
                cameraTarget = Target.position;
            }

            UpdateYAngle();
            UpdateXAngle(true);

            activateTimeout = 2.0f;
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
        private bool RotateCamera(Vector2 mousePosition)
        {
            if (config.GetBool("EnableRotation") && mousePosition.sqrMagnitude > Mathf.Epsilon)
            {
                rotX += config.GetFloat("RotationSpeed") * mousePosition.x * 0.01f;

                updatePanTarget = true;

                return true;
            }

            return false;
        }

        /// <summary>
        /// pan camera by dragging the mouse
        /// </summary>
        private void DragCamera(Vector2 mousePosition)
        {
            if (panning)
            {
                // run raycast in direction of camera
                UnityCamera.transform.position = panCameraPos;
                var ray = UnityCamera.ScreenPointToRay(mousePosition);
                Vector3 currPanPos;

                var rayEnter = 0.0f;
                if (groundPlane.Raycast(ray, out rayEnter))
                {
                    currPanPos = ray.origin + ray.direction * rayEnter;
                }
                else
                {
                    Utils.Debug.Assert(false, "Invalid game plane!");
                    currPanPos = ray.origin + ray.direction * targetDistance;
                }

                var v0 = currPanPos - panMousePosition;
                v0.y = 0.0f;
                var nextTarget = panCameraTarget - v0;

                ClampWithinMapBounds(cameraTarget, ref nextTarget, true);

                dragVelocity = nextTarget - cameraTarget;
                dragSlowdown = 1.0f;
                cameraTarget = nextTarget;
            }
            else
            {
                panCameraTarget = cameraTarget;
                panCameraPos = UnityCamera.transform.position;

                // run raycast in direction of camera
                var ray = UnityCamera.ScreenPointToRay(mousePosition);

                // update ground plane
                Vector3 wp;
                if (GameInput.FindWaypointPosition(mousePosition, out wp))
                {
                    groundPlane.distance = wp.y;
                }

                var rayEnter = 0.0f;
                if (groundPlane.Raycast(ray, out rayEnter))
                {
                    panMousePosition = ray.origin + ray.direction*rayEnter;
                }
                else
                {
                    Utils.Debug.Assert(false, "Invalid game plane!");
                    panMousePosition = ray.origin + ray.direction * targetDistance;
                }

//                Utils.DebugDraw.Sphere(panMousePosition, 0.5f, Color.red, 10);

                panning = true;
            }
        }

        private void UpdatePanTarget()
        {
            if (updatePanTarget)
            {
             //   panning = false;
            }
        }

        private void UpdateDragMomentum()
        {
            if (dragVelocity.sqrMagnitude > Mathf.Epsilon)
            {
                dragSlowdown -= Time.deltaTime;

                if (dragSlowdown < 0.0f)
                {
                    dragSlowdown = 0.0f;
                }

                dragVelocity *= dragSlowdown;

                cameraTarget += dragVelocity * Time.deltaTime * config.GetFloat("DragMomentum") * 100;

                ClampWithinMapBounds(cameraTarget, ref cameraTarget, true);
            }

            var center = config.GetVector2("MapCenter");
            var size = config.GetVector2("MapSize");
            var softBorder = config.GetFloat("SoftBorder");
            const float s = 40.0f;
            var speed = 0.0f;

            if (cameraTarget.x > center.x + size.x / 2)
            {
                speed = (cameraTarget.x - (center.x + size.x/2))/softBorder;
                cameraTarget.x -= Time.deltaTime*s*speed;
            }
            else if (cameraTarget.x < center.x - size.x / 2)
            {
                speed = (-cameraTarget.x + center.x - size.x / 2) / softBorder;
                cameraTarget.x += Time.deltaTime*s*speed;
            }

            if (cameraTarget.z > center.y + size.y / 2)
            {
                speed = (cameraTarget.z - (center.y + size.y / 2)) / softBorder;
                cameraTarget.z -= Time.deltaTime*s*speed;
            }
            else if (cameraTarget.z < center.y - size.y / 2)
            {
                speed = (-cameraTarget.z + center.y - size.y / 2) / softBorder;
                cameraTarget.z += Time.deltaTime*s*speed;
            }
        }

        /// <summary>
        /// pan camera with movement vector (useful for gamepad movement)
        /// </summary>
        /// <param name="move"></param>
        private void MoveCamera(Vector2 move)
        {
            if (move.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            move *= 0.1f * config.GetFloat("MoveSpeed") * GetZoomFactor();

            var forward = UnityCamera.transform.forward;
            forward.y = 0.0f;
            forward.Normalize();

            var right = UnityCamera.transform.right;
            right.y = 0.0f;
            right.Normalize();

            var panDiff = forward * move.y + right * move.x;
            var nextTarget = cameraTarget + panDiff;

            ClampWithinMapBounds(cameraTarget, ref nextTarget, false);
            cameraTarget = nextTarget;
        }

        /// <summary>
        /// pan camera with mouse when the cursor is close to the screen border
        /// </summary>
        /// <param name="mousePosition"></param>
        private void MoveCameraByScreenBorder(Vector2 mousePosition)
        {
            var screenPos = mousePosition;
            screenPos.y = Screen.height - screenPos.y;

            var borderOffset = config.GetFloat("ScreenBorderOffset");

            var v0 = Vector2.zero;
            var distRatio = 0.0f;

            if (screenPos.x <= borderOffset)
            {
                v0.x = -1.0f;
                distRatio = 1.0f - screenPos.x/borderOffset;
            }
            else if (screenPos.x >= Screen.width - borderOffset)
            {
                v0.x = 1.0f;
                distRatio = 1.0f - (Screen.width - screenPos.x) / borderOffset;
            }

            if (screenPos.y >= Screen.height - borderOffset)
            {
                v0.y = -1.0f;
                distRatio = 1.0f - (Screen.height - screenPos.y) / borderOffset;
            }
            else if (screenPos.y <= borderOffset)
            {
                v0.y = 1.0f;
                distRatio = 1.0f - screenPos.y/borderOffset;
            }

            if (v0.sqrMagnitude > Mathf.Epsilon)
            {
                v0.Normalize();

                distRatio = Mathf.Clamp01(distRatio);
                var move = v0 * Time.deltaTime * distRatio * config.GetFloat("ScreenBorderSpeed") * GetZoomFactor();

                var forward = UnityCamera.transform.forward;
                forward.y = 0.0f;
                forward.Normalize();

                var right = UnityCamera.transform.right;
                right.y = 0.0f;
                right.Normalize();

                var panDiff = forward * move.y + right * move.x;
                var nextTarget = Vector3.Lerp(cameraTarget, cameraTarget+panDiff, Time.deltaTime*50);

                ClampWithinMapBounds(cameraTarget, ref nextTarget, false);
                cameraTarget = nextTarget;
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
                rotX = config.GetFloat("DefaultAngleX") * -Mathf.Deg2Rad;
            }
        }

        private void UpdateDir()
        {
            Vector3 dir;
            Utils.Math.ToCartesian(rotX, rotY, out dir);

            UnityCamera.transform.forward = dir;

            UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;

            UpdatePanTarget();
        }

        private void UpdateConfig()
        {
        }

        private void UpdateDistance()
        {
            if (Target && config.GetBool("FollowTargetY"))
            {
                cameraTarget.y = Target.position.y;
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

                updatePanTarget = true;
            }
        }

        private bool IsInMapBounds(Vector3 point)
        {
            Utils.Math.Swap(ref point.y, ref point.z);

            var center = config.GetVector2("MapCenter");
            var size = config.GetVector2("MapSize");

            var rect = new Rect(center.x - size.x/2, center.y - size.y/2, size.x, size.y);

            return rect.Contains(point);
        }

        private void ClampWithinMapBounds(Vector3 currTarget, ref Vector3 point, bool border)
        {
            var center = config.GetVector2("MapCenter");
            var size = config.GetVector2("MapSize");

            // limit horizontal / vertical movement
            if (config.GetBool("DisableHorizontal"))
            {
                point.x = currTarget.x;
            }

            if (config.GetBool("DisableVertical"))
            {
                point.z = currTarget.z;
            }

            var softBorder = config.GetFloat("SoftBorder");

            if (!border)
            {
                softBorder = 0.0f;
            }

            if (point.x > center.x + size.x/2 + softBorder)
            {
                point.x = center.x + size.x/2 + softBorder;
            }
            else if (point.x < center.x - size.x/2 - softBorder)
            {
                point.x = center.x - size.x/2 - softBorder;
            }

            if (point.z > center.y + size.y/2 + softBorder)
            {
                point.z = center.y + size.y/2 + softBorder;
            }
            else if (point.z < center.y - size.y/2 - softBorder)
            {
                point.z = center.y - size.y/2 - softBorder;
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

		/// <summary>
		/// reset camera to its original settings
		/// </summary>
		public override void Reset()
		{
			activateTimeout = 0.1f;
			targetZoom = 0.0f;

			targetDistance = config.GetFloat("Distance");

			if (Target)
			{
				cameraTarget = Target.position;
			}
		}

        public override void PostUpdate()
        {
            if (disableInput)
            {
                return;
            }

            if (InputManager)
            {
                updatePanTarget = false;

                //
                // update camera config
                //
                UpdateConfig();
                UpdateDistance();

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
                var rotating = false;
                if (InputManager.GetInput(InputType.Rotate).Valid)
                {
                    RotateCamera((Vector2)InputManager.GetInput(InputType.Rotate).Value);
                    rotating = true;
                }

                if (!rotating)
                {
                    //
                    // update camera movement
                    //
                    if (config.GetBool("DraggingMove"))
                    {
                        if (InputManager.GetInput(InputType.Pan).Valid)
                        {
                            DragCamera((Vector2)InputManager.GetInput(InputType.Pan).Value);
                        }
                        else
                        {
                            UpdateDragMomentum();
                            panning = false;
                        }
                    }

                    if (!panning)
                    {
                        //
                        // update camera movement by keyboard/joystick
                        //
                        if (config.GetBool("KeyMove"))
                        {
                            if (InputManager.GetInput(InputType.Move).Valid)
                            {
                                MoveCamera((Vector2)InputManager.GetInput(InputType.Move).Value);
                            }
                        }

                        //
                        // update camera movement by moving mouse on screen border
                        //
                        if (config.GetBool("ScreenBorderMove"))
                        {
                            MoveCameraByScreenBorder(UnityEngine.Input.mousePosition);
                        }
                    }
                }

                //
                // apply rotations to camera forward dir
                //
                UpdateDir();
            }

            activateTimeout -= Time.deltaTime;
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

        public override void FixedStepUpdate()
        {
            //
            // update camera collision to update transpart objects in collision with camera
            //
            UpdateCollision();
        }
    }
}
