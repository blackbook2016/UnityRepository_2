// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Modes
{
    /// <summary>
    /// Dead camera - simple camera mode rotating around target (dead character)
    /// </summary>
    [RequireComponent(typeof(Config.EmptyConfig))]
    public class EmptyCameraMode : CameraMode
    {
        public override Type Type
        {
            get { return Type.None; }
        }

        public override void Init()
        {
            base.Init();

            UnityCamera.transform.LookAt(cameraTarget);

            config = GetComponent<Config.EmptyConfig>();
        }
    }
}
