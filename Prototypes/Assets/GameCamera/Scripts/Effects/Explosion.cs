// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Effects
{
    public class Explosion : Effect
    {
        public float Mass;
        public float Distance;
        public float Strength;
        public float Damping;
        public Vector3 position;
        public float Size = 1f;
        public float Speed = 10.0f;

        private float size;
        private Spring posSpring;
        private Vector3 v0;
        private Vector3 diff;

        public override void Init()
        {
            base.Init();
            posSpring = new Spring();
        }

        public override void OnPlay()
        {
            posSpring.Setup(Mass, Distance, Strength, Damping);
            v0 = (position - unityCamera.transform.position).normalized;
            diff = Vector3.zero;
        }

        public override void OnUpdate()
        {
            var rot = unityCamera.transform.rotation.eulerAngles;
            size = Size;

            var springDistance = posSpring.Calculate(Time.deltaTime);
            var ratio = 1.0f;

            switch (fadeState)
            {
                case FadeState.FadeIn:
                    ratio = Utils.Interpolation.LerpS3(0.0f, springDistance, 1.0f - fadeInNormalized);
                    size = Utils.Interpolation.LerpS3(0.0f, Size, 1.0f - fadeInNormalized);
                    break;

                case FadeState.FadeOut:
                    ratio = Utils.Interpolation.LerpS2(springDistance, 0.0f, fadeOutNormalized);
                    size = Utils.Interpolation.LerpS2(Size, 0.0f, fadeOutNormalized);
                    break;
            }

            var v1 = SmoothRandom.GetVector3(Speed) * size;
            var newRot = rot - diff + v1;
            diff = v1;

            unityCamera.transform.rotation = Quaternion.Euler(newRot);
            unityCamera.transform.position += v0 * springDistance * ratio;
        }
    }
}
