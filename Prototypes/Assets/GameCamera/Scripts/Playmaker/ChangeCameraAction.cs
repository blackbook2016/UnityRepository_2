// uncomment next line to work with Playmaker
//#define PLAYMAKER
#if PLAYMAKER

// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using RG_GameCamera.Input;
using RG_GameCamera.Modes;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [Tooltip("Change camera to different mode")]
    public class GameCameraChangeMode : FsmStateAction
    {
        [Tooltip("Type of game camera")]
        public RG_GameCamera.Modes.Type CameraMode;

        [Tooltip("Transition speed of the camera")]
        public FsmFloat TransitionSpeed = 0.5f;

        [Tooltip("Maximum time of transition")] 
        public FsmFloat TransitionTimeMax = 1.0f;

        public override void OnEnter()
        {
            var cm = RG_GameCamera.CameraManager.Instance;
            var input = RG_GameCamera.Input.InputManager.Instance;

            if (cm != null && input != null)
            {
                cm.TransitionSpeed = TransitionSpeed.Value;
                cm.TransitionTimeMax = TransitionTimeMax.Value;

                cm.RegisterTransitionCallback(OnTransitionFinished);

                switch (CameraMode)
                {
                    case Type.ThirdPerson:
                        cm.SetMode(CameraMode);
                        input.SetInputPreset(InputPreset.ThirdPerson);
                        break;

                    case Type.RTS:
                        cm.SetMode(CameraMode);
                        input.SetInputPreset(InputPreset.RTS);
                        break;

                    case Type.RPG:
                        cm.SetMode(CameraMode);
                        input.SetInputPreset(InputPreset.RPG);
                        break;

                    case Type.Orbit:
                        cm.SetMode(CameraMode);
                        input.SetInputPreset(InputPreset.Orbit);
                        break;

                    case Type.FPS:
                        cm.SetMode(CameraMode);
                        input.SetInputPreset(InputPreset.FPS);
                        break;

                    case Type.None:
                        cm.SetMode(CameraMode);
                        OnTransitionFinished();
                        break;

                    case Type.Dead:
                        cm.SetMode(CameraMode);
                        break;

                    default:
                        OnTransitionFinished();
                        break;
                }
            }
        }

        void OnTransitionFinished()
        {
            Finish();

            var cm = RG_GameCamera.CameraManager.Instance;

            cm.UnregisterTransitionCallback(OnTransitionFinished);
        }
    }
}

#endif
