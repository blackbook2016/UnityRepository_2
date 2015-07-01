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
    /// collision algorithm based on a single spherecast (based on Unity ThirdPersonCamera samples)
    /// good results
    /// </summary>
    public class SphericalCollision : ViewCollision
    {
        private Ray ray;
        private RaycastHit[] hits;
        private readonly RayHitComparer rayHitComparer;

        public SphericalCollision(Config.Config config)
            : base(config)
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
            var closestDistance = config.GetFloat("MinDistance");
            var tollerance = config.GetFloat("SphereCastTolerance");
            var sphereCastRadius = config.GetFloat("SphereCastRadius");

            ray.origin = cameraTarget + dir * sphereCastRadius;
            ray.direction = -dir;

//            Debug.DrawRay(cameraTarget, -dir * distance, Color.white);

            // initial check to see if start of spherecast intersects anything
            var cols = Physics.OverlapSphere(ray.origin, sphereCastRadius);

            var initialIntersect = false;

            var dontClipTag = config.GetString("IgnoreCollisionTag");
            var transparentClipTag = config.GetString("TransparentCollisionTag");

            // loop through all the collisions to check if something we care about
            for (int i = 0; i < cols.Length; i++)
            {
                var cclass = GetCollisionClass(cols[i], dontClipTag, transparentClipTag);

                if (cclass == CollisionClass.Collision)
                {
                    initialIntersect = true;
                    break;
                }
            }

            // if there is a collision 
            if (initialIntersect)
            {
                ray.origin += dir * sphereCastRadius;

                // do a raycast and gather all the intersections
                hits = Physics.RaycastAll(ray, distance - sphereCastRadius + tollerance);
            }
            else
            {

                // if there was no collision do a sphere cast to see if there were any other collisions
                hits = Physics.SphereCastAll(ray, sphereCastRadius, distance + sphereCastRadius);
            }

            // sort the collisions by distance
            Array.Sort(hits, rayHitComparer);

            // set the variable used for storing the closest to be as far as possible
            float nearest = Mathf.Infinity;

            // loop through all the collisions
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
