// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace RG_GameCamera.Utils
{
    class DebugDraw : MonoBehaviour
    {
        private static DebugDraw Instance
        {
            get
            {
                if (!instance)
                {
                    instance = Utils.CameraInstance.CreateInstance<DebugDraw>("DebugDraw");
                }
                return instance;
            }
        }

        public static bool Enabled = true;

        private static DebugDraw instance;
        private GameObject debugRoot;

        private class DbgObject
        {
            public GameObject obj;
            public int timer;
        }

        private DbgObject[] dbgObjects;

        void Awake()
        {
            instance = this;
            debugRoot = instance.gameObject;
            dbgObjects = new DbgObject[20];
            for (int i = 0; i < dbgObjects.Length; i++)
            {
                dbgObjects[i] = new DbgObject();
            }
        }

        void Update()
        {
            debugRoot.SetActive(Enabled);

            foreach (var dbgObject in dbgObjects)
            {
                if (dbgObject.obj)
                {
                    dbgObject.timer -= 1;

                    if (dbgObject.timer < 0.0f)
                    {
                        dbgObject.obj.gameObject.SetActive(false);
                    }
                }
            }
        }

        [Conditional("UNITY_EDITOR")]
        public static void Sphere(Vector3 pos, float scale, Color color, int time)
        {
            var inst = Instance;
            var hit = false;
            DbgObject emptyObj = null;

            foreach (var dbgObject in inst.dbgObjects)
            {
                if (dbgObject.obj && !Utils.Debug.IsActive(dbgObject.obj))
                {
                    dbgObject.obj.SetActive(true);
                    dbgObject.obj.transform.position = pos;
                    dbgObject.obj.transform.localScale = new Vector3(scale, scale, scale);
                    dbgObject.timer = time;
                    dbgObject.obj.GetComponent<MeshRenderer>().material.color = color;
                    hit = true;
                    break;
                }

                if (!dbgObject.obj)
                {
                    emptyObj = dbgObject;
                }
            }

            if (!hit)
            {
                if (emptyObj != null)
                {
                    emptyObj.obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Destroy(emptyObj.obj.GetComponent<SphereCollider>());
                    emptyObj.obj.transform.position = pos;
                    emptyObj.obj.transform.parent = inst.debugRoot.transform;
                    emptyObj.timer = time;
                    var material = new Material(Shader.Find("VertexLit"));
                    emptyObj.obj.GetComponent<MeshRenderer>().material = material;
                    material.color = color;
                    hit = true;
                }
            }

            if (!hit)
            {
                Array.Sort(inst.dbgObjects, delegate(DbgObject obj0, DbgObject obj1)
                {
                    return obj0.timer.CompareTo(obj1.timer);
                });

                var obj = inst.dbgObjects[0];
                obj.obj.SetActive(true);
                obj.obj.transform.position = pos;
                obj.obj.transform.localScale = new Vector3(scale, scale, scale);
                obj.timer = time;
                obj.obj.GetComponent<MeshRenderer>().material.color = color;
            }
        }
    }
}
