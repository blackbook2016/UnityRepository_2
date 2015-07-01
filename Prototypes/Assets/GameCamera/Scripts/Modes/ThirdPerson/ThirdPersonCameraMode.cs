// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.Input;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Modes
{
    /// <summary>
    /// third person camera (Tombraider like)
    /// </summary>
    [RequireComponent(typeof(Config.ThirdPersonConfig))]
    public class ThirdPersonCameraMode : CameraMode
    {
        public float AutoResetTimeout = 1.0f;
        public float AutoResetSpeed = 1.0f;
        public bool AutoReset = true;

        public bool dbgRing = false;
        private bool rotationInput;
        private float rotationInputTimeout;
        private bool hasRotated;
        private float rotX;
        private float rotY;
        private float targetVelocity;
        private float collisionDistance;
        private float collisionZoomVelocity;

        private float currCollisionTargetDist;
        private float collisionTargetDist;
        private float collisionTargetVelocity;
        private Vector3 targetPos;
        private Vector3 lastTargetPos;
        private Vector3 springVelocity;
        private GameObject debugRing;
        private float activateTimeout;
        private float resetTimeout;

        private PositionFilter targetFilter;

        public override Type Type
        {
            get { return Type.ThirdPerson; }
        }

        public override void Init()
        {
            base.Init();

            Utils.Debug.Assert(collision != null, "Missing collision system componnent!");

            UnityCamera.transform.LookAt(cameraTarget);

            config = GetComponent<Config.ThirdPersonConfig>();

            lastTargetPos = Target.position;
            targetVelocity = 0.0f;

            debugRing = Utils.RingPrimitive.Create(3.0f, 3.0f, 0.1f, 50, Color.red);
//            debugRing.GetComponent<MeshRenderer>().castShadows = false;
            Utils.Debug.SetActive(debugRing, dbgRing);

            Utils.Debug.Assert(collision);

            targetFilter = new PositionFilter(10, 1.0f);
            targetFilter.Reset(Target.position);

            Utils.DebugDraw.Enabled = true;

            resetTimeout = 0.0f;
        }

        public override void OnActivate()
        {
            base.OnActivate();

            config.SetCameraMode(DefaultConfiguration);

            targetFilter.Reset(Target.position);
            lastTargetPos = Target.position;
            targetVelocity = 0.0f;
            activateTimeout = 1.0f;
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        private void RotateCamera(Vector2 mousePosition)
        {
            rotationInput = mousePosition.sqrMagnitude > Mathf.Epsilon;
            rotationInputTimeout += Time.deltaTime;

            if (rotationInput)
            {
                rotationInputTimeout = 0.0f;
                hasRotated = true;
            }

            Utils.Math.ToSpherical(UnityCamera.transform.forward, out rotX, out rotY);

            rotY += config.GetFloat("RotationSpeedY") * mousePosition.y * 0.01f;
            rotX += config.GetFloat("RotationSpeedX") * mousePosition.x * 0.01f;

            var yAngle = -rotY * Mathf.Rad2Deg;
            var limitMax = config.GetFloat("RotationYMax");
            var limitMin = config.GetFloat("RotationYMin");

            if (yAngle > limitMax)
            {
                rotY = -limitMax*Mathf.Deg2Rad;
            }
            if (yAngle < limitMin)
            {
                rotY = -limitMin*Mathf.Deg2Rad;
            }
        }

        private void UpdateFollow()
        {
            Vector3 targetDiff = targetPos - lastTargetPos;
            targetDiff.y = 0.0f;
            var targetDiffLen = Mathf.Clamp(targetDiff.magnitude, 0.0f, 5.0f);

            if (Time.deltaTime > Mathf.Epsilon)
            {
                targetVelocity = targetDiffLen / Time.deltaTime;
            }
            else
            {
                targetVelocity = 0.0f;
            }

            if (InputManager.GetInput(InputType.Move).Valid)
            {
                var runDir = (Vector2)InputManager.GetInput(InputType.Move).Value;
                runDir.Normalize();

                var followParam = config.GetFloat("FollowCoef");

                // angle between current movement and forward
                var deltaAngle = Mathf.Atan2(runDir.x, runDir.y);

                // sin(atan(...)) will clamp the results into -1,1
                var sined = Mathf.Sin(deltaAngle);

                var rt = Mathf.Clamp01(rotationInputTimeout);

                var rotDelta = sined * Time.deltaTime * followParam * targetVelocity * 0.2f * rt;

                rotX += rotDelta;
            }
        }

        private void UpdateDistance()
        {
            var newTarget = targetPos + GetOffsetPos();
            //cameraTarget = Utils.Interpolation.LerpS3(newTarget, GetTargetHeadPos(), currCollisionTargetDist);
            cameraTarget = Vector3.Lerp(newTarget, GetTargetHeadPos(), 1.0f - currCollisionTargetDist);
        }

        private void UpdateFOV()
        {
            UnityCamera.fieldOfView = config.GetFloat("FOV");
        }

        private void UpdateDir()
        {
            activateTimeout -= Time.deltaTime;

            if (activateTimeout > 0.0f)
            {
                var yRot = config.GetFloat("DefaultYRotation");
                rotY = -yRot * Mathf.Deg2Rad;
                rotX = Mathf.Atan2(Target.forward.x, Target.forward.z);
            }

            Vector3 dir;
            Utils.Math.ToCartesian(rotX, rotY, out dir);

            UpdateAutoReset(ref dir);

            UnityCamera.transform.forward = dir;

            UnityCamera.transform.position = cameraTarget - UnityCamera.transform.forward * targetDistance;
            lastTargetPos = targetPos;
        }

        private void UpdateAutoReset(ref Vector3 dir)
        {
            if (AutoReset)
            {
                resetTimeout -= Time.deltaTime;

                if (rotationInputTimeout < 0.1f)
                {
                    resetTimeout = AutoResetTimeout;
                }

                if (resetTimeout < 0.0f && hasRotated)
                {
                    var yRot = config.GetFloat("DefaultYRotation");
                    var targetRotY = -yRot * Mathf.Deg2Rad;
                    var targetRotX = Mathf.Atan2(Target.forward.x, Target.forward.z);

                    if (Mathf.Abs(targetRotX-rotX) < 0.1f && Mathf.Abs(targetRotY - rotY) < 0.1f)
                    {
                        hasRotated = false;
                    }

                    Vector3 dirTarget;
                    Utils.Math.ToCartesian(targetRotX, targetRotY, out dirTarget);

                    var to = Quaternion.FromToRotation(dir, dirTarget);
                    var rot = Quaternion.Slerp(Quaternion.identity, to, Time.deltaTime*AutoResetSpeed*10);
                    dir = rot * dir;
                }
            }
        }

        private Vector3 GetOffsetPos()
        {
            var cfgOffset = Vector3.zero;

            if (config.IsVector3("TargetOffset"))
            {
                cfgOffset = config.GetVector3("TargetOffset");
            }

            var camForwardXZ = Utils.Math.VectorXZ(UnityCamera.transform.forward);
            var camRightXZ = Utils.Math.VectorXZ(UnityCamera.transform.right);
            var camUp = Vector3.up;

            return camForwardXZ * cfgOffset.z +
                   camRightXZ * cfgOffset.x +
                   camUp * cfgOffset.y;
        }

        private void UpdateYRotation()
        {
            // change Y rotation only if there is no rotation input and target is moving
            if (!rotationInput && targetVelocity > 0.1f)
            {
                var yAngle = -rotY * Mathf.Rad2Deg;
                var targetYRot = config.GetFloat("DefaultYRotation");
                var rt = Mathf.Clamp01(rotationInputTimeout);
                var t = Mathf.Clamp01(targetVelocity * config.GetFloat("AutoYRotation") * Time.deltaTime) * rt;
                yAngle = Mathf.Lerp(yAngle, targetYRot, t);

                rotY = -yAngle * Mathf.Deg2Rad;
            }
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
                // update rotation
                //
                if (InputManager.GetInput(InputType.Reset, false))
                {
                    activateTimeout = 0.1f;
                }

                //
                // update field of view
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
                // update follow
                //
                UpdateFollow();

                //
                // update distance
                //
                UpdateDistance();

                //
                // update automatic Y rotation
                //
                UpdateYRotation();

                //
                // apply rotations to camera forward dir
                //
                UpdateDir();
            }
        }

        private void UpdateCollision()
        {
            var newTarget = targetPos + GetOffsetPos();
            var distance = config.GetFloat("Distance");

            collision.ProcessCollision(newTarget, GetTargetHeadPos(), UnityCamera.transform.forward, distance, out collisionTargetDist, out collisionDistance);

            // adjust camera target, this will move the camera closer to the character head in case of close collision
            var collisionDistanceRatio = collisionDistance / distance;

//            Utils.Debug.Log("ColDistRatio {0} colTargetDist {1}", collisionDistanceRatio, collisionTargetDist);

            if (collisionTargetDist > collisionDistanceRatio)
            {
                collisionTargetDist = collisionDistanceRatio;
            }

            // interpolate camera distance
            targetDistance = Utils.Interpolation.Lerp/*Mathf.MoveTowards*/(targetDistance, collisionDistance, targetDistance > collisionDistance ? collision.GetClipSpeed() : collision.GetReturnSpeed());

            // interpolate camera target
            currCollisionTargetDist = Mathf.SmoothDamp(currCollisionTargetDist, collisionTargetDist, ref collisionTargetVelocity,
                                                       currCollisionTargetDist > collisionTargetDist ? collision.GetTargetClipSpeed() : collision.GetReturnTargetSpeed());

//            Utils.Debug.Log("targetDistance: {0} collisionDistance: {1}", targetDistance, collisionDistance);
        }

        public override void GameUpdate()
        {
            base.GameUpdate();

//            targetFilter.AddSample(Target.position);

            var spring = config.GetFloat("Spring");
            var deadZone = config.GetVector2("DeadZone");

            if (spring <= 0.0f && deadZone.sqrMagnitude <= Mathf.Epsilon)
            {
//                targetPos = Target.position;
                targetPos = targetFilter.GetValue();
            }

            UpdateTargetDummy();
        }

        public override void FixedStepUpdate()
        {
            targetFilter.AddSample(Target.position);

            //
            // calculate camera Collision
            //
            UpdateCollision();

            //
            // calculate position in dead zone
            //

            var deadZone = config.GetVector2("DeadZone");

            if (deadZone.sqrMagnitude > Mathf.Epsilon)
            {
                // draw dead zone ring
                Utils.RingPrimitive.Generate(debugRing, deadZone.x, deadZone.y, 0.1f, 50);
                debugRing.transform.position = targetPos + Vector3.up * 2;
                var forward = Utils.Math.VectorXZ(UnityCamera.transform.forward);
                if (forward.sqrMagnitude < Mathf.Epsilon)
                {
                    forward = Vector3.forward;
                }
                debugRing.transform.forward = forward;
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
            }
        }
    }
}
