// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Effects
{
    public class Yes : Effect
    {
        public float Angle = 1f;
        public float Speed = 10.0f;

        private float diff;
        private float size;
        private Vector3 origPos;
        private Vector3 currPos;

        public override void OnPlay()
        {
            diff = 0.0f;
            origPos = unityCamera.transform.position;
        }

        public override void OnUpdate()
        {
            var rot = unityCamera.transform.rotation.eulerAngles;

            switch (fadeState)
            {
                case FadeState.FadeIn:
                    size = Utils.Interpolation.LerpS3(0.0f, Angle, 1.0f - fadeInNormalized);
                    currPos = origPos;
                    break;

                case FadeState.FadeOut:
                    size = Utils.Interpolation.LerpS2(Angle, 0.0f, fadeOutNormalized);
                    currPos = Utils.Interpolation.LerpS2(origPos, unityCamera.transform.position, fadeOutNormalized);
                    break;

                case FadeState.Full:
                    size = Angle;
                    currPos = origPos;
                    break;
            }

            var a = Mathf.Sin(timeout*Speed)*size;
            var newX = rot.x - diff + a;
            diff = a;
            var newRot = rot;
            newRot.x = newX;

            unityCamera.transform.position = currPos;
            unityCamera.transform.rotation = Quaternion.Euler(newRot);
        }
    }
}
