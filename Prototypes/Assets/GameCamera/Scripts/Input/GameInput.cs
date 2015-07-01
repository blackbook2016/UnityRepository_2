// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using RG_GameCamera.CharacterController;
using RG_GameCamera.CollisionSystem;
using UnityEngine;

namespace RG_GameCamera.Input
{
    public enum InputPreset
    {
        ThirdPerson,
        RTS,
        RPG,
        Orbit,
        FPS,
        Custom
    }

    public enum MousePanRotDirection
    {
        Horizontal_R = 1,
        Horizontal_L = -1,
        Vertical_U = 2,
        Vertical_D = -2,
    }

    public enum MoveMethod
    {
        Waypoint,
        Stick,
    }

    public abstract class GameInput : MonoBehaviour
    {
        protected InputFilter mouseFilter;
        protected InputFilter padFilter;
        protected float doubleClickTimeout;

        /// <summary>
        /// type
        /// </summary>
        public abstract InputPreset PresetType { get; }

        public bool ResetInputArray { get; protected set; }

        /// <summary>
        /// c-tor
        /// </summary>
        protected virtual void Awake()
        {
            mouseFilter = new InputFilter(10, 0.5f);
            padFilter = new InputFilter(10, 0.6f);
            ResetInputArray = true;
        }

        /// <summary>
        /// update input array
        /// </summary>
        public abstract void UpdateInput(Input[] inputs);

        /// <summary>
        /// update the input array
        /// </summary>
        /// <param name="inputs">input array</param>
        /// <param name="type">type of input</param>
        /// <param name="value">value of input</param>
        protected void SetInput(Input[] inputs, InputType type, object value)
        {
            if (inputs[(int) type].Enabled)
            {
                inputs[(int)type].Value = value;
                inputs[(int)type].Valid = true;
            }
        }

        /// <summary>
        /// find position in scene by projecting ray from mouse position
        /// </summary>
        /// <param name="mousePos">position in mouse coordinates</param>
        /// <param name="pos">found position</param>
        /// <returns>true if raycast hit the ground</returns>
        public static bool FindWaypointPosition(Vector2 mousePos, out Vector3 pos)
        {
            // run raycast in direction of camera
            var camera = CameraManager.Instance.UnityCamera;
            var ray = camera.ScreenPointToRay(mousePos);

            var hits = Physics.RaycastAll(ray, float.MaxValue);

            if (hits.Length == 0)
            {
                pos = Vector3.zero;
                return false;
            }

            Array.Sort(hits, delegate(RaycastHit x, RaycastHit y)
            {
                return x.distance.CompareTo(y.distance);
            });

            var nearest = float.MaxValue;
            var pnt = Vector3.zero;

            foreach (var hit in hits)
            {
                var coll = hit.collider;

                var cclass = CameraCollision.Instance.GetCollisionClass(coll);

                if (hit.distance < nearest && cclass == ViewCollision.CollisionClass.Collision)
                {
                    nearest = hit.distance;
                    pnt = hit.point;
                }
            }

            pos = pnt;

            return true;
        }
    }
}
