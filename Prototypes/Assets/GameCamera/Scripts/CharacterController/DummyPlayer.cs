// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

//#define USING_NAVMESH

using RG_GameCamera.Input;
using RG_GameCamera.Modes;
using UnityEngine;
using System.Collections;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// dummy player is an example of very basic character controller without animations (just moving capsule)
    /// it communicates with camera input manager to move around the scene
    /// there are three main updating method:
    /// * for third person camera UpdateThirdPersonController()
    /// * for RTS camera UpdateRTS()
    /// * for RPG camera UpdateRPG()
    /// </summary>
#if USING_NAVMESH
    [RequireComponent(typeof (NavMeshAgent))]
#endif
    public class DummyPlayer : MonoBehaviour
    {
        private Transform cam;
        private InputManager inputManager;
        private Vector3 targetPos;

#if USING_NAVMESH
        private NavMeshAgent agent;
#endif

        void Start()
        {
            cam = CameraManager.Instance.UnityCamera.transform;
            inputManager = RG_GameCamera.Input.InputManager.Instance;
            targetPos = transform.position;

#if USING_NAVMESH
            agent = GetComponent<NavMeshAgent>();
#endif
        }

        /// <summary>
        /// main update method for the dummy player with ThirdPersonCamera
        /// </summary>
        void UpdateThirdPersonController()
        {
            //
            // make sure we are using correct input manager preset
            //
            inputManager.SetInputPreset(InputPreset.ThirdPerson);

#if USING_NAVMESH
            agent.enabled = false;
#endif

            var moveInput = inputManager.GetInput(InputType.Move, Vector2.zero);
            var sprint = inputManager.GetInput(InputType.Sprint, false);

            var camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            var move = moveInput.y * camForward + moveInput.x * cam.right;

            if (move.magnitude > 1) move.Normalize();

            var walkToggle = inputManager.GetInput(InputType.Walk).Valid && (bool)inputManager.GetInput(InputType.Walk).Value;
            var walkMultiplier = walkToggle ? 0.5f : 1;
            move *= walkMultiplier;
            sprint &= move.sqrMagnitude > 0.5f;

            if (sprint)
            {
                move *= 1.5f;
            }

            var aim = inputManager.GetInput(InputType.Aim, false);

            // update camera mode config by state
            var mode = CameraManager.Instance.GetCameraMode();

            // activate sprint camera mode
            if (sprint)
            {
                mode.SetCameraConfigMode("Sprint");
            }
            // activate aim camera mode
            else if (aim)
            {
                mode.SetCameraConfigMode("Aim");
            }
            // activate default camera mode
            else
            {
                mode.SetCameraConfigMode("Default");
            }

            //
            // move capsule
            //
            transform.position += move*0.1f;

            if (move.sqrMagnitude > 0)
            {
                transform.forward = move.normalized;
            }

            // rotate the capsule in aim mode
            if (aim)
            {
                transform.forward = camForward;
            }
        }

        /// <summary>
        /// main update method for the dummy player with RPG camera (same as RTS)
        /// </summary>
        void UpdateRPG()
        {
            //
            // make sure we are using correct input manager preset
            //
            inputManager.SetInputPreset(InputPreset.RPG);

#if USING_NAVMESH
            agent.enabled = true;
#endif

            // get waypoint position
            var waypoint = inputManager.GetInput(InputType.WaypointPos);

            if (waypoint.Valid)
            {
                targetPos = (Vector3) waypoint.Value;
            }

            if ((transform.position - targetPos).sqrMagnitude > 1.0f)
            {
                var v0 = (-(transform.position - targetPos))*1.0f;
                transform.forward = v0;

#if USING_NAVMESH
                // this will move capsule to desired waypoint position
                agent.SetDestination(targetPos);
#else
                transform.position += v0*Time.deltaTime;
#endif
            }

            GetComponent<Rigidbody>().position = transform.position;
        }

        /// <summary>
        /// main update method for the dummy player with RTS camera (same as RPG)
        /// </summary>
        void UpdateRTS()
        {
            //
            // make sure we are using correct input manager preset
            //
            inputManager.SetInputPreset(InputPreset.RTS);

            UpdateRPG();
        }

        /// <summary>
        /// this is the update function from Unity called every frame
        /// in this method we update the dummy capsule based on camera mode
        /// </summary>
        void FixedUpdate()
        {
            if (!inputManager.IsValid)
                return;

            // get camera manager
            var cameraManager = CameraManager.Instance;

            // get camera mode
            var cameraMode = cameraManager.GetCameraMode();

            //
            // we use different update method for different camera mode
            //
            switch (cameraMode.Type)
            {
                case Type.ThirdPerson:
                    UpdateThirdPersonController();
                    break;

                case Type.RPG:
                    UpdateRPG();
                    break;

                case Type.RTS:
                    UpdateRTS();
                    break;
            }
        }
    }
}
