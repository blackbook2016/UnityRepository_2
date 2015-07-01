// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Effects
{
    public class Stomp : Effect
    {
        public float Mass;
        public float Distance;
        public float Strength;
        public float Damping;

        private Spring spring;

        public override void Init()
        {
            base.Init();
            spring = new Spring();
        }

        public override void OnPlay()
        {
            spring.Setup(Mass, Distance, Strength, Damping);
        }

        public override void OnUpdate()
        {
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

            unityCamera.transform.position += Vector3.up*springDistance*ratio;
        }
    }
}
