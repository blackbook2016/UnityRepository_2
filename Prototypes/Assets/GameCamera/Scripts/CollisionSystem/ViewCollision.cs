// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.CollisionSystem
{
    /// <summary>
    /// base class for view collision algorithm
    /// </summary>
    public abstract class ViewCollision
    {
        protected readonly Config.Config config;

        public enum CollisionClass
        {
            Collision,
            Trigger,
            Ignore,
            IgnoreTransparent,
        }

        protected ViewCollision(Config.Config config)
        {
            this.config = config;
        }

        /// <summary>
        /// calculate camera collision
        /// </summary>
        /// <param name="cameraTarget">target of the camera (character head)</param>
        /// <param name="cameraDir">view vector of camera</param>
        /// <param name="distance">optimal length of view vector</param>
        /// <returns>distance of view vector outside of collision</returns>
        public abstract float Process(Vector3 cameraTarget, Vector3 cameraDir, float distance);

        /// <summary>
        /// check if the collider is a valid camera collision
        /// </summary>
        /// <param name="collider">geometry collider</param>
        /// <returns>collision information</returns>
        public static CollisionClass GetCollisionClass(Collider collider, string ignoreTag, string transparentTag)
        {
            var collisionClass = CollisionClass.Collision;

            if (collider.isTrigger)
            {
                collisionClass = CollisionClass.Trigger;
            }
            else if (collider.gameObject != null)
            {
                if (collider.gameObject.GetComponent<IgnoreCollision>() || collider.gameObject.tag == ignoreTag)
                {
                    collisionClass = CollisionClass.Ignore;
                }
                else if (collider.gameObject.GetComponent<TransparentCollision>() || collider.gameObject.tag == transparentTag)
                {
                    collisionClass = CollisionClass.IgnoreTransparent;
                }
            }

            return collisionClass;
        }

        /// <summary>
        /// update transparency for objects that have camera transparency tag
        /// </summary>
        /// <param name="collider">collider</param>
        protected void UpdateTransparency(Collider collider)
        {
            TransparencyManager.Instance.UpdateObject(collider.gameObject);
        }
    }
}
