// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Events
{
    /// <summary>
    /// camera event is a area with box collider that triggers various camera event (effect, change of paramater, etc)
    /// if the object (character) with camera trigger component hits it
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public partial class CameraEvent
    {
        private abstract class ITween
        {
            public string mode;
            public string key;
            public float time;
            public float timeout;
            public abstract void Interpolate(float t);
        }

        private class FloatTween : ITween
        {
            public float t0;
            public float t1;
            public override void Interpolate(float t)
            {
                var val = Utils.Interpolation.LerpS(t0, t1, t);
                var cfg = CameraManager.Instance.GetCameraMode().Configuration;
                cfg.SetFloat(mode, key, val);
            }
        }

        private class Vector2Tween : ITween
        {
            public Vector2 t0;
            public Vector2 t1;
            public override void Interpolate(float t)
            {
                var val = Utils.Interpolation.LerpS(t0, t1, t);
                var cfg = CameraManager.Instance.GetCameraMode().Configuration;
                cfg.SetVector2(mode, key, val);
            }
        }

        private class Vector3Tween : ITween
        {
            public Vector3 t0;
            public Vector3 t1;
            public override void Interpolate(float t)
            {
                var val = Utils.Interpolation.LerpS(t0, t1, t);
                var cfg = CameraManager.Instance.GetCameraMode().Configuration;
                cfg.SetVector3(mode, key, val);
            }
        }

        private List<ITween> tweens;

        void Awake()
        {
            tweens = new List<ITween>();
        }

        private void SmoothParam(string mode, string key, float t0, float t1, float time)
        {
            var tween = new FloatTween
            {
                key = key,
                mode = mode,
                t0 = t0,
                t1 = t1,
                time = time,
                timeout = time,
            };
            tweens.Add(tween);
        }

        private void SmoothParam(string mode, string key, Vector2 t0, Vector2 t1, float time)
        {
            var tween = new Vector2Tween
            {
                key = key,
                mode = mode,
                t0 = t0,
                t1 = t1,
                time = time,
                timeout = time,
            };
            tweens.Add(tween);
        }

        private void SmoothParam(string mode, string key, Vector3 t0, Vector3 t1, float time)
        {
            var tween = new Vector3Tween
            {
                key = key,
                mode = mode,
                t0 = t0,
                t1 = t1,
                time = time,
                timeout = time,
            };
            tweens.Add(tween);
        }

        private void Update()
        {
            foreach (var tween in tweens)
            {
                tween.timeout -= Time.deltaTime;
                var ts = 1.0f - Mathf.Clamp01(tween.timeout/tween.time);
                tween.Interpolate(ts);
                if (tween.timeout < 0.0f)
                {
                    tweens.Remove(tween);
                    break;
                }
            }

            if (cameraTrigger != null)
            {
                if (RestoreOnTimeout)
                {
                    restorationTimeout -= Time.deltaTime;

                    if (restorationTimeout < 0.0f)
                    {
                        Exit(true, cameraTrigger);
                    }
                }
            }
        }
    }
}
