// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using System.Collections;

namespace RG_GameCamera.Extras
{
    /// <summary>
    /// this class manages a projector for making "highlighted position effect" when moving unit in RTS game to a new position
    /// </summary>
    [RequireComponent(typeof(Projector))]
    public class RTSProjector : MonoBehaviour
    {
        public static RTSProjector Instance;
        public float AnimTimeout;
        public float Distance;
        public float FovMax;
        public float FovMin;

        private Projector projector;
        private float timeout;

        void Awake()
        {
            Instance = this;
            projector = GetComponent<Projector>();
            Utils.Debug.Assert(projector != null, "Missing projector componnent!");
        }

        public void Enable()
        {
            projector.enabled = true;
        }

        public void Disable()
        {
            projector.enabled = false;
        }

        public void Project(Vector3 pos, Color color)
        {
            projector.material.color = color;
            Enable();
            projector.fieldOfView = FovMax;
            timeout = AnimTimeout;
            transform.position = pos + Vector3.up*Distance;
        }

        void Update()
        {
            timeout -= Time.deltaTime;

            if (timeout > 0.0f)
            {
                var t = (timeout/AnimTimeout);

                float tt;

                if (t > 0.5)
                {
                    tt = (t - 0.5f)/0.5f;
                }
                else
                {
                    tt = t/0.5f;
                }

                projector.fieldOfView = FovMin + (FovMax - FovMin)*tt;
            }
            else
            {
                projector.fieldOfView = FovMax;
            }
        }
    }
}
