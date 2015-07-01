// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using System.Collections;

namespace RG_GameCamera.Extras
{
    public class BloodScreenSplash : MonoBehaviour
    {
        public static BloodScreenSplash Instance;
        public float FadeoutTimer = 1.0f;
        public float MaxAlpha = 0.5f;

        private GUITexture bloodTexture;
        private float timeout;

        void Awake()
        {
            Instance = this;
            bloodTexture = GetComponent<GUITexture>();
        }

        public void Hit()
        {
            timeout = FadeoutTimer;
        }

        void Update()
        {
            var color = bloodTexture.color;
            color.a = Mathf.Clamp01(timeout/FadeoutTimer)*MaxAlpha;
            bloodTexture.color = color;

            timeout -= Time.deltaTime;
        }
    }
}
