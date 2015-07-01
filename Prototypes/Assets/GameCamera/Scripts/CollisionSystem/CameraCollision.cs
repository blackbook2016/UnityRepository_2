// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections;
using RG_GameCamera.Config;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem
{
    [RequireComponent(typeof(Config.CollisionConfig))]
    public class CameraCollision : MonoBehaviour
    {
        /// <summary>
        /// singleton instance
        /// </summary>
        public static CameraCollision Instance { get; private set; }

        private Camera unityCamera;
        private Config.Config config;
        private TargetCollision targetCollision;
        private SimpleCollision simpleCollision;
        private VolumetricCollision volumetricCollision;
        private SphericalCollision sphericalCollision;
		private bool Enabled;

        void Awake()
        {
            Instance = this;
            Enabled = true;

            unityCamera = CameraManager.Instance.UnityCamera;
            Utils.Debug.Assert(unityCamera);
            config = GetComponent<CollisionConfig>();

            targetCollision = new TargetCollision(config);
            simpleCollision = new SimpleCollision(config);
            sphericalCollision = new SphericalCollision(config);
            volumetricCollision = new VolumetricCollision(config);
        }

        void Start()
        {
            unityCamera.nearClipPlane = config.GetFloat("NearClipPlane");
        }

        private ViewCollision GetCollisionAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "Simple":
                    return simpleCollision;

                case "Spherical":
                    return sphericalCollision;

                case "Volumetric":
                    return volumetricCollision;
            }

            Utils.Debug.Assert(false);
            return null;
        }

		public void Enable(bool status)
		{
            Enabled = status;
		}

        /// <summary>
        /// process camera collision, calculate collision of camera target and camera distance
        /// </summary>
        /// <param name="cameraTarget">current camera target</param>
        /// <param name="targetHead">position of character head</param>
        /// <param name="dir">direction of camera view</param>
        /// <param name="distance">length of camera view vector</param>
        /// <param name="collisionTarget">calculated camera target outside of collision</param>
        /// <param name="collisionDistance">calculated camera view vector oustide of collision</param>
        public void ProcessCollision(Vector3 cameraTarget, Vector3 targetHead, Vector3 dir, float distance, out float collisionTarget, out float collisionDistance)
        {
            if (!Enabled)
			{
				collisionTarget = 1.0f;
				collisionDistance = distance;
			}
			else
			{
				// calculate target position
				collisionTarget = targetCollision.CalculateTarget(targetHead, cameraTarget);
				
				// choose collision algorithm
				var viewCollision = GetCollisionAlgorithm(config.GetSelection("CollisionAlgorithm"));
				
				// calculate view distance
				var currTargetPos = cameraTarget*collisionTarget + targetHead*(1.0f - collisionTarget);
				collisionDistance = viewCollision.Process(currTargetPos, dir, distance);
			}
        }

        public float GetRaycastTolerance()
        {
            return config.GetFloat("RaycastTolerance");
        }

        public float GetClipSpeed()
        {
            return config.GetFloat("ClipSpeed");
        }

        public float GetTargetClipSpeed()
        {
            return config.GetFloat("TargetClipSpeed");
        }

        public float GetReturnSpeed()
        {
            return config.GetFloat("ReturnSpeed");
        }

        public float GetReturnTargetSpeed()
        {
            return config.GetFloat("ReturnTargetSpeed");
        }

        public float GetHeadOffset()
        {
            return config.GetFloat("HeadOffset");
        }

        public ViewCollision.CollisionClass GetCollisionClass(Collider coll)
        {
            var dontClipTag = config.GetString("IgnoreCollisionTag");
            var transparentClipTag = config.GetString("TransparentCollisionTag");
            return ViewCollision.GetCollisionClass(coll, dontClipTag, transparentClipTag);
        }
    }
}
