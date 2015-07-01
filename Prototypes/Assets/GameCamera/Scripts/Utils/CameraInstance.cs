// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Utils
{
    /// <summary>
    /// helper class for creating camera singletons and game objects
    /// </summary>
    class CameraInstance
    {
        public static string RootName = "GameCamera";

        private static GameObject cameraRoot;

        /// <summary>
        /// get camera root game object where all the camera modes and managers belongs to
        /// </summary>
        public static GameObject GetCameraRoot()
        {
            if (!cameraRoot)
            {
                cameraRoot = GameObject.Find(RootName);
                if (!cameraRoot)
                {
                    cameraRoot = new GameObject(RootName);
                }
            }

            return cameraRoot;
        }

        /// <summary>
        /// create a new instance of camera singleton class
        /// </summary>
        public static T CreateInstance<T>(string name) where T : Component
        {
            var root = GetCameraRoot();

            // check for early creation
            var comp = root.GetComponentInChildren<T>();

            if (comp)
            {
                return comp;
            }

            var obj = new GameObject(name);
            obj.transform.parent = root.transform;
            return obj.AddComponent<T>();
        }
    }
}
