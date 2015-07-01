// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem
{
    /// <summary>
    /// this will check the collision of camera target
    /// in case of the hit camera target will be moved in direction of character head (or above)
    /// it is used player is aiming, camera target (crosshair) is on right side from the head, ...
    /// player strafes to the wall and hit the wall collision
    /// </summary>
    public class TargetCollision
    {
        private Ray ray;
        private RaycastHit[] hits;
        private readonly RayHitComparer rayHitComparer;
        private readonly Config.Config config;

        public TargetCollision(Config.Config config)
        {
            rayHitComparer = new RayHitComparer();
            this.config = config;
        }

        public float CalculateTarget(Vector3 targetHead, Vector3 cameraTarget)
        {
            var dontClipTag = config.GetString("IgnoreCollisionTag");
            var transparentClipTag = config.GetString("TransparentCollisionTag");
            var radius = config.GetFloat("TargetSphereRadius");

            var newTarget = 1.0f;

            var rayDir = (cameraTarget - targetHead).normalized;
            var ray = new Ray(targetHead, rayDir);

            hits = Physics.RaycastAll(ray, rayDir.magnitude + radius);
            Array.Sort(hits, rayHitComparer);
            var nearest = Mathf.Infinity;
            var rayhit = false;

            foreach (var hit in hits)
            {
                var cclass = ViewCollision.GetCollisionClass(hit.collider, dontClipTag, transparentClipTag);

                if (hit.distance < nearest && cclass == ViewCollision.CollisionClass.Collision)
                {
                    nearest = hit.distance;
                    newTarget = hit.distance - radius;
                    rayhit = true;
                    Debug.DrawLine(targetHead, hit.point, Color.yellow);
                }
            }

            if (rayhit)
            {
                return Mathf.Clamp01(newTarget / (targetHead - cameraTarget).magnitude);
            }
            return 1.0f;
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
