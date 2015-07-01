// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections;
using RG_GameCamera.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RG_GameCamera.CollisionSystem
{
    /// <summary>
    /// collision algorithm based on one raycast
    /// very fast but not so smooth, good for slower mobile devices
    /// </summary>
    public class SimpleCollision : ViewCollision
    {
        private Ray ray;
        private RaycastHit[] hits;
        private readonly RayHitComparer rayHitComparer;

        public SimpleCollision(Config.Config config) : base(config)
        {
            rayHitComparer = new RayHitComparer();
        }

        /// <summary>
        /// calculate camera collision
        /// </summary>
        /// <param name="cameraTarget">target of the camera (character head)</param>
        /// <param name="dir">view vector of camera</param>
        /// <param name="distance">optimal length of view vector</param>
        /// <returns>distance of view vector outside of collision</returns>
        public override float Process(Vector3 cameraTarget, Vector3 dir, float distance)
        {
            // initially set the target distance
            var targetDist = distance;
            var tollerance = config.GetFloat("RaycastTolerance");
            var closestDistance = config.GetFloat("MinDistance");

            var nearest = Mathf.Infinity;

//            Debug.DrawRay(cameraTarget, -dir * distance, Color.white);

            ray.origin = cameraTarget;
            ray.direction = -dir;

            hits = Physics.RaycastAll(ray, distance + tollerance);

            Array.Sort(hits, rayHitComparer);

            var dontClipTag = config.GetString("IgnoreCollisionTag");
            var transparentClipTag = config.GetString("TransparentCollisionTag");

            foreach (var hit in hits)
            {
                var cclass = GetCollisionClass(hit.collider, dontClipTag, transparentClipTag);

                if (hit.distance < nearest && cclass == CollisionClass.Collision)
                {
                    nearest = hit.distance;
                    targetDist = hit.distance - tollerance;
//                    DebugDraw.Sphere(hit.point, 0.1f, Color.red, 1);
                }

                if (cclass == CollisionClass.IgnoreTransparent)
                {
                    UpdateTransparency(hit.collider);
                }
            }

            return Mathf.Clamp(targetDist, closestDistance, distance);
        }

        public class RayHitComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
            }
        }
    }
}
