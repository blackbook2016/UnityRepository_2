// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections;
using System.Collections.Generic;
using RG_GameCamera.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RG_GameCamera.CollisionSystem
{
    /// <summary>
    /// advanced collision algorithm based on view volume
    /// volume is constructed from set of raycasts
    /// </summary>
    public class VolumetricCollision : ViewCollision
    {
        private readonly List<RaycastHit> hits;
        private readonly Ray[] rays;
        private readonly RayHitComparer rayHitComparer;

        public VolumetricCollision(Config.Config config)
            : base(config)
        {
            rayHitComparer = new RayHitComparer();

            const int maxRays = 10;

            hits = new List<RaycastHit>(maxRays * 4);
            rays = new Ray[maxRays];

            for (var i = 0; i < rays.Length; i++)
            {
                rays[i] = new Ray(Vector3.zero, Vector3.zero);
            }
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
            var coneRadius = config.GetVector2("ConeRadius");
            var coneSegments = config.GetFloat("ConeSegments");

            // construct collision volume from raycasts
            var p0 = cameraTarget;
            var p1 = cameraTarget - dir*distance;
            var right = Vector3.Cross(dir, Vector3.up);

            var v0 = Vector3.zero;

            for (var i = 0; i < coneSegments; i++)
            {
                var angle = i / coneSegments*360.0f;

                var q0 = Quaternion.AngleAxis(angle, dir);
                var pnt0 = p0 + q0 * (right*coneRadius.x);
                var pnt1 = p1 + q0 * (right*coneRadius.y);

                v0 = pnt1 - pnt0;

                rays[i].origin = pnt0;
                rays[i].direction = pnt1 - pnt0;
//                Debug.DrawLine(pnt0, pnt1, Color.yellow);
            }

            var rayDistance = v0.magnitude;

            hits.Clear();

            foreach (var ray in rays)
            {
                hits.AddRange(Physics.RaycastAll(ray, rayDistance + tollerance));
            }

            hits.Sort(rayHitComparer);

            var nearest = Mathf.Infinity;

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

        public class RayHitComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit x, RaycastHit y)
            {
                return x.distance.CompareTo(y.distance);
            }
        }
    }
}
