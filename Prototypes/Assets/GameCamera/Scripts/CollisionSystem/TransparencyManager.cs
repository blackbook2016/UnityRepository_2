// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.CollisionSystem
{
    /// <summary>
    /// transparency manager handles objects that have cameara transparent tag and are ignored from camera collision
    /// if the camera is inside the object collider, object becomes transparent
    /// this will work only for objects with transparency shader since it is changing color.a value
    /// </summary>
    public class TransparencyManager : MonoBehaviour
    {
        public static TransparencyManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = Utils.CameraInstance.CreateInstance<TransparencyManager>("TransparencyManager");
                }
                return instance;
            }
        }

        private static TransparencyManager instance;

        /// <summary>
        /// maximum transparency value (totally transparent is 1.0f, totally opaque is 0.0f)
        /// </summary>
        public float TransparencyMax = 0.5f;

        /// <summary>
        /// timeout when the object becomes fully opaque again after camera leaves the object collider
        /// </summary>
        public float TransparencyFadeOut = 0.2f;

        /// <summary>
        /// timeout when the object becomes transparent to max after camera enters the object collider
        /// </summary>
        public float TransparencyFadeIn = 0.1f;

        private float fadeVelocity;
        private const float fadeoutTimerMax = 0.1f;

        private class TransObject
        {
            public float originalAlpha;
            public bool fadeIn;
            public float fadeoutTimer;
        }

        private Dictionary<GameObject, TransObject> objects;

        void Awake()
        {
            instance = this;
            objects = new Dictionary<GameObject, TransObject>();
        }

        void Update()
        {
            foreach (var obj in objects)
            {
                obj.Value.fadeoutTimer += Time.deltaTime;

                if (obj.Value.fadeoutTimer > fadeoutTimerMax)
                {
                    obj.Value.fadeIn = false;
                }

                var alpha = GetAlpha(obj.Key);
                var remove = false;

                if (obj.Value.fadeIn)
                {
                    alpha = Mathf.SmoothDamp(alpha, TransparencyMax, ref fadeVelocity, TransparencyFadeIn);
                }
                else
                {
                    alpha = Mathf.SmoothDamp(alpha, obj.Value.originalAlpha, ref fadeVelocity, TransparencyFadeOut);

                    if (Mathf.Abs(alpha - obj.Value.originalAlpha) < Mathf.Epsilon)
                    {
                        remove = true;
                        alpha = obj.Value.originalAlpha;
                    }
                }

                SetAlpha(obj.Key, alpha);

                if (remove)
                {
                    objects.Remove(obj.Key);
                    break;
                }
            }
        }

        /// <summary>
        /// update transparency object
        /// </summary>
        /// <param name="obj">object that will become transparent while in collision with camera</param>
        public void UpdateObject(GameObject obj)
        {
            TransObject transObj = null;

            if (objects.TryGetValue(obj, out transObj))
            {
                transObj.fadeIn = true;
                transObj.fadeoutTimer = 0.0f;
            }
            else
            {
                objects.Add(obj, new TransObject { originalAlpha = GetAlpha(obj), fadeIn = true, fadeoutTimer = 0.0f });
            }
        }

        static void SetAlpha(GameObject obj, float alpha)
        {
            var render = obj.GetComponent<MeshRenderer>();

            if (render)
            {
                var material = render.sharedMaterial;

                if (material)
                {
                    var color = material.color;
                    color.a = alpha;
                    material.color = color;
                }
            }
        }

        static float GetAlpha(GameObject obj)
        {
            var render = obj.GetComponent<MeshRenderer>();

            if (render)
            {
                var material = render.sharedMaterial;

                if (material)
                {
                    return material.color.a;
                }
            }

            return 1.0f;
        }

        void OnApplicationQuit()
        {
            foreach (var obj in objects)
            {
                SetAlpha(obj.Key, obj.Value.originalAlpha);
            }
        }
    }
}
