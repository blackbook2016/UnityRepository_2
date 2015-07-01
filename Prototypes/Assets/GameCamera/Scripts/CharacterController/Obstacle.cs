// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections;
using UnityEngine;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// helper class for finding obstacles
    /// </summary>
    public struct Obstacle
    {
        public ObstacleType Type;
        public Vector3 WallPoint;
        public Vector3 WallNormal;
        public float Height;
        public float Distance;
    }

    public enum ObstacleType
    {
        None,
        ObstacleLow,
        ObstacleMedium,
        ObstacleHigh,
    }

    public class ObstacleHelper
    {
        public static Obstacle FindObstacle(Vector3 pos, Vector3 dir, float maxDistance, float maxHeight, string ignoreTags)
        {
            var ray = new Ray(pos + Vector3.up*0.5f, dir);
            var hits = Physics.RaycastAll(ray, maxDistance);

            UnityEngine.Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow, 1);

            var nearest = Mathf.Infinity;
            var distancePoint = Vector3.zero;
            var wallNormal = Vector3.zero;
            var wallDetected = false;

            foreach (var hit in hits)
            {
                if (hit.distance < nearest && !hit.collider.isTrigger && hit.collider.gameObject.tag != ignoreTags)
                {
                    if (Vector3.Dot(hit.normal, Vector3.up) < 0.2f)
                    {
                        nearest = hit.distance;
                        distancePoint = hit.point;
                        wallNormal = hit.normal;
                        wallDetected = true;
                    }
                }
            }

            if (wallDetected)
            {
                var wallDistance = nearest;

//                Utils.DebugDraw.Sphere(distancePoint, 0.3f, Color.yellow, 1);

                ray.origin = distancePoint + dir * 0.25f + Vector3.up*maxHeight;
                ray.direction = Vector3.up*-1.0f;

                hits = Physics.RaycastAll(ray, maxHeight);

                Debug.DrawRay(ray.origin, ray.direction * maxHeight, Color.yellow, 1);

                nearest = Mathf.Infinity;
                var wallPoint = Vector3.zero;
                var topDetected = false;

                foreach (var hit in hits)
                {
                    if (hit.distance < nearest && !hit.collider.isTrigger && hit.collider.gameObject.tag != ignoreTags)
                    {
                        nearest = hit.distance;
                        wallPoint = hit.point;
                        topDetected = true;
                    }
                }

                if (topDetected)
                {
//                    Utils.DebugDraw.Sphere(wallPoint, 0.3f, Color.red, 1);

                    return new Obstacle
                    {
                        Distance = wallDistance,
                        Height = nearest,
                        WallPoint = wallPoint,
                        WallNormal = wallNormal,
                        Type = GetType(pos, wallPoint),
                    };
                }
            }

            return new Obstacle { Type = ObstacleType.None };
        }

        private static ObstacleType GetType(Vector3 ground, Vector3 wall)
        {
            var height = wall.y - ground.y;
            Utils.Debug.Assert(height > 0);

//            Utils.Debug.Log("Obstacle Height: {0}", height);

            if (height < 1.1)
            {
                return ObstacleType.ObstacleLow;
            }

            if (height < 1.6)
            {
                return ObstacleType.ObstacleMedium;
            }

//            // disable high obstacle since the animation doesn't look really good
//            if (height < 2.5)
//            {
//                return ObstacleType.ObstacleHigh;
//            }

            return ObstacleType.None;
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
