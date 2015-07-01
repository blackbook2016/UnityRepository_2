// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Effects
{
    public class FireKick : Effect
    {
        public float KickTime;
        public float KickAngle;

        private float diff;
        private float kickTimeout;

        public override void OnPlay()
        {
            diff = 0.0f;
            KickTime = Mathf.Clamp(KickTime, 0.0f, Length);
        }

        public override void OnUpdate()
        {
            var rot = unityCamera.transform.rotation.eulerAngles;
            var angle = 0.0f;

            if (timeout < KickTime)
            {
                var t = timeout/KickTime;
                angle = Utils.Interpolation.LerpS2(0.0f, KickAngle, t);
            }
            else
            {
                var t = (timeout - KickTime) / (Length - KickTime);
                angle = Utils.Interpolation.LerpS(KickAngle, 0.0f, t);
            }

            angle = -angle;

            var newX = rot.x - diff + angle;
            diff = angle;
            var newRot = rot;
            newRot.x = newX;

            unityCamera.transform.rotation = Quaternion.Euler(newRot);
        }
    }
}
