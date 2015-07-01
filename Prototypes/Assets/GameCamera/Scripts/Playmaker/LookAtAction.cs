// uncomment next line to work with Playmaker
//#define PLAYMAKER
#if PLAYMAKER

// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [Tooltip("Change camera to LookAt mode")]
    public class GameCameraLookAt : FsmStateAction
    {
        [Tooltip("Position of the camera eye")]
        public FsmGameObject From;

        [Tooltip("Position of the camera target")]
        public FsmGameObject Target;

        [Tooltip("Timeout to change the camera")]
        public FsmFloat Timeout;

        public override void OnEnter()
        {
            var lookAt = RG_GameCamera.CameraManager.Instance.SetMode(RG_GameCamera.Modes.Type.LookAt) as RG_GameCamera.Modes.LookAtCameraMode;

            if (lookAt != null)
            {
                if (From.Value && Target.Value)
                {
                    lookAt.LookAt(From.Value.transform.position, Target.Value.transform.position, Timeout.Value);
                }
                else if (From.Value)
                {
                    lookAt.LookFrom(From.Value.transform.position, Timeout.Value);
                }
                else if (Target.Value)
                {
                    lookAt.LookAt(Target.Value.transform.position, Timeout.Value);
                }

                // register look at callback
                lookAt.RegisterFinishCallback(OnLookAtFinished);
            }
            else
            {
                OnLookAtFinished();
            }
        }

        void OnLookAtFinished()
        {
            Finish();

            var lookAt = RG_GameCamera.CameraManager.Instance.SetMode(RG_GameCamera.Modes.Type.LookAt) as RG_GameCamera.Modes.LookAtCameraMode;

            if (lookAt)
            {
                lookAt.UnregisterFinishCallback(OnLookAtFinished);
            }
        }
    }
}

#endif
