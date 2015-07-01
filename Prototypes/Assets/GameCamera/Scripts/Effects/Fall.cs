// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Effects
{
    public class Fall : Effect
    {
        public float Mass;
        public float Distance;
        public float Strength;
        public float Damping;
        public float Force;
        public int ForceFrames;
        public float ImpactVelocity;

        private Spring spring;
        private float frameCounter;
        private float DistanceMax;

        public override void Init()
        {
            base.Init();
            spring = new Spring();
            spring.Setup(Mass, Distance, Strength, Damping);
        }

        public override void OnPlay()
        {
            frameCounter = ForceFrames;
            spring.Setup(Mass, Distance, Strength, Damping);
        }

        public override void OnUpdate()
        {
            if (frameCounter > 0)
            {
                spring.AddForce(Force);
                frameCounter--;
            }

            var springDistance = spring.Calculate(Time.deltaTime);
            var ratio = 1.0f;

            switch (fadeState)
            {
                case FadeState.FadeIn:
                    ratio = Utils.Interpolation.LerpS3(0.0f, springDistance, 1.0f - fadeInNormalized);
                    break;

                case FadeState.FadeOut:
                    ratio = Utils.Interpolation.LerpS2(springDistance, 0.0f, fadeOutNormalized);
                    break;
            }


            var rv = Mathf.Clamp01(ImpactVelocity/10.0f);
            DistanceMax = rv*2.0f;

            if (springDistance > DistanceMax)
            {
                springDistance = DistanceMax;
            }

            unityCamera.transform.position += Vector3.up * springDistance * ratio * -1;
        }
    }
}
