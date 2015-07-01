// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.Effects;
using RG_GameCamera.Extras;
using RG_GameCamera.Input;
using UnityEngine;
using System.Collections;
using Type = RG_GameCamera.Modes.Type;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// player is used for direct manipulation of human entity
    /// it is controlled directly by keyboard/gamepad and it is used for demonstration of 3rd person camera
    /// </summary>
    public class Player : Human
    {
        public bool WalkByDefault = false;
        public bool LookInCameraDirection = true;
        public bool EnableCrosshairAim = true;
        public bool AutomaticShoot = false;
        public bool AutoInput = true;

        private Vector3 lookPos;
        private Transform cam;
        private Vector3 camForward;
        private Vector3 move;
        private InputManager inputManager;
        private InputSource inputSource;
        private MoveState state;
        private Vector3 lastWaypoint;

        private enum MoveState 
        {
            Default,
            Aim,
            Crouch,
            Sprint,
        }

        public enum InputSource
        {
            /// <summary>
            /// ai input is used for RTS and RPG cameras
            /// </summary>
            AI,

            /// <summary>
            /// direct input is used for ThirdPerson camera
            /// </summary>
            Direct,

            /// <summary>
            /// not determined input type
            /// </summary>
            Invalid,
        }

        /// <summary>
        /// select input type
        /// </summary>
        /// <param name="input">input type</param>
        public void SetInput(InputSource input)
        {
            if (input != inputSource)
            {
                inputSource = input;

                switch (input)
                {
                    case InputSource.AI:
                        gameObject.GetComponent<AIController>().Activate(true);
                        EnableCrosshairAim = false;
                        break;

                    case InputSource.Direct:
                        gameObject.GetComponent<AIController>().Activate(false);
                        EnableCrosshairAim = true;
                        break;
                }
            }
        }

        /// <summary>
        /// switch input preset based on camera mode
        /// </summary>
        void DetermineInputBasedOnCameraMode()
        {
            var modeType = CameraManager.Instance.GetCameraMode().Type;

            switch (modeType)
            {
                case Modes.Type.ThirdPerson:
                    SetInput(InputSource.Direct);
                    RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.ThirdPerson);
                    break;

                case Modes.Type.FPS:
                    SetInput(InputSource.Direct);
                    RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.FPS);
                    break;

                case Modes.Type.RPG:
                    SetInput(InputSource.AI);
                    RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.RPG);
                    break;

                case Modes.Type.RTS:
                    SetInput(InputSource.AI);
                    RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.RTS);
                    break;

                case Modes.Type.Orbit:
                    SetInput(InputSource.AI);
                    RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.Orbit);
                    break;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            EntityManager.Instance.RegisterPlayer(this);
        }

        protected override void Start()
        {
            cam = CameraManager.Instance.UnityCamera.transform;
            inputManager = RG_GameCamera.Input.InputManager.Instance;
            Utils.Debug.Assert(inputManager, "Missing camera input!");

            weaponController.InitWeapon(gameObject, "M4MB");
            weaponController.Hold();

            animController.OnFallImpactCallback = OnFallImpact;

            Health = 100.0f;

            state = MoveState.Default;
            inputSource = InputSource.Invalid;
        }

        protected override void Update()
        {
            if (!inputManager.IsValid)
                return;

            if (Extras.HealthBar.Instance)
            {
                Extras.HealthBar.Instance.SetHealth(Health);
            }
        }

        private void OnFallImpact(float velocity)
        {
            if (UnityEngine.Time.timeSinceLevelLoad > 5)
            {
                var fallEffect = EffectManager.Instance.Create<Fall>();
                fallEffect.ImpactVelocity = velocity;
                fallEffect.Play();
            }
        }

        protected override void FixedUpdate()
        {
            //
            // in case of multiplayer remote player we can ignore direct input
            //
            if (Remote)
            {
                return;
            }

            //
            // this will determine correct camera input based on camera mode
            // NOTE: this function should be called on Start however we leave it here
            // to demonstrate real-time changing of camera/game modes
            //
            if (AutoInput)
            {
                DetermineInputBasedOnCameraMode();
            }

            if (!inputManager.IsValid || inputSource != InputSource.Direct)
                return;

            // read inputs
            var crouch = inputManager.GetInput(InputType.Crouch, false);
            var moveInput = inputManager.GetInput(InputType.Move, Vector2.zero);
            var sprint = inputManager.GetInput(InputType.Sprint, false);
            var reset = inputManager.GetInput(InputType.Reset, false);

            camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
            move = moveInput.y * camForward + moveInput.x * cam.right;

            if (move.magnitude > 1) move.Normalize();

            var walkToggle = inputManager.GetInput(InputType.Walk, false);
            var walkMultiplier = (WalkByDefault ? walkToggle ? 1 : 0.5f : walkToggle ? 0.5f : 1);
            move *= walkMultiplier;
            sprint &= move.sqrMagnitude > 0.5f;

            if (sprint)
            {
                move *= 1.5f;
            }

            // when player dies set dead camera
            if (IsDead)
            {
                CameraManager.Instance.SetMode(Modes.Type.Dead);
            }

            // reset camera back to 3rd on reset
            if (reset)
            {
                CameraManager.Instance.SetMode(Modes.Type.ThirdPerson);
                Resurect();
            }

            var aim = inputManager.GetInput(InputType.Aim, false) && !IsDead;
            var shoot = inputManager.GetInput(InputType.Fire, false) && !IsDead;
            var jump = inputManager.GetInput(InputType.Jump, false);

            if (AutomaticShoot && !IsDead)
            {
                if (TargetManager.Instance && TargetManager.Instance.TargetType == TargetType.Enemy)
                {
                    aim = true;
                    shoot = true;
                }
            }

            var mode = CameraManager.Instance.GetCameraMode();
            var oldState = state;

            if (sprint)
            {
                if (state == MoveState.Default)
                {
                    state = MoveState.Sprint;
                }
            }
            else if (aim)
            {
                if (state == MoveState.Default)
                {
                    state = MoveState.Aim;
                }
            }
            else if (crouch)
            {
                if (state == MoveState.Default)
                {
                    state = MoveState.Crouch;
                }
            }
            else
            {
                state = MoveState.Default;
            }

            if (oldState != state)
            {
                switch (state)
                {
                    case MoveState.Default:
                        mode.SetCameraConfigMode("Default");
                        break;

                    case MoveState.Crouch:
                        mode.SetCameraConfigMode("Crouch");
                        break;

                    case MoveState.Aim:
                        mode.SetCameraConfigMode("Aim");
                        break;

                    case MoveState.Sprint:
                        mode.SetCameraConfigMode("Sprint");

                        // create sprint shake effect
                        var effect = EffectManager.Instance.Create(Effects.Type.SprintShake);
                        effect.Loop = true;
                        effect.Play();
                        break;
                }

                if (oldState == MoveState.Sprint)
                {
                    var effect = EffectManager.Instance.Create(Effects.Type.SprintShake);
                    effect.Stop();
                }
            }

            Aim(state == MoveState.Aim);

            // calculate the head look target position
            lookPos = LookInCameraDirection && cam != null
                          ? transform.position + cam.forward * 100
                          : transform.position + transform.forward * 100;

            UnityEngine.Debug.DrawRay(transform.position, (lookPos - transform.position) * 100, Color.red);

            // pass all parameters to the animController control script
            Move(new AnimationController.Input
            {
                aim = state == MoveState.Aim,
                camMove = move,
                crouch = state == MoveState.Crouch,
                inputMove = moveInput,
                jump = jump,
                lookPos = lookPos,
                die = IsDead,
                reset = reset,
                smoothAimRotation = false,
                aimTurn = false,
            });

            if (shoot)
            {
                Shoot();
            }

            if (TargetManager.Instance)
            {
                TargetManager.Instance.HideCrosshair = !(EnableCrosshairAim && state == MoveState.Aim);
            }
        }
    }
}
