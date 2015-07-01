// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using RG_GameCamera.Effects;
using RG_GameCamera.Input;
using RG_GameCamera.Modes;
using UnityEngine;
using Type = RG_GameCamera.Modes.Type;

namespace RG_GameCamera
{
    /// <summary>
    /// master camera manager, responsible for setting and changing camera modes
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = Utils.CameraInstance.CreateInstance<CameraManager>("CameraManager");
                }
                return instance;
            }
        }

        /// <summary>
        /// unity camera object
        /// </summary>
        public Camera UnityCamera;

        /// <summary>
        /// transition speed of changing between camera modes
        /// </summary>
        public float TransitionSpeed = 0.5f;

        /// <summary>
        /// maximum time spend to interpolate between camera modes
        /// </summary>
        public float TransitionTimeMax = 1.0f;

        /// <summary>
        /// callback for finishing the transition
        /// </summary>
        public delegate void OnTransitionFinished();

        /// <summary>
        /// unity skin for fancier look
        /// </summary>
        public GUISkin GuiSkin;

        /// <summary>
        /// activate this camera mode on start
        /// </summary>
        public Modes.Type ActivateModeOnStart;

        /// <summary>
        /// use this variable to define target to follow (like character controller)
        /// </summary>
        public Transform CameraTarget;

        /// <summary>
        /// register camera mode, every camera mode register self on awake
        /// </summary>
        /// <param name="cameraMode">camera mode</param>
        public void RegisterMode(Modes.CameraMode cameraMode)
        {
//            Utils.Debug.Log("RegisterMode {0} {1}", cameraMode, cameraMode.Type);
            cameraModes.Add(cameraMode.Type, cameraMode);
            cameraMode.gameObject.SetActive(false);
        }

        /// <summary>
        /// set new camera mode and interpolate from the old one
        /// </summary>
        /// <param name="cameraMode">new camera mode</param>
        public CameraMode SetMode(Modes.Type cameraMode)
        {
            if (currModeType != cameraMode)
            {
                cameraModes[currModeType].OnDeactivate();
                Utils.Debug.SetActive(cameraModes[currModeType].gameObject, false);
                oldModeTransform = new CameraTransform(UnityCamera);

                if (currModeType != Type.None)
                {
                    transition = true;
                }

                currModeType = cameraMode;

                Utils.Debug.Assert(cameraModes.ContainsKey(cameraMode), "Invalid camera type: " + cameraMode + " " + cameraModes.Count);
                Utils.Debug.SetActive(cameraModes[currModeType].gameObject, true);
                cameraModes[currModeType].SetCameraTarget(CameraTarget);
                cameraModes[currModeType].OnActivate();
            }

            return cameraModes[currModeType];
        }

        /// <summary>
        /// set default camera mode configuration
        /// </summary>
        /// <param name="cameraMode">type of camera mode</param>
        /// <param name="configuration">name of configuration</param>
        public void SetDefaultConfiguration(Modes.Type cameraMode, string configuration)
        {
            cameraModes[cameraMode].DefaultConfiguration = configuration;
        }

        /// <summary>
        /// set camera target to active camera mode
        /// </summary>
        /// <param name="target">transform game object</param>
        public void SetCameraTarget(Transform target)
        {
            CameraTarget = target;
            cameraModes[currModeType].SetCameraTarget(target);
        }

        /// <summary>
        /// returns current camera mode
        /// </summary>
        /// <returns>current camera mode</returns>
        public CameraMode GetCameraMode()
        {
            if (cameraModes != null && cameraModes.ContainsKey(currModeType))
            {
                return cameraModes[currModeType];
            }

            return null;
        }

        public void RegisterTransitionCallback(OnTransitionFinished callback)
        {
            finishedCallbak += callback;
        }

        public void UnregisterTransitionCallback(OnTransitionFinished callback)
        {
            finishedCallbak -= callback;
        }

        private static CameraManager instance;
        private Dictionary<Modes.Type, CameraMode> cameraModes;
        private bool transition;
        private Modes.Type currModeType;
        private CameraTransform oldModeTransform;
        private OnTransitionFinished finishedCallbak;

        private struct CameraTransform
        {
            Vector3 pos;
            Quaternion rot;
            float fov;

            private Vector3 posVel;
            private Vector3 rotVel;
            private float fovVel;

            private float timeout;
            private float speedRatio;

            public CameraTransform(Camera cam)
            {
                pos = cam.transform.position;
                rot = cam.transform.rotation;
                fov = cam.fieldOfView;
                posVel = Vector3.zero;
                rotVel = Vector3.zero;
                fovVel = 0.0f;
                timeout = 0.0f;
                speedRatio = 1.0f;
            }

            public bool Interpolate(Camera cam, float speed, float timeMax)
            {
                var speedr = speed*speedRatio;

                pos = Vector3.SmoothDamp(pos, cam.transform.position, ref posVel, speedr);
                rot = Quaternion.Euler(Vector3.SmoothDamp(rot.eulerAngles, cam.transform.eulerAngles, ref rotVel, speedr));
                Utils.Math.CorrectRotationUp(ref rot);
                fov = Mathf.SmoothDamp(fov, cam.fieldOfView, ref fovVel, 0.05f);

                const float epsylon = 0.001f;
                var bellowTolerance = (cam.transform.position - pos).sqrMagnitude < epsylon &&
                                      Quaternion.Angle(cam.transform.rotation, rot) < epsylon &&
                                      Mathf.Abs(fov - cam.fieldOfView) < epsylon;

                timeout += Time.deltaTime;
                speedRatio = 1.0f - Mathf.Clamp01(timeout/timeMax);

                cam.transform.position = pos;
                cam.transform.rotation = rot;
                cam.fieldOfView = fov;

                return !bellowTolerance;
            }
        }

        void Awake()
        {
            if (CameraTarget == null)
            {
                UnityEngine.Debug.LogWarning("Empty CameraTarget! Creating dummy one...");

                var dummy = new GameObject("DummyTarget");
                CameraTarget = dummy.transform;
            }

            instance = this;
            cameraModes = new Dictionary<Modes.Type, Modes.CameraMode>();
            currModeType = Modes.Type.None;

            if (!UnityCamera)
            {
                UnityCamera = GetComponent<Camera>();

                if (!UnityCamera)
                {
                    UnityCamera = UnityEngine.Camera.main;
                }
            }

            Utils.Debug.Assert(UnityCamera != null, "Missing unity camera!");

            //
            // search for camera modes and register all of them
            //
            var parent = gameObject.transform.parent;

            for (var i=0; i<parent.childCount; i++)
            {
                var child = parent.GetChild(i);

                if (child)
                {
                    var cameraMode = child.GetComponent<CameraMode>();

                    if (cameraMode)
                    {
                        RegisterMode(cameraMode);
                    }
                }
            }

            Initialize();
            SetMode(ActivateModeOnStart);
        }

        void Initialize()
        {
            // initialize all camera modes
            foreach (var cameraMode in cameraModes)
            {
                cameraMode.Value.Init();
            }
        }

        void Update()
        {
            //
            // update input
            //
            RG_GameCamera.Input.InputManager.Instance.GameUpdate();

            //
            // update camera mode
            //
            cameraModes[currModeType].GameUpdate();
        }

        void LateUpdate()
        {
            cameraModes[currModeType].PostUpdate();

            if (transition)
            {
//              Utils.Debug.Log("Transiting");
                transition = oldModeTransform.Interpolate(UnityCamera, TransitionSpeed, TransitionTimeMax);

                if (!transition)
                {
                    if (finishedCallbak != null)
                    {
                        finishedCallbak();
                    }
                }
            }

            //
            // update effect manager
            //
            if (EffectManager.Instance)
            {
                EffectManager.Instance.PostUpdate();
            }
        }

        void FixedUpdate()
        {
            cameraModes[currModeType].FixedStepUpdate();
        }
    }
}
