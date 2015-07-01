// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

#define DBG
#define PROFILING

using RG_GameCamera.CollisionSystem;
using RG_GameCamera.Input;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Modes
{
    public enum Type
    {
        ThirdPerson,
        RTS,
        RPG,
        LookAt,
        Orbit,
        Dead,
        FPS,
        Debug,
        None,
    }

    public abstract class CameraMode : MonoBehaviour
    {
        /// <summary>
        /// name of the camera mode
        /// </summary>
        public abstract Type Type { get; }

        /// <summary>
        /// camera target
        /// </summary>
        public Transform Target;

        /// <summary>
        /// show target as a purple sphere
        /// </summary>
        public bool ShowTargetDummy = false;

        /// <summary>
        /// enable live config GUI inside game window
        /// </summary>
        public bool EnableLiveGUI = false;

        /// <summary>
        /// return current configuration of this camera mode
        /// </summary>
        public Config.Config Configuration { get { return config; } }

		/// <summary>
		/// default configuration for camera
		/// </summary>
		public string DefaultConfiguration = "Default";

        /// <summary>
        /// camera collision system
        /// </summary>
        protected CameraCollision collision;
        protected InputManager InputManager;
        protected Camera UnityCamera;
        protected Config.Config config;
        protected Vector3 cameraTarget;
        protected float targetDistance;
        protected bool disableInput;
        private GameObject targetDummy;

        /// <summary>
        /// initialize camera mode (called from camera manager)
        /// </summary>
        public virtual void Init()
        {
            var cm = CameraManager.Instance;
            UnityCamera = cm.UnityCamera;
            InputManager = RG_GameCamera.Input.InputManager.Instance;

            Utils.Debug.Assert(InputManager);

            if (!Target)
            {
                Target = cm.CameraTarget;
            }

            if (Target)
            {
                cameraTarget = Target.position;
                targetDistance = (UnityCamera.transform.position - Target.position).magnitude;
            }

            CreateTargetDummy();

            collision = CameraCollision.Instance;

            Utils.Debug.Assert(Target);
        }

        /// <summary>
        /// event when the camera mode gets activated from camera manager
        /// </summary>
        public virtual void OnActivate()
        {
        }

        /// <summary>
        /// event when the camera mode gets deactivated from camera manager
        /// </summary>
        public virtual void OnDeactivate()
        {
        }

        /// <summary>
        /// set camera target to follow
        /// </summary>
        /// <param name="target"></param>
        public virtual void SetCameraTarget(Transform target)
        {
            Target = target;
        }

        /// <summary>
        /// set camera config by mode, for example: "Aim", "Default", "Crouch"
        /// </summary>
        /// <param name="modeName">mode name</param>
        public virtual void SetCameraConfigMode(string modeName)
        {
            config.SetCameraMode(modeName);
        }

		/// <summary>
		/// Enables the collision.
		/// </summary>
		/// <param name="status">If set to <c>true</c> status.</param>
		public void EnableCollision(bool status)
		{
			if (collision)
			{
				collision.Enable(status);
			}
		}

		/// <summary>
		/// reset camera to its original settings
		/// </summary>
		public virtual void Reset()
		{
		}

        /// <summary>
        /// enable orthographic camera
        /// </summary>
        /// <param name="status"></param>
        public void EnableOrthoCamera(bool status)
        {
            if (status == UnityCamera.orthographic)
                return;

            if (status)
            {
                UnityCamera.orthographic = true;
                UnityCamera.orthographicSize = (UnityCamera.transform.position - cameraTarget).magnitude / 2;
            }
            else
            {
                UnityCamera.orthographic = false;
                UnityCamera.transform.position = cameraTarget -
                                                UnityCamera.transform.forward * UnityCamera.orthographicSize * 2;
            }
        }

        public bool IsOrthoCamera()
        {
            return UnityCamera.orthographic;
        }

        public void CreateTargetDummy()
        {
            targetDummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            targetDummy.name = "TargetDummy";
            targetDummy.transform.parent = gameObject.transform;
            var dummyCollider = targetDummy.GetComponent<SphereCollider>();
            if (dummyCollider)
            {
                Destroy(dummyCollider);
            }
            var targetMaterial = new Material(Shader.Find("Diffuse"));
            targetMaterial.color = Color.magenta;
            targetDummy.GetComponent<MeshRenderer>().sharedMaterial = targetMaterial;
            targetDummy.transform.position = cameraTarget;
            targetDummy.SetActive(ShowTargetDummy);
        }

        protected Vector3 GetTargetHeadPos()
        {
            var offset = collision.GetHeadOffset();

            var crouch = InputManager.GetInput(InputType.Crouch);
            if (crouch.Valid && (bool)crouch.Value)
            {
                offset = 1.2f;
            }

            if (Target)
            {
                return Target.position + Vector3.up * offset;
            }
            else
            {
                return cameraTarget + Vector3.up * offset;
            }
        }

        protected void UpdateTargetDummy()
        {
            Utils.Debug.SetActive(targetDummy, ShowTargetDummy);

            if (targetDummy)
            {
                var cameraDistance = (UnityCamera.transform.position - targetDummy.transform.position).magnitude;

                if (cameraDistance > 70)
                {
                    cameraDistance = 70;
                }

                var ratio = cameraDistance / 70;

                targetDummy.transform.localScale = new Vector3(ratio, ratio, ratio);

                targetDummy.transform.position = cameraTarget;
            }
        }

        public virtual void GameUpdate()
        {
#if DBG
            if (UnityInput.GetKeyDown(KeyCode.O))
            {
                EnableOrthoCamera(!UnityCamera.orthographic);
            }
#endif

            UpdateTargetDummy();

            if (config)
            {
                config.EnableLiveGUI(EnableLiveGUI);
            }

            if (config.IsBool("Orthographic"))
            {
                EnableOrthoCamera(config.GetBool("Orthographic"));
            }
        }

        public virtual void FixedStepUpdate()
        {
        }

        public virtual void PostUpdate()
        {
        }

        protected float GetZoomFactor()
        {
            var distance = 1.0f;

            if (UnityCamera.orthographic)
            {
                distance = UnityCamera.orthographicSize;
            }
            else
            {
                distance = (UnityCamera.transform.position - cameraTarget).magnitude;
            }

            if (distance > 1)
            {
                return (distance) / (1.0f + Mathf.Log(distance));
            }

            return distance;
        }

        protected void DebugDraw()
        {
            Debug.DrawLine(UnityCamera.transform.position, cameraTarget, Color.red, 1);
            Debug.DrawRay(cameraTarget, UnityCamera.transform.up, Color.green, 1);
            Debug.DrawRay(cameraTarget, UnityCamera.transform.right, Color.yellow, 1);
        }

#if DBG
        void OnGUI()
        {
#if PROFILING
            var results = Utils.Profiler.GetResults();
            var y = 10;
            var x = Screen.width - 300;

            foreach (var result in results)
            {
                GUI.Label(new Rect(x, y, 500, 30), result);
                y += 20;
            }
#endif
        }
#endif
    }
}
