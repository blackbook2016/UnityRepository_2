// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// tweener is helper for tweening (smooth manipulation) of positions and rotations from A to B
    /// </summary>
    class Tweener : MonoBehaviour
    {
        public static Tweener Instance
        {
            get
            {
                if (!instance)
                {
                    instance = Utils.CameraInstance.CreateInstance<Tweener>("Tweener");
                }
                return instance;
            }
        }

        private static Tweener instance;

        /// <summary>
        /// move object to target position in time
        /// </summary>
        public void MoveTo(Transform trans, Vector3 targetPos, float time)
        {
            tweens.Add(new TweenPos
            {
                Transform = trans,
                StartPos = trans.position,
                TargetPos = targetPos,
                Time = time,
                Timeout = 0.0f,
            });
        }

        /// <summary>
        /// rotate object (y-axis) to target angle in time
        /// </summary>
        public void RotateTo(Transform trans, Quaternion rot, float time)
        {
            tweens.Add(new TweenRot
            {
                Transform = trans,
                StartRot = trans.rotation,
                TargetRot = rot,
                Time = time,
                Timeout = 0.0f,
            });
        }

        abstract private class Tween
        {
            public Transform Transform;
            public float Time;
            public float Timeout;

            public abstract void Update();
        }

        private class TweenPos : Tween
        {
            public Vector3 TargetPos;
            public Vector3 StartPos;

            public override void Update()
            {
                Timeout += UnityEngine.Time.deltaTime;

                var t = Timeout/Time;
                Transform.position = Vector3.Lerp(StartPos, TargetPos, t);
            }
        }

        private class TweenRot : Tween
        {
            public Quaternion TargetRot;
            public Quaternion StartRot;

            public override void Update()
            {
                Timeout += UnityEngine.Time.deltaTime;

                var t = Timeout / Time;
                Transform.rotation = Quaternion.Slerp(StartRot, TargetRot, t);
            }
        }

        private List<Tween> tweens;
        private List<Tween> finishedTweens;

        void Awake()
        {
            instance = this;
            tweens = new List<Tween>();
            finishedTweens = new List<Tween>();
        }

        void FixedUpdate()
        {
            foreach (var tween in tweens)
            {
                tween.Update();

                if (tween.Timeout >= tween.Time)
                {
                    finishedTweens.Add(tween);
                }
            }

            foreach (var finishedTween in finishedTweens)
            {
                tweens.Remove(finishedTween);
            }
            finishedTweens.Clear();
        }
    }
}
