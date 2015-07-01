// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Effects
{
    public class SprintShake : Effect
    {
        public float Size = 1f;
        public float Speed = 10.0f;

        private Vector3 diff;
        private float size;

        public override void OnPlay()
        {
            diff = Vector2.zero;
        }

        public override void OnUpdate()
        {
            var rot = unityCamera.transform.rotation.eulerAngles;

            switch (fadeState)
            {
                case FadeState.FadeIn:
                    size = Utils.Interpolation.LerpS3(0.0f, Size, 1.0f - fadeInNormalized);
                    break;

                case FadeState.FadeOut:
                    size = Utils.Interpolation.LerpS2(Size, 0.0f, fadeOutNormalized);
                    break;

                case FadeState.Full:
                    size = Size;
                    break;
            }

            var v0 = SmoothRandom.GetVector3(Speed) * size;
            var newRot = rot - diff + v0;
            diff = v0;

            unityCamera.transform.rotation = Quaternion.Euler(newRot);
        }
    }
}
