// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Effects
{
    /// <summary>
    /// effects list
    /// </summary>
    public enum Type
    {
        Explosion,
        Stomp,
        Earthquake,
        Yes,
        No,
        FireKick,
        SprintShake,
    }

    /// <summary>
    /// this class represents base object for camera effect such as shake/explosion/earthquake
    /// </summary>
    public abstract class Effect : MonoBehaviour
    {
        /// <summary>
        /// effect playing in loop
        /// </summary>
        public bool Loop;

        /// <summary>
        /// time length of playback
        /// </summary>
        public float Length = 1.0f;

        /// <summary>
        /// time to fade-in
        /// </summary>
        public float FadeIn = 0.5f;

        /// <summary>
        /// time to fade-out
        /// </summary>
        public float FadeOut = 0.5f;

        /// <summary>
        /// is effect playing
        /// </summary>
        public bool Playing { get; protected set; }

        protected float timeout;
        protected float timeoutNormalized;
        protected float fadeInNormalized;
        protected float fadeOutNormalized;

        protected enum FadeState
        {
            FadeIn,
            Full,
            FadeOut,
        }

        protected FadeState fadeState;

        protected UnityEngine.Camera unityCamera;

        /// <summary>
        /// unity start
        /// </summary>
        void Start()
        {
            if (!unityCamera)
            {
                EffectManager.Instance.Register(this);
                Init();
            }
        }

        /// <summary>
        /// initialize
        /// </summary>
        public virtual void Init()
        {
            Playing = false;
            unityCamera = CameraManager.Instance.UnityCamera;
        }

        /// <summary>
        /// run the effect
        /// </summary>
        public void Play()
        {
            Playing = true;

            timeout = 0.0f;
            FadeIn = Mathf.Clamp(FadeIn, 0.0f, Length);
            FadeOut = Mathf.Clamp(FadeOut, 0.0f, Length);

            OnPlay();
        }

        /// <summary>
        /// stop the effect
        /// </summary>
        public void Stop()
        {
            Playing = false;
            OnStop();
        }

        /// <summary>
        /// play callback
        /// </summary>
        public virtual void OnPlay()
        {
        }

        /// <summary>
        /// stop callback
        /// </summary>
        public virtual void OnStop()
        {
        }

        /// <summary>
        /// update callback
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// update the effect
        /// </summary>
        public void PostUpdate()
        {
            timeout += Time.deltaTime;
            timeoutNormalized = Mathf.Clamp01(timeout/Length);

            fadeState = FadeState.Full;

            if (FadeIn > 0.0f)
            {
                if (timeout < FadeIn)
                {
                    fadeInNormalized = timeout / FadeIn;
                    fadeState = FadeState.FadeIn;
                }
                else
                {
                    fadeInNormalized = 1.0f;
                }
            }

            if (FadeOut > 0.0f)
            {
                if (timeout > Length - FadeOut)
                {
                    fadeOutNormalized = (timeout - (Length - FadeOut)) / FadeOut;
                    fadeState = FadeState.FadeOut;
                }
                else
                {
                    fadeOutNormalized = 0.0f;
                }
            }

            if (timeout > Length)
            {
                if (Loop)
                {
                    Play();
                }
                else
                {
                    Stop();
                }
            }

            OnUpdate();
        }

        /// <summary>
        /// pause the effect
        /// </summary>
        public void Delete()
        {
            EffectManager.Instance.Delete(this);
        }
    }
}
