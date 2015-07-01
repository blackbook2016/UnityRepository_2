// uncomment next line to work with Playmaker
//#define PLAYMAKER
#if PLAYMAKER

// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [Tooltip("Set camera target")]
    public class GameCameraSetTarget : FsmStateAction
    {
        [Tooltip("Type of game camera")]
        public FsmGameObject CameraTarget;

        public override void OnEnter()
        {
            var cm = RG_GameCamera.CameraManager.Instance;
            var input = RG_GameCamera.Input.InputManager.Instance;

            if (cm != null && input != null)
            {
                cm.CameraTarget = CameraTarget.Value.transform;
            }
        }
    }
}

#endif
